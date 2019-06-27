namespace ancient.compiler.tokens
{
    using Sprache;

    public class ClassicEvolve : IEvolveToken
    {
        public Position InputPosition { get; set; }
        public string[] Result { get; set; }
    }
}