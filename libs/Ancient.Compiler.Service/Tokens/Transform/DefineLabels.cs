namespace ancient.compiler.tokens
{
    using System.Linq;
    using Sprache;

    public class DefineLabels : IEvolveToken
    {
        public DefineLabel[] Labels { get; set; }

        public DefineLabels(IEvolveToken[] labels) => Labels = labels.Cast<DefineLabel>().ToArray();
        public Position InputPosition { get; set; }
    }
}