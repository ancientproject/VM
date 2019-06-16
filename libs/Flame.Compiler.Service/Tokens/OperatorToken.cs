namespace flame.compiler.tokens
{
    using Sprache;

    public class OperatorToken : IInputToken
    {
        public OperatorToken(OperatorKind kind) => Kind = kind;
        public OperatorKind Kind { get; set; }
        public Position InputPosition { get; set; }
    }
}