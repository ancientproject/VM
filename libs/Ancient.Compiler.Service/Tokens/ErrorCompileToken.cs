namespace ancient.compiler.tokens
{
    using Sprache;

    public class ErrorCompileToken : ErrorToken<IInputToken>, IInputToken
    {
        public ErrorCompileToken(IResult<IInputToken> error) : base(error){}
    }
    public class ErrorEvolveToken : ErrorToken<IEvolveToken>, IEvolveToken
    {
        public ErrorEvolveToken(IResult<IEvolveToken> error) : base(error){}
    }

    public abstract class ErrorToken<T>
    {
        public readonly IResult<T> ErrorResult;
        public Position InputPosition { get; set; }

        protected ErrorToken(IResult<T> error) => ErrorResult = error;
        public override string ToString() => ErrorResult.ToString();
    }
}