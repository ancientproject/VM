namespace ancient.runtime
{
    using System.Linq;
    using System.Text;
    using exceptions;

    public class mva : Instruction
    {
        internal readonly byte _addressBus;
        internal readonly byte _addressDev;
        internal readonly ushort _value;

        public mva(byte addressBus, byte addressDev, ushort value) : base(IID.mva)
        {
            _addressBus = addressBus;
            _addressDev = addressDev;
            _value = value;
        }


        protected override void OnCompile()
        {
            var u1 = (byte)((_value & 0xF000) >> 12);
            var u2 = (byte)((_value & 0xF00 ) >> 8);
            var u3 = (byte)((_value & 0xF0  ) >> 4);
            var u4 = (byte)((_value & 0xF   ) >> 0);

            SetRegisters(_addressBus,_addressDev, u1, u2, u3, u4, 0xC);
        }
    }
}