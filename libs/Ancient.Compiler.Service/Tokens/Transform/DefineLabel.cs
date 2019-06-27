namespace ancient.compiler.tokens
{
    using Sprache;

    public class DefineLabel : IEvolveToken
    {
        public string Name { get; set; }
        public string Hex { get; set; }
        public Position InputPosition { get; set; }

        public DefineLabel(string name, string hex)
        {
            Name = name;
            Hex = hex;
        }
    }
}