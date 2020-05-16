namespace ancient.compiler.tokens
{
    using System.Collections.Generic;
    using runtime;
    using Sprache;

    public abstract class ClassicEvolve : IEvolveToken, IEvolveEvent
    {
        public Position InputPosition { get; set; }
        private readonly List<Instruction> cache = new List<Instruction>();


        void IEvolveEvent.OnBuild()
        {
            cache.Clear();
            OnBuild(cache);
        }

        protected abstract void OnBuild(List<Instruction> jar);

        public IReadOnlyCollection<Instruction> GetInstructions() => cache.AsReadOnly();
    }
}