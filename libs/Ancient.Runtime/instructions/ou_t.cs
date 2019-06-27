namespace ancient.runtime
{
    public class ou_t : Instruction
    {
        internal readonly byte _addressBus;
        internal readonly byte _addressDev;
        internal readonly byte _cellValue;

        public ou_t(byte addressBus, byte addressDev, byte cellValue) : base(InsID.ou_t)
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