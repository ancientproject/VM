namespace flame.compiler.tokens
{
    using Sprache;

    public interface IInputToken
    {
        Position InputPosition { get; set; }
    }
}