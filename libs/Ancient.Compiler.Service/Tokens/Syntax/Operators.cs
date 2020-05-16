namespace ancient.compiler.tokens
{
    using System.Globalization;
    using Sprache;

    public partial class AssemblerSyntax
    {
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

        public virtual Parser<string> SignatureToken =>
            (from refSym in Parse.String("!{")
             from sign in Parse.AnyChar.Except(Parse.Char('}')).Many().Text()
             from closeParen in Parse.Char('}')
             select sign)
            .Token()
            .Named("signature expression");

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
    }
}