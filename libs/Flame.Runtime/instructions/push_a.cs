namespace flame.runtime
{
    using System.Linq;
    using System.Text;
    using exceptions;

    public class push_a : Instruction
    {
        internal readonly byte _addressBus;
        internal readonly byte _addressDev;
        internal readonly ushort _value;

        public push_a(byte addressBus, byte addressDev, ushort value) : base(InsID.push_a)
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

    public class push_d : Instruction
    {
        private readonly byte _addressBus;
        private readonly byte _addressDev;
        private readonly byte _cellIndex;

        public push_d(byte addressBus, byte addressDev, byte cellIndex) : base(InsID.push_d)
        {
            _addressBus = addressBus;
            _addressDev = addressDev;
            _cellIndex = cellIndex;
        }
        protected override void OnCompile() 
            => SetRegisters(_addressBus, _addressDev, 0xE, _cellIndex, x2: 0xE);
    }
    public class push_x_debug : Instruction
    {
        private readonly byte _addressBus;
        private readonly byte _addressDev;
        private readonly byte _cellIndex;

        public push_x_debug(byte addressBus, byte addressDev, byte cellIndex) : base(InsID.push_d)
        {
            _addressBus = addressBus;
            _addressDev = addressDev;
            _cellIndex = cellIndex;
        }

        protected override void OnCompile() 
            => SetRegisters(_addressBus, _addressDev, 0xE, _cellIndex, x2: 0xF);
    }
    public static class StringEx
    {
        public static push_a[] Cast_f<p>(this string s, byte addrBus, byte addrDev)
        {
            var err_bit = s.Select(x => new { len = Encoding.UTF8.GetBytes($"{x}").Length, x}).Where(x => x.len > 2).ToArray();

            if(err_bit.Any())
                throw new InvalidCharsException($"Chars: '{string.Join(",", err_bit.Select(x => x.x))}' more than two bytes.");
            return s.Select(x => new push_a(addrBus, addrDev, x)).ToArray();
        }

        public static p[] Cast_t<p>(this push_a[] s) where p : struct => s.Select(x => (p) (object) (uint)x.Assembly()).ToArray();
    }
}