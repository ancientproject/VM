namespace ancient.runtime
{
    public class inc : Instruction
    {
        private readonly byte _r1;

        public inc(byte r1) : base(IID.inc) => _r1 = r1;

        protected override void OnCompile() => SetRegisters(_r1);
    }
    public class dec : Instruction
    {
        private readonly byte _r1;

        public dec(byte r1) : base(IID.dec) => _r1 = r1;

        protected override void OnCompile() => SetRegisters(_r1);
    }
}