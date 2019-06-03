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
            ["@>>"] = OperatorKind.PipeLeft,
            ["<<@"] = OperatorKind.PipeRight,
            ["&"] = OperatorKind.Ref,
            ["$"] = OperatorKind.Value,
            ["^"] = OperatorKind.AltRef,
            ["("] = OperatorKind.OpenParen,
            [")"] = OperatorKind.CloseParen,
        };

        public static Parser<IInputToken[]> InstructionParser => (
                from many in
                    SwapToken

                        .Or(JumpT)
                        .Or(RefT)
                        .Or(PushA).Or(PushD).Or(PushX)
                        .Or(LoadI)

                        .Or(ByIIDToken(InsID.halt))
                        .Or(ByIIDToken(InsID.warm))

                        .Or(MathInstruction(InsID.add))
                        .Or(MathInstruction(InsID.mul))
                        .Or(MathInstruction(InsID.sub))
                        .Or(MathInstruction(InsID.div))
                        .Or(MathInstruction(InsID.pow))

                        .Or(PushJ)

                select many)
            .ContinueMany()
            .Select(x => x.ToArray());

        public static Parser<char> CharToken =
            (from _1 in Parse.Char('\'')
                from @char in Parse.AnyChar
                from _2 in Parse.Char('\'')
             select @char)
            .Token()
            .Named("char token");
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
            (from _ in Parse.String("@>>")
                select OperatorKind.PipeLeft)
            .Token()
            .NamedOperator(OperatorKind.PipeLeft);
        public static Parser<OperatorKind> PipeRight =>
            (from _ in Parse.String("<<@")
                select OperatorKind.PipeRight)
            .Token()
            .NamedOperator(OperatorKind.PipeRight);
        public static Parser<RefExpression> RefToken =
            (from refSym in Parse.Char('&')
                from openParen in Parse.Char('(')
                from cellID in HexNumber
                from closeParen in Parse.Char(')')
                select new RefExpression(short.Parse(cellID, NumberStyles.AllowHexSpecifier)))
            .Token()
            .WithPosition()
            .Named("ref_token");
        public static Parser<RefExpression> ValueToken =
            (from refSym in Parse.Char('$')
                from openParen in Parse.Char('(')
                from cellID in HexNumber
                from closeParen in Parse.Char(')')
                select new RefExpression(short.Parse(cellID, NumberStyles.AllowHexSpecifier)))
            .Token()
            .WithPosition()
            .Named("value_token");
        public static Parser<short> CastCharToken =
            (from refSym in Parse.String("@char_t")
             from openParen in Parse.Char('(')
                from @char in CharToken
             from closeParen in Parse.Char(')')
                select (short)@char)
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
                from cell2 in ValueToken
                select new InstructionExpression(new loadi(cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("loadi expression");
        public static Parser<IInputToken> JumpT =>
            (from dword in InstructionToken(InsID.jump_t)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                select new InstructionExpression(new jump_t(cell1.Cell)))
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
                from op2 in PipeLeft
                from cell3 in ValueToken.Or(CastCharToken.Select(x => new RefExpression(x)))
             select new InstructionExpression(new push_a(cell1.Cell, cell2.Cell, cell3.Cell)))
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
                from cell1 in RefToken
                from cell2 in RefToken
                select new InstructionExpression(Instruction.Summon(id, (ushort)cell1.Cell, (ushort)cell2.Cell)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");
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

        #endregion

    }

    public class TransformationContext : IInputToken
    {
        public Instruction[] Instructions { get; set; }
        public Position InputPosition { get; set; }
    }

    public class TransformPushJ : TransformationContext
    {
        public TransformPushJ(string value, short cellDev, short ActionDev)
        {
            Instructions = value.Select(x => new push_a(cellDev, ActionDev, (short) x)).Cast<Instruction>().ToArray();
        }
    }
}