namespace ancient.compiler.tokens
{
    using System.Globalization;

    public class RefExpression : OperatorToken
    {
        public readonly byte Cell;

        public RefExpression(string cell) : base(OperatorKind.Ref)
        {
            if (byte.TryParse(cell, NumberStyles.AllowHexSpecifier,null, out var result))
                Cell = result;
        }
    }
    public class ValueExpression : OperatorToken
    {
        public readonly ushort Value;

        public ValueExpression(string value) : base(OperatorKind.Ref)
        {
            if (ushort.TryParse(value, NumberStyles.AllowHexSpecifier,null, out var result))
                Value = result;
        }
    }
}