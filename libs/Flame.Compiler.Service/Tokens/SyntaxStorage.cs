namespace flame.compiler.tokens
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using runtime;
    using Sprache;

    public class SyntaxStorage
    {
        internal static readonly Dictionary<string, OperatorKind> Operators = new Dictionary<string, OperatorKind>
        {
            ["."] = OperatorKind.Dot,
            ["|>"] = OperatorKind.PipeLeft,
            ["<|"] = OperatorKind.PipeRight,
            ["&"] = OperatorKind.Ref,
            ["$"] = OperatorKind.Value,
            ["^"] = OperatorKind.AltRef,
            ["("] = OperatorKind.OpenParen,
            [")"] = OperatorKind.CloseParen,
            ["-~"] = OperatorKind.When
        };

        public static Parser<IInputToken[]> InstructionParser => (
                from many in
                    CommentToken
                        // base instruction token
                        .Or(SwapToken)
                        .Or(RefT)
                        .Or(PushA).Or(PushD).Or(PushX)
                        .Or(LoadI)
                        .Or(LoadI_X)
                        // jumps
                        .Or(JumpT)
                        .Or(JumpAt(InsID.jump_e))
                        .Or(JumpAt(InsID.jump_g))
                        .Or(JumpAt(InsID.jump_u))
                        .Or(JumpAt(InsID.jump_y))
                        // empty instruction token
                        .Or(ByIIDToken(InsID.halt))
                        .Or(ByIIDToken(InsID.warm))
                        // math instruction token
                        .Or(MathInstruction(InsID.add))
                        .Or(MathInstruction(InsID.mul))
                        .Or(MathInstruction(InsID.sub))
                        .Or(MathInstruction(InsID.div))
                        .Or(MathInstruction(InsID.pow))
                        .Or(SqrtToken)
                        // tramsformation instruction token
                        .Or(PushJ)
                select many)
            .ContinueMany()
            .Select(x => x.ToArray());

        public static Parser<CommentToken> CommentToken =
            (from comment in new CommentParser(";",null, null, "\n").SingleLineComment
                select new CommentToken(comment))
            .Token()
            .Named("comment token");

        public static Parser<char> CharToken =
               (from _1 in Parse.Char('\'')
                from @char in Parse.AnyChar
                from _2 in Parse.Char('\'')
             select @char)
            .Token()
            .Named("char token");

        public static Parser<string> IdentifierToken =
            (from word in Parse.AnyChar.Except(Parse.Char(' ')).Many().Text()
            select word)
            .Token()
            .Named("identifier token");

        public static Parser<string> Keyword(string keyword) =>
            (from word in Parse.String(keyword).Text()
                select word)
            .Token()
            .Named($"keyword {keyword} token");

        public static Parser<string> StringToken =
               (from open in Parse.Char('"')
                from @string in Parse.AnyChar.Except(Parse.Char('"')).Many().Text()
                from close in Parse.Char('"')
             select @string)
            .Token()
            .Named("char token");
        public static Parser<string> HexNumber =
            (from zero in Parse.Char('0')
                from x in Parse.Chars("x")
                from number in Parse.Chars("0xABCDEF123456789").Many().Text()
                select number)
            .Token()
            .Named("hex number");

        #region Operator tokens
        public static Parser<OperatorKind> PipeLeft =>
            (from _ in Parse.String("|>")
                select OperatorKind.PipeLeft)
            .Token()
            .NamedOperator(OperatorKind.PipeLeft);
        public static Parser<OperatorKind> PipeRight =>
            (from _ in Parse.String("<|")
                select OperatorKind.PipeRight)
            .Token()
            .NamedOperator(OperatorKind.PipeRight);

        public static Parser<OperatorKind> When =>
            (from _ in Parse.String("-~")
                select OperatorKind.When)
            .Token()
            .NamedOperator(OperatorKind.When);
        public static Parser<RefExpression> RefToken =
            (from refSym in Parse.Char('&')
                from openParen in Parse.Char('(')
                from cellID in HexNumber
                from closeParen in Parse.Char(')')
                select new RefExpression(byte.Parse(cellID, NumberStyles.AllowHexSpecifier)))
            .Token()
            .WithPosition()
            .Named("ref_token");
        public static Parser<ValueExpression> ValueToken =
            (from refSym in Parse.Char('$')
                from openParen in Parse.Char('(')
                from value in HexNumber
                from closeParen in Parse.Char(')')
                select new ValueExpression(ushort.Parse(value, NumberStyles.AllowHexSpecifier)))
            .Token()
            .WithPosition()
            .Named("value_token");
        public static Parser<ushort> CastCharToken =
            (from refSym in Parse.String("@char_t")
                 from openParen in Parse.Char('(')
                 from @char in CharToken
                 from closeParen in Parse.Char(')')
                 select (ushort)@char)
            .Token()
            .Named("char_t expression");
        public static Parser<string> CastStringToken =
            (from refSym in Parse.String("@string_t")
                from openParen in Parse.Char('(')
                from @string in StringToken
                from closeParen in Parse.Char(')')
                select @string)
            .Token()
            .Named("string_t expression");
        #endregion
        #region Instructuions token
        public static Parser<IInputToken> LoadI =>
            (from dword in InstructionToken(InsID.loadi)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from pipe in PipeRight
                from val1 in ValueToken
                select new InstructionExpression(new loadi(cell1.Cell, val1.Value)))
            .Token()
            .WithPosition()
            .Named("loadi expression");
        public static Parser<IInputToken> LoadI_X =>
            (from dword in InstructionToken(InsID.loadi_x)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from pipe in PipeRight
                from val1 in ValueToken
                select new InstructionExpression(new loadi_x(cell1.Cell, val1.Value)))
            .Token()
            .WithPosition()
            .Named("loadi_x expression");
        public static Parser<IInputToken> JumpT =>
            (from dword in InstructionToken(InsID.jump_t)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                select new InstructionExpression(new jump_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_t expression");

        public static Parser<IInputToken> JumpAt(InsID id) =>
            (from dword in InstructionToken(id)
                from space1 in Parse.WhiteSpace.Optional()
                from cell0 in RefToken
                from _ in When
                from cell1 in RefToken
                from cell2 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell, cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_t expression");
        public static Parser<IInputToken> SwapToken =>
            (from dword in InstructionToken(InsID.swap)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from space2 in Parse.WhiteSpace.Optional()
                from cell2 in RefToken
                select new InstructionExpression(new swap(cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("swap expression");
        public static Parser<IInputToken> PushA =>
            (from dword in InstructionToken(InsID.push_a)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeRight
                from cell3 in ValueToken.Or(CastCharToken.Select(x => new ValueExpression(x)))
             select new InstructionExpression(new push_a(cell1.Cell, cell2.Cell, cell3.Value)))
            .Token()
            .WithPosition()
            .Named("push_a expression");
        public static Parser<IInputToken> PushD =>
            (from dword in InstructionToken(InsID.push_d)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new push_d(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("push_d expression");
        public static Parser<IInputToken> PushX =>
            (from dword in InstructionToken(InsID.push_x)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new push_x_debug(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("push_x expression");
        public static Parser<IInputToken> RefT => (
                from dword in InstructionToken(InsID.ref_t)
                from cell1 in RefToken
                select new InstructionExpression(new ref_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("ref_t expression");

        public static Parser<IInputToken> MathInstruction(InsID id) => (
                from dword in InstructionToken(id)
                from cell0 in RefToken
                from cell1 in RefToken
                from cell2 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell, cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        public static Parser<IInputToken> SqrtToken => (
                from dword in InstructionToken(InsID.sqrt)
                from cell0 in RefToken
                from cell1 in RefToken
                select new InstructionExpression(Instruction.Summon(InsID.sqrt, cell0.Cell, cell1.Cell)))
            .Token()
            .WithPosition()
            .Named($"sqrt expression");
        #endregion
        #region etc tokens
        public static Parser<string> InstructionToken(InsID instruction) =>
            from dot in Parse.Char('.')
            from ident in Parse.String(instruction.ToString()).Text()
            select ident;
        public static Parser<InstructionExpression> ByIIDToken(InsID id) =>
            (from dword in InstructionToken(id)
                select new InstructionExpression(Instruction.Summon(id)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        #endregion

        #region TramsformToken

        public static Parser<IInputToken> PushJ =>
            (from dword in InstructionToken(InsID.push_j)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeRight
                from cell3 in CastStringToken
             select new TransformPushJ(cell3, cell1.Cell, cell2.Cell))
            .Token()
            .WithPosition()
            .Named("push_j transform expression");
        public static Parser<IInputToken> Label => (
                from dword in InstructionToken(InsID.label)
                from nameOfLabel in IdentifierToken
                from auto in Keyword("auto").Optional()
                from id in RefToken.Optional()
                select new LabelTransform(nameOfLabel, auto.IsDefined, id.GetOrDefault()?.Cell))
            .Token()
            .WithPosition()
            .Named("label transform expression");
        #endregion

    }
    
    public class LabelTransform : TransformationContext
    {
        public LabelTransform(string name, bool isAuto, byte? cell_id)
        {
            Instructions = name.Select((v, i) => new label(v, isAuto, i == name.Length, cell_id)).Cast<Instruction>().ToArray();
        }
    }

    public class CommentToken : IInputToken
    {
        public readonly string _comment;
        public Position InputPosition { get; set; }
        public CommentToken(string comment) => _comment = comment;
    }
}