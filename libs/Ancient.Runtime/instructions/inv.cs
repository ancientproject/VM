namespace ancient.runtime
{
    public class inv : Instruction
    {
        private readonly byte _sigId;

        public inv(byte sigID) : base(InsID.inv) => _sigId = sigID;
        protected override void OnCompile() => SetRegisters(r2: _sigId);
    }

    public class sig : Instruction
    {
        private readonly byte _sigId;

        public sig(byte sigID) : base(InsID.sig) => _sigId = sigID;
        protected override void OnCompile() => SetRegisters(r2: _sigId);
    }

    public class ret : Instruction
    {
        public ret() : base(InsID.ret) { }
        protected override void OnCompile() { }
    }
}