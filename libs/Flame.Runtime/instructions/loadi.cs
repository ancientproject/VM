namespace flame.runtime
{
    public class loadi : Instruction
    {
        internal readonly short _index;
        internal readonly short _value;

        public loadi(short index, short value) : base(InsID.loadi)
        {
            _index = index;
            _value = value;
        }

        public override ulong Assembly()
        {
            var u1 = (_value & 0xF0) >> 4;
            var u2 = (_value & 0xF);
            return (ulong)((OPCode << 24) | (_index << 20) | (0 << 16) | (0 << 12) | (u1 << 8) | (u2 << 4) | 0);
        }
    }
}