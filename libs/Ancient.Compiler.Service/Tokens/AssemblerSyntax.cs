namespace ancient.compiler.tokens
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using runtime;
    using Sprache;

    public class AssemblerSyntax
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
            .Or(RFD)
            //etc
            .Or(StageN)
            .Or(NValue)
            .Or(Raw)
            .Or(CKFT)
            .Or(Dup)
            // jumps
            .Or(JumpT)
            .Or(JumpAt(IID.jump_e))
            .Or(JumpAt(IID.jump_g))
            .Or(JumpAt(IID.jump_u))
            .Or(JumpAt(IID.jump_y))
            // empty instruction token
            .Or(ByIIDToken(IID.halt))
            .Or(ByIIDToken(IID.warm))
            // break instruction
            .Or(ByIIDToken(IID.brk_a))
            .Or(ByIIDToken(IID.brk_n))
            .Or(ByIIDToken(IID.brk_s))
            // etc
            .Or(ByIIDToken(IID.nop))
            // math instruction token
            .Or(MathInstruction(IID.add))
            .Or(MathInstruction(IID.mul))
            .Or(MathInstruction(IID.sub))
            .Or(MathInstruction(IID.div))
            .Or(MathInstruction(IID.pow))
            .Or(SqrtToken)
            // advanced math
            .Or(AdvMathInstruction(IID.abs)).Or(AdvMathInstruction(IID.acos)).Or(AdvMathInstruction(IID.atan))
            .Or(AdvMathInstruction(IID.acosh)).Or(AdvMathInstruction(IID.atanh)).Or(AdvMathInstruction(IID.asin))
            .Or(AdvMathInstruction(IID.asinh)).Or(AdvMathInstruction(IID.cbrt)).Or(AdvMathInstruction(IID.cell))
            .Or(AdvMathInstruction(IID.cos)).Or(AdvMathInstruction(IID.cosh)).Or(AdvMathInstruction(IID.flr))
            .Or(AdvMathInstruction(IID.exp)).Or(AdvMathInstruction(IID.log)).Or(AdvMathInstruction(IID.log10))
            .Or(AdvMathInstruction(IID.tan)).Or(AdvMathInstruction(IID.tanh)).Or(AdvMathInstruction(IID.trc))
            .Or(AdvMathInstruction(IID.biti)).Or(AdvMathInstruction(IID.bitd)).Or(AdvMathInstruction(IID.sin))
            .Or(AdvMathInstruction(IID.sinh))

            .Or(AdvMathInstruction(IID.inc))
            .Or(AdvMathInstruction(IID.dec))

            .Or(Adv2MathInstruction(IID.min))
            .Or(Adv2MathInstruction(IID.max))
            .Or(Adv2MathInstruction(IID.atan2))
        ;

        #region Segments
        public virtual Parser<IInputToken[]> ManyParser => (
                from many in
                    Parser
                select many)
            .ContinueMany()
            .Select(x => x.ToArray());
        //  
        /// <summary>
        /// Comment token
        /// </summary>
        /// <example>
        /// CharToken.Parse("; this is single-line comment");
        /// </example>
        public virtual Parser<CommentToken> CommentToken =>
            (from comment in new CommentParser(";","{|", "|}", "\n").SingleLineComment
                select new CommentToken(comment))
            .Token()
            .Named("comment token");
        /// <summary>
        /// Single char wrapped in quote character
        /// </summary>
        /// <example>
        /// CharToken.Parse("'1'");
        /// </example>
        public virtual Parser<char> CharToken =>
            (from @char in Wrap(Parse.AnyChar, Parse.Char('\''))
                select @char)
            .Token()
            .Named("char token");
        /// <summary>
        /// Single quote wrapped identifier token
        /// </summary>
        /// <example>
        /// QuoteIdentifierToken.Parse("'test identifier token'");
        /// </example>
        public virtual Parser<string> QuoteIdentifierToken => (   
                from @string in Wrap(Parse.AnyChar.Except(Parse.Char('\'')).Many().Text(), Parse.Char('\''))
                select @string)
            .Token()
            .Named("quote string token");
        /// <summary>
        /// string wrapped in double quote chars
        /// </summary>
        /// <example>
        /// StringToken.Parse("\"str\"") -> "str"
        /// </example>
        public virtual Parser<string> StringToken => (
            from @string in Wrap(Parse.AnyChar.Except(Parse.Char('"')).Many().Text(), Parse.Char('"'))
                select @string)
            .Token().Named("string token");

        /// <summary>
        /// Keyword token
        /// </summary>
        /// <param name="keyword">
        /// keyword string
        /// </param>
        /// <example>
        /// Keyword("var").Parse("var")
        /// </example>
        public virtual Parser<string> Keyword(string keyword) =>
            (from word in Parse.String(keyword).Text() select word)
                .Token().Named($"keyword {keyword} token");
        /// <summary>
        /// Float number token
        /// </summary>
        /// <example>
        /// FloatToken.Parse("12.45") -> "12.45"
        /// </example>
        public virtual Parser<string> FloatToken => 
            (from @string in Parse.Decimal select @string)
            .Token().Named("string token");

        /// <summary>
        /// hex number token
        /// </summary>
        /// <example>
        /// HexToken.Parse("0xDA") -> DA
        /// </example>
        public virtual Parser<string> HexToken =>
            (from zero in Parse.Char('0')
                from x in Parse.Chars("x")
                from number in Parse.Chars("ABCDEF1234567890").Many().Text()
                select number)
            .Token()
            .Named("hex number");

        [Obsolete]
        public virtual Parser<string> IdentifierToken =>
            (from word in Parse.AnyChar.Except(Parse.Char(' ')).Many().Text()
                select word)
            .Token()
            .Named("identifier token");

        #endregion

        #region etc

        protected internal virtual Parser<T> Wrap<T, S>(Parser<T> el, Parser<S> wrapper) =>
            from _1 in wrapper
            from result in el
            from _2 in wrapper
            select result;

        #endregion

        

       

        
       

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
                from cellID in HexToken
                from closeParen in Parse.Char(')')
                select new RefExpression(cellID))
            .Token()
            .WithPosition()
            .Named("ref_token");
        public virtual Parser<ValueExpression> ValueToken =>
            (from refSym in Parse.Char('$')
                from openParen in Parse.Char('(')
                from value in HexToken
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
                from @string in FloatToken
                from closeParen in Parse.Char(')')
                select float.Parse(@string, CultureInfo.InvariantCulture))
            .Token()
            .Named("string_t expression");
        #endregion
        #region Instructuions token
        public virtual Parser<IInputToken> Raw =>
            (from dword in InstructionToken(IID.raw)
                from space1 in Parse.WhiteSpace.Optional()
                from val1 in HexToken
                select new InstructionExpression(new raw(ulong.Parse(val1, NumberStyles.AllowHexSpecifier))))
            .Token()
            .WithPosition()
            .Named("raw expression");

        public virtual Parser<IInputToken> StageN =>
            (from dword in InstructionToken(IID.orb)
                from val1 in RefToken
                select new InstructionExpression(new orb(val1.Cell)))
            .Token()
            .WithPosition()
            .Named("orb expression");

        public virtual Parser<IInputToken> CKFT =>
            (from dword in InstructionToken(IID.ckft)
                from val1 in RefToken
                select new InstructionExpression(new ckft(val1.Cell)))
            .Token()
            .WithPosition()
            .Named("ckft expression");

        public virtual Parser<IInputToken> Dup =>
            (from dword in InstructionToken(IID.dup)
                from val1 in RefToken
                from val2 in RefToken
                select new InstructionExpression(new dup(val1.Cell, val2.Cell)))
            .Token()
            .WithPosition()
            .Named("dup expression");

        public virtual Parser<IInputToken> NValue =>
            (from dword in InstructionToken(IID.val)
                from val1 in CastFloat
                select new InstructionExpression(new val(val1)))
            .Token()
            .WithPosition()
            .Named("val expression");


        public virtual Parser<IInputToken> LoadI =>
            (from dword in InstructionToken(IID.ldi)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from pipe in PipeRight
                from val1 in ValueToken
                select new InstructionExpression(new ldi(cell1.Cell, val1.Value)))
            .Token()
            .WithPosition()
            .Named("ldi expression");
        public virtual Parser<IInputToken> LoadI_X =>
            (from dword in InstructionToken(IID.ldx)
                from cell1 in RefToken
                from pipe in PipeRight
                from val1 in ValueToken
                select new InstructionExpression(new ldx(cell1.Cell, val1.Value)))
            .Token()
            .WithPosition()
            .Named("ldx expression");

        public virtual Parser<IInputToken> LoadI_S =>
            (from dword in InstructionToken(IID.pull)
                from cell1 in RefToken
                select new InstructionExpression(new pull(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("pull expression");
        public virtual Parser<IInputToken> JumpT =>
            (from dword in InstructionToken(IID.jump_t)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                select new InstructionExpression(new jump_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_t expression");

        public virtual Parser<IInputToken> JumpAt(IID id) =>
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
            (from dword in InstructionToken(IID.swap)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from space2 in Parse.WhiteSpace.Optional()
                from cell2 in RefToken
                select new InstructionExpression(new swap(cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("swap expression");
        public virtual Parser<IInputToken> RFD =>
            (from dword in InstructionToken(IID.rfd)
                from cell1 in RefToken
                from cell2 in RefToken
                select new InstructionExpression(new rfd(cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("mva expression");
        public virtual Parser<IInputToken> PushA =>
            (from dword in InstructionToken(IID.mva)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeRight
                from cell3 in ValueToken.Or(CastCharToken.Select(x => new ValueExpression($"{x:x}")))
             select new InstructionExpression(new mva(cell1.Cell, cell2.Cell, cell3.Value)))
            .Token()
            .WithPosition()
            .Named("mva expression");
        public virtual Parser<IInputToken> PushD =>
            (from dword in InstructionToken(IID.mvd)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new mvd(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("mvd expression");
        public virtual Parser<IInputToken> PushX =>
            (from dword in InstructionToken(IID.mvx)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new mvx(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("mvx expression");
        public virtual Parser<IInputToken> RefT => (
                from dword in InstructionToken(IID.ref_t)
                from cell1 in RefToken
                select new InstructionExpression(new ref_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("ref_t expression");

        public virtual Parser<IInputToken> MathInstruction(IID id) => (
                from dword in InstructionToken(id)
                from cell0 in RefToken
                from cell1 in RefToken
                from cell2 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell, cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        public virtual Parser<IInputToken> AdvMathInstruction(IID id) => (
                from dword in InstructionToken(id)
                from cell0 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");
        public virtual Parser<IInputToken> Adv2MathInstruction(IID id) => (
                from dword in InstructionToken(id)
                from cell0 in RefToken
                from cell1 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell, cell1.Cell)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        public virtual Parser<IInputToken> SqrtToken => (
                from dword in InstructionToken(IID.sqrt)
                from cell0 in RefToken
                from cell1 in RefToken
                select new InstructionExpression(Instruction.Summon(IID.sqrt, cell0.Cell, cell1.Cell)))
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
        public virtual Parser<string> InstructionToken(IID instruction) =>
            (from dot in Parse.Char('.')
                from ident in Parse.String(instruction.ToString()).Text()
                select ident).Token().Named($"{instruction} expression");
        public virtual Parser<InstructionExpression> ByIIDToken(IID id) =>
            (from dword in InstructionToken(id)
                select new InstructionExpression(Instruction.Summon(id)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        #endregion
    }
    
    public class CommentToken : IInputToken
    {
        public readonly string _comment;
        public Position InputPosition { get; set; }
        public CommentToken(string comment) => _comment = comment;
    }
}