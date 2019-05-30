namespace vm.models.list
{
    public class sum : _math
    {
        public sum(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.add, 0x2) { }
    }
    public class sub : _math
    {
        public sub(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.sub, 0x4) { }
    }
    public class div : _math
    {
        public div(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.div, 0x6) { }
    }
    public class mul : _math
    {
        public mul(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.mul, 0x5) { }
    }

    public class pow : _math
    {
        public pow(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.pow, 0x7) { }
    }


    public class _math : Instruction
    {
        private readonly ushort _cell;
        private readonly ushort _ref1;
        private readonly ushort _ref2;

        public _math(ushort cell, ushort ref1, ushort ref2, InsID id, short op) : base(id, op)
        {
            _cell = cell;
            _ref1 = ref1;
            _ref2 = ref2;
        }

        public override ulong Assembly() 
            => (ulong)((OPCode << 24) | (_cell << 20) | (_ref1 << 16) | (_ref2 << 12) | (0 << 8) | (0 << 4) | 0);
    }
}