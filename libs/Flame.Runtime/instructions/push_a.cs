namespace flame.runtime
{
    using System.Linq;
    using System.Text;
    using exceptions;

    public class push_a : Instruction
    {
        private readonly short _addressBus;
        private readonly short _addressDev;
        private readonly short _value;

        public push_a(short addressBus, short addressDev, short value) : base(InsID.push_a)
        {
            _addressBus = addressBus;
            _addressDev = addressDev;
            _value = value;
        }

        public override ulong Assembly()
        {
            var u1 = (_value & 0xF0) >> 4;
            var u2 = (_value & 0xF );
            return (ulong)((OPCode << 24) | (_addressBus << 20) | (_addressDev << 16) | (0xE << 12) | (u1 << 8) | (u2 << 4) | 0xC);
        }
    }

    public class push_d : Instruction
    {
        private readonly short _addressBus;
        private readonly short _addressDev;
        private readonly short _cellIndex;

        public push_d(short addressBus, short addressDev, short cellIndex) : base(InsID.push_d)
        {
            _addressBus = addressBus;
            _addressDev = addressDev;
            _cellIndex = cellIndex;
        }

        public override ulong Assembly()
        {
            return (ulong)((OPCode << 24) | (_addressBus << 20) | (_addressDev << 16) | (0xE << 0) | (_cellIndex << 8) | (0x0 << 4) | 0xE);
        }
    }
    public class push_x_debug : Instruction
    {
        private readonly short _addressBus;
        private readonly short _addressDev;
        private readonly short _cellIndex;

        public push_x_debug(short addressBus, short addressDev, short cellIndex) : base(InsID.push_d)
        {
            _addressBus = addressBus;
            _addressDev = addressDev;
            _cellIndex = cellIndex;
        }

        public override ulong Assembly()
        {
            return (ulong)((OPCode << 24) | (_addressBus << 20) | (_addressDev << 16) | (0xE << 0) | (_cellIndex << 8) | (0x0 << 4) | 0xF);
        }
    }
    public static class StringEx
    {
        public static push_a[] Cast_f<p>(this string s, short addrBus, short addrDev)
        {
            var err_bit = s.Select(x => new { len = Encoding.UTF8.GetBytes($"{x}").Length, x}).Where(x => x.len > 2).ToArray();

            if(err_bit.Any())
                throw new InvalidCharsException($"Chars: '{string.Join(",", err_bit.Select(x => x.x))}' more than two bytes.");
            return s.Select(x => new push_a(addrBus, addrDev, (short) x)).ToArray();
        }

        public static p[] Cast_t<p>(this push_a[] s) where p : struct => s.Select(x => (p) (object) (uint)x.Assembly()).ToArray();
    }
}