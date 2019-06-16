namespace flame.compiler.tokens
{
    public class RefExpression : OperatorToken
    {
        public readonly byte Cell;

        public RefExpression(byte cell) : base(OperatorKind.Ref) => Cell = cell;
    }
    public class ValueExpression : OperatorToken
    {
        public readonly ushort Value;

        public ValueExpression(ushort value) : base(OperatorKind.Ref) => Value = value;
    }
}