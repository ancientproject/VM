namespace ancient.compiler.tokens
{
    using runtime;
    using Sprache;

    public class TransformationContext : IInputToken
    {
        public Instruction[] Instructions { get; set; }
        public Position InputPosition { get; set; }
    }
}