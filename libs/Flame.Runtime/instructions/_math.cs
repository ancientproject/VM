namespace flame.runtime
{
    public class sum : _math
    {
        public sum(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.add) { }
    }
    public class sub : _math
    {
        public sub(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.sub) { }
    }
    public class div : _math
    {
        public div(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.div) { }
    }
    public class mul : _math
    {
        public mul(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.mul) { }
    }

    public class pow : _math
    {
        public pow(ushort cell, ushort ref1, ushort ref2) : base(cell, ref1, ref2, InsID.pow) { }
    }


    public abstract class _math : Instruction
    {
        private readonly ushort _cell;
        private readonly ushort _ref1;
        private readonly ushort _ref2;

        protected _math(ushort cell, ushort ref1, ushort ref2, InsID id) : base(id)
        {
            _cell = cell;
            _ref1 = ref1;
            _ref2 = ref2;
        }

        public override ulong Assembly() 
            => (ulong)((OPCode << 24) | (_cell << 20) | (_ref1 << 16) | (_ref2 << 12) | (0 << 8) | (0 << 4) | 0);
    }
}