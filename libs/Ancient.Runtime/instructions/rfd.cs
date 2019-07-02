namespace ancient.runtime
{
    public class rfd : Instruction
    {
        private readonly byte _addressDev;
        private readonly byte _addressField;

        public rfd(byte addressDev, byte addressField) : base(IID.rfd)
        {
            _addressDev = addressDev;
            _addressField = addressField;
        }
        protected override void OnCompile() 
            => SetRegisters(_addressDev, _addressField);
    }
}