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

    public static class StringEx
    {
        public static mva[] Cast_f<p>(this string s, byte addrBus, byte addrDev)
        {
            var err_bit = s.Select(x => new { len = Encoding.UTF8.GetBytes($"{x}").Length, x}).Where(x => x.len > 2).ToArray();

            if(err_bit.Any())
                throw new InvalidCharsException($"Chars: '{string.Join(",", err_bit.Select(x => x.x))}' more than two bytes.");
            return s.Select(x => new mva(addrBus, addrDev, x)).ToArray();
        }

        public static p[] Cast_t<p>(this mva[] s) where p : struct => s.Select(x => (p) (object) (uint)x.Assembly()).ToArray();
    }
}