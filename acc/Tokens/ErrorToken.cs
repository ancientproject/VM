namespace flame.compiler.tokens
{
    using Sprache;

    public class ErrorToken : IInputToken
    {
        public readonly IResult<IInputToken> ErrorResult;
        public Position InputPosition { get; set; }

        public ErrorToken(IResult<IInputToken> error) => ErrorResult = error;
    }
}