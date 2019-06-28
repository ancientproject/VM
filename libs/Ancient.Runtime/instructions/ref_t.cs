namespace ancient.runtime
{
    public class ref_t : Instruction
    {
        internal readonly byte _cell;

        public ref_t(byte cell) : base(IID.ref_t) => _cell = cell;

        protected override void OnCompile() => SetRegisters(_cell, u2: 0xC);
    }
}