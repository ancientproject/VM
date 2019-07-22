namespace ancient.runtime
{
    using System.Linq;
    using System.Text;
    using emit.@unsafe;
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
            var (u1, u2, u3, u4) = new d16u(_value);

            Construct(_addressBus,_addressDev, u1, u2, u3, u4, 0xC);
        }
    }
}