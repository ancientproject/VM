namespace ancient.compiler.tokens
{
    using System.Collections.Generic;
    using System.Linq;
    using runtime;

    public class PushJEvolve : ClassicEvolve
    {
        private readonly string _value;
        private readonly byte _cellDev;
        private readonly byte _actionDev;

        public PushJEvolve(string value, byte cellDev, byte ActionDev)
        {
            _value = value;
            _cellDev = cellDev;
            _actionDev = ActionDev;
        }

        protected override void OnBuild(List<Instruction> jar) 
            => jar.AddRange(_value.Select(x => new mva(_cellDev, _actionDev, x)));
    }
}