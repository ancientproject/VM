namespace ancient.runtime
{
    using emit.@unsafe;

    public class wtd: Instruction
    {
        private readonly byte _addressDev;
        private readonly byte _addressField;
        private readonly byte _data;

        public wtd(byte addressDev, byte addressField, byte data) : base(IID.wtd)
        {
            _addressDev = addressDev;
            _addressField = addressField;
            _data = data;
        }

        protected override void OnCompile()
        {
            var (r1, r2) = new d8u(_addressDev);
            var (u1, u2) = new d8u(_addressField);
            var (x1, x2) = new d8u(_data);
            Construct(r1, r2, u1: u1, u2: u2, x1: x1, x2: x2);
        }
    }
}