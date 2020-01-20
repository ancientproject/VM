namespace ancient.runtime
{
    public class mvd : Instruction
    {
        private readonly byte _addressBus;
        private readonly byte _addressDev;
        private readonly byte _cellIndex;

        public mvd(byte addressBus, byte addressDev, byte cellIndex) : base(IID.mvd)
        {
            _addressBus = addressBus;
            _addressDev = addressDev;
            _cellIndex = cellIndex;
        }

        protected override void OnCompile()
            => Construct(_addressBus, _addressDev, 0xE, _cellIndex, x2: 0xE);
    }
}