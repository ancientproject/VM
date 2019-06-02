namespace flame.compiler.tokens
{
    public class RefExpression : OperatorToken
    {
        public readonly short Cell;

        public RefExpression(short cell) : base(OperatorKind.Ref) => Cell = cell;
    }
}