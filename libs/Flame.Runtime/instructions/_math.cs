namespace flame.runtime
{
    public class sum : _math
    {
        public sum(byte cell, byte ref1, byte ref2) : base(cell, ref1, ref2, InsID.add) { }
    }
    public class sub : _math
    {
        public sub(byte cell, byte ref1, byte ref2) : base(cell, ref1, ref2, InsID.sub) { }
    }
    public class div : _math
    {
        public div(byte cell, byte ref1, byte ref2) : base(cell, ref1, ref2, InsID.div) { }
    }
    public class mul : _math
    {
        public mul(byte cell, byte ref1, byte ref2) : base(cell, ref1, ref2, InsID.mul) { }
    }

    public class pow : _math
    {
        public pow(byte cell, byte ref1, byte ref2) : base(cell, ref1, ref2, InsID.pow) { }
    }
    public class sqrt : Instruction
    {
        private readonly byte _cell;
        private readonly byte _ref1;

        public sqrt(byte cell, byte ref1) : base(InsID.sqrt)
        {
            _cell = cell;
            _ref1 = ref1;
        }
        protected override void OnCompile() 
            => SetRegisters(_cell, _ref1);
    }

    public abstract class _math : Instruction
    {
        private readonly byte _cell;
        private readonly byte _ref1;
        private readonly byte _ref2;

        protected _math(byte cell, byte ref1, byte ref2, InsID id) : base(id)
        {
            _cell = cell;
            _ref1 = ref1;
            _ref2 = ref2;
        }
        protected override void OnCompile() 
            => SetRegisters(_cell, _ref1, _ref2);
    }
}