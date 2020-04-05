namespace ancient.runtime
{
    public class @readonly : Instruction
    {
        private readonly byte _cell;

        public @readonly(byte cell) : base(IID.@readonly) 
            => _cell = cell;

        protected override void OnCompile()
            => Construct(_cell);
    }
}