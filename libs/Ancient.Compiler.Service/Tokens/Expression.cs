namespace ancient.compiler.tokens
{
    public class Expression { }

    internal class IdentifierExpression : Expression
    {
        public string Identifier { get; }

        public IdentifierExpression(string identifier) => Identifier = identifier;

        public override string ToString() => $"{Identifier}";
    }
}