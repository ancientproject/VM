namespace ancient.runtime
{
    public class mvx : Instruction
    {
        private readonly byte _addressBus;
        private readonly byte _addressDev;
        private readonly byte _cellIndex;

        public mvx(byte addressBus, byte addressDev, byte cellIndex) : base(InsID.mvd)
        {
            _addressBus = addressBus;
            _addressDev = addressDev;
            _cellIndex = cellIndex;
        }

        protected override void OnCompile() 
            => SetRegisters(_addressBus, _addressDev, 0xE, _cellIndex, x2: 0xF);
    }
}