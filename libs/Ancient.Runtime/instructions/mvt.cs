namespace ancient.runtime
{
    public class mvt : Instruction
    {
        internal readonly byte _addressBus;
        internal readonly byte _addressDev;
        internal readonly byte _cellValue;

        public mvt(byte addressBus, byte addressDev, byte cellValue) : base(IID.mvt)
        {
            _addressBus = addressBus;
            _addressDev = addressDev;
            _cellValue = cellValue;
        }

        protected override void OnCompile()
        {
            SetRegisters(_addressBus, _addressDev, 0, _cellValue);
        }
    }
}