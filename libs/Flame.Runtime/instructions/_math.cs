namespace flame.runtime
{
    public class sum : _math
    {
        public sum(short cell, short ref1, short ref2) : base(cell, ref1, ref2, InsID.add) { }
    }
    public class sub : _math
    {
        public sub(short cell, short ref1, short ref2) : base(cell, ref1, ref2, InsID.sub) { }
    }
    public class div : _math
    {
        public div(short cell, short ref1, short ref2) : base(cell, ref1, ref2, InsID.div) { }
    }
    public class mul : _math
    {
        public mul(short cell, short ref1, short ref2) : base(cell, ref1, ref2, InsID.mul) { }
    }

    public class pow : _math
    {
        public pow(short cell, short ref1, short ref2) : base(cell, ref1, ref2, InsID.pow) { }
    }
    public class sqrt : Instruction
    {
        private readonly short _cell;
        private readonly short _ref1;

        public sqrt(short cell, short ref1) : base(InsID.sqrt)
        {
            _cell = cell;
            _ref1 = ref1;
        }

        public override ulong Assembly()
            => (ulong)((OPCode << 24) | (_cell << 20) | (_ref1 << 16) | (0 << 12) | (0 << 8) | (0xA << 4) | 0);
    }

    public abstract class _math : Instruction
    {
        private readonly short _cell;
        private readonly short _ref1;
        private readonly short _ref2;

        protected _math(short cell, short ref1, short ref2, InsID id) : base(id)
        {
            _cell = cell;
            _ref1 = ref1;
            _ref2 = ref2;
        }

        public override ulong Assembly() 
            => (ulong)((OPCode << 24) | (_cell << 20) | (_ref1 << 16) | (_ref2 << 12) | (0 << 8) | (0 << 4) | 0);
    }
}