namespace ancient.compiler.tokens
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using runtime;
    using Sprache;

    public class FlameAssemblerSyntax
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
            ["~-"] = OperatorKind.When
        };

        internal static readonly Dictionary<OperatorKind, string> OperatorsReversed =
            Operators.Reverse().ToDictionary(x => x.Value, x => x.Key);

        public virtual Parser<IInputToken> Parser => CommentToken
            // base instruction token
            .Or(SwapToken)
            .Or(RefT)
            .Or(PushA).Or(PushD).Or(PushX)
            .Or(LoadI)
            .Or(LoadI_X)
            .Or(LoadI_S)
            .Or(OuT)
            //etc
            .Or(StageN)
            .Or(NValue)
            .Or(Raw)
            // jumps
            .Or(JumpT)
            .Or(JumpAt(InsID.jump_e))
            .Or(JumpAt(InsID.jump_g))
            .Or(JumpAt(InsID.jump_u))
            .Or(JumpAt(InsID.jump_y))
            // empty instruction token
            .Or(ByIIDToken(InsID.halt))
            .Or(ByIIDToken(InsID.warm))
            // break instruction
            .Or(ByIIDToken(InsID.brk_a))
            .Or(ByIIDToken(InsID.brk_n))
            .Or(ByIIDToken(InsID.brk_s))
            // math instruction token
            .Or(MathInstruction(InsID.add))
            .Or(MathInstruction(InsID.mul))
            .Or(MathInstruction(InsID.sub))
            .Or(MathInstruction(InsID.div))
            .Or(MathInstruction(InsID.pow))
            .Or(SqrtToken);

        public virtual Parser<IInputToken[]> ManyParser => (
                from many in
                    Parser
                select many)
            .ContinueMany()
            .Select(x => x.ToArray());

        public virtual Parser<CommentToken> CommentToken =>
            (from comment in new CommentParser(";",null, null, "\n").SingleLineComment
                select new CommentToken(comment))
            .Token()
            .Named("comment token");

        public virtual Parser<char> CharToken =>
               (from _1 in Parse.Char('\'')
                from @char in Parse.AnyChar
                from _2 in Parse.Char('\'')
             select @char)
            .Token()
            .Named("char token");
        public virtual Parser<string> QuoteIdentifierToken =>
            (from open in Parse.Char('\'')
                from @string in Parse.AnyChar.Except(Parse.Char('\'')).Many().Text()
                from close in Parse.Char('\'')
                select @string)
            .Token()
            .Named("quote string token");

        public virtual Parser<string> IdentifierToken =>
            (from word in Parse.AnyChar.Except(Parse.Char(' ')).Many().Text()
            select word)
            .Token()
            .Named("identifier token");

        public virtual Parser<string> Keyword(string keyword) =>
            (from word in Parse.String(keyword).Text()
                select word)
            .Token()
            .Named($"keyword {keyword} token");

        public virtual Parser<string> StringToken =>
               (from open in Parse.Char('"')
                from @string in Parse.AnyChar.Except(Parse.Char('"')).Many().Text()
                from close in Parse.Char('"')
             select @string)
            .Token()
            .Named("string token");

        public virtual Parser<string> FloatToken =>
            (from @string in Parse.Decimal
                select @string)
            .Token()
            .Named("string token");

        
        public virtual Parser<string> HexNumber =>
            (from zero in Parse.Char('0')
                from x in Parse.Chars("x")
                from number in Parse.Chars("0xABCDEF123456789").Many().Text()
                select number)
            .Token()
            .Named("hex number");

        #region Operator tokens
        public virtual Parser<OperatorKind> PipeLeft =>
            (from _ in Parse.String("|>")
                select OperatorKind.PipeLeft)
            .Token()
            .NamedOperator(OperatorKind.PipeLeft);
        public virtual Parser<OperatorKind> PipeRight =>
            (from _ in Parse.String("<|")
                select OperatorKind.PipeRight)
            .Token()
            .NamedOperator(OperatorKind.PipeRight);

        public virtual Parser<OperatorKind> When =>
            (from _ in Parse.String(OperatorsReversed[OperatorKind.When])
                select OperatorKind.When)
            .Token()
            .NamedOperator(OperatorKind.When);
        public virtual Parser<RefExpression> RefToken =>
            (from refSym in Parse.Char('&')
                from openParen in Parse.Char('(')
                from cellID in HexNumber
                from closeParen in Parse.Char(')')
                select new RefExpression(cellID))
            .Token()
            .WithPosition()
            .Named("ref_token");
        public virtual Parser<ValueExpression> ValueToken =>
            (from refSym in Parse.Char('$')
                from openParen in Parse.Char('(')
                from value in HexNumber
                from closeParen in Parse.Char(')')
                select new ValueExpression(value))
            .Token()
            .WithPosition()
            .Named("value_token");
        public virtual Parser<ushort> CastCharToken =>
            (from refSym in Parse.String("@char_t")
                 from openParen in Parse.Char('(')
                 from @char in CharToken
                 from closeParen in Parse.Char(')')
                 select (ushort)@char)
            .Token()
            .Named("char_t expression");
        public virtual Parser<string> CastStringToken =>
            (from refSym in Parse.String("@string_t")
                from openParen in Parse.Char('(')
                from @string in StringToken
                from closeParen in Parse.Char(')')
                select @string)
            .Token()
            .Named("string_t expression");

        public virtual Parser<float> CastFloat =>
            (from refSym in Parse.String("@float_t")
                from openParen in Parse.Char('(')
                from @string in StringToken
                from closeParen in Parse.Char(')')
                select float.Parse(@string, CultureInfo.InvariantCulture))
            .Token()
            .Named("string_t expression");
        #endregion
        #region Instructuions token
        public virtual Parser<IInputToken> Raw =>
            (from dword in InstructionToken(InsID.raw)
                from space1 in Parse.WhiteSpace.Optional()
                from val1 in HexNumber
                select new InstructionExpression(new raw(ulong.Parse(val1, NumberStyles.AllowHexSpecifier))))
            .Token()
            .WithPosition()
            .Named("raw expression");

        public virtual Parser<IInputToken> StageN =>
            (from dword in InstructionToken(InsID.stage_n)
                from val1 in RefToken
                select new InstructionExpression(new stage_n(val1.Cell)))
            .Token()
            .WithPosition()
            .Named("stage_n expression");

        public virtual Parser<IInputToken> NValue =>
            (from dword in InstructionToken(InsID.n_value)
                from val1 in CastFloat
                select new InstructionExpression(new n_value(val1)))
            .Token()
            .WithPosition()
            .Named("n_value expression");


        public virtual Parser<IInputToken> LoadI =>
            (from dword in InstructionToken(InsID.loadi)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from pipe in PipeRight
                from val1 in ValueToken
                select new InstructionExpression(new loadi(cell1.Cell, val1.Value)))
            .Token()
            .WithPosition()
            .Named("loadi expression");
        public virtual Parser<IInputToken> LoadI_X =>
            (from dword in InstructionToken(InsID.loadi_x)
                from cell1 in RefToken
                from pipe in PipeRight
                from val1 in ValueToken
                select new InstructionExpression(new loadi_x(cell1.Cell, val1.Value)))
            .Token()
            .WithPosition()
            .Named("loadi_x expression");

        public virtual Parser<IInputToken> LoadI_S =>
            (from dword in InstructionToken(InsID.loadi_s)
                from cell1 in RefToken
                select new InstructionExpression(new loadi_s(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("loadi_s expression");
        public virtual Parser<IInputToken> JumpT =>
            (from dword in InstructionToken(InsID.jump_t)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                select new InstructionExpression(new jump_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_t expression");

        public virtual Parser<IInputToken> JumpAt(InsID id) =>
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
        public virtual Parser<IInputToken> SwapToken =>
            (from dword in InstructionToken(InsID.swap)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from space2 in Parse.WhiteSpace.Optional()
                from cell2 in RefToken
                select new InstructionExpression(new swap(cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("swap expression");
        public virtual Parser<IInputToken> OuT =>
            (from dword in InstructionToken(InsID.ou_t)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new ou_t(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("push_a expression");
        public virtual Parser<IInputToken> PushA =>
            (from dword in InstructionToken(InsID.push_a)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeRight
                from cell3 in ValueToken.Or(CastCharToken.Select(x => new ValueExpression($"{x:x}")))
             select new InstructionExpression(new push_a(cell1.Cell, cell2.Cell, cell3.Value)))
            .Token()
            .WithPosition()
            .Named("push_a expression");
        public virtual Parser<IInputToken> PushD =>
            (from dword in InstructionToken(InsID.push_d)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new push_d(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("push_d expression");
        public virtual Parser<IInputToken> PushX =>
            (from dword in InstructionToken(InsID.push_x)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new push_x_debug(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("push_x expression");
        public virtual Parser<IInputToken> RefT => (
                from dword in InstructionToken(InsID.ref_t)
                from cell1 in RefToken
                select new InstructionExpression(new ref_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("ref_t expression");

        public virtual Parser<IInputToken> MathInstruction(InsID id) => (
                from dword in InstructionToken(id)
                from cell0 in RefToken
                from cell1 in RefToken
                from cell2 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell, cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        public virtual Parser<IInputToken> SqrtToken => (
                from dword in InstructionToken(InsID.sqrt)
                from cell0 in RefToken
                from cell1 in RefToken
                select new InstructionExpression(Instruction.Summon(InsID.sqrt, cell0.Cell, cell1.Cell)))
            .Token()
            .WithPosition()
            .Named($"sqrt expression");
        #endregion
        #region etc tokens
        public virtual Parser<string> ProcToken(string name) =>
            (from dot in Parse.Char('~')
                from ident in Parse.String(name).Text()
                select ident)
            .Token()
            .Named("~ident");
        public virtual Parser<string> InstructionToken(InsID instruction) =>
            from dot in Parse.Char('.')
            from ident in Parse.String(instruction.ToString()).Text()
            select ident;
        public virtual Parser<InstructionExpression> ByIIDToken(InsID id) =>
            (from dword in InstructionToken(id)
                select new InstructionExpression(Instruction.Summon(id)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

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