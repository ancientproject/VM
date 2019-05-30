namespace vm.models.list
{
    public class loadi : Instruction
    {
        private readonly ushort _index;
        private readonly ushort _value;

        public loadi(ushort index, ushort value) : base(InsID.loadi, 0x1)
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