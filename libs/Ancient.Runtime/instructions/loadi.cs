namespace ancient.runtime
{
    public class loadi : Instruction
    {
        internal readonly byte _index;
        internal readonly ushort _value;

        public loadi(byte index, ushort value) : base(InsID.loadi)
        {
            _index = index;
            _value = value;
        }

        protected override void OnCompile()
        {
            var u2 = (byte)((_value & 0xF0) >> 4);
            var u1 = (byte)(_value & 0x0F);
            SetRegisters(_index, 0x0, 0x0, u1, u2);
        }
    }
    public class loadi_s : Instruction
    {
        internal readonly byte _index;

        public loadi_s(byte index) : base(InsID.loadi_s) => _index = index;
        protected override void OnCompile() => SetRegisters(_index);
    }
    public class loadi_x : Instruction
    {
        internal readonly ushort _index;
        internal readonly byte _x2 = 0xA;
        internal readonly ushort _value;

        public loadi_x(ushort index, ushort value) : base(InsID.loadi_x)
        {
            _index = index;
            _value = value;
        }
        public loadi_x(ushort index, bool value) : base(InsID.loadi_x)
        {
            _index = index;
            _value = (ushort)(value ? 1 : 0);
        }
        public loadi_x(ushort index, ushort value, byte x2) : base(InsID.loadi_x)
        {
            _index = index;
            _value = value;
            _x2 = x2;
        }

        protected override void OnCompile()
        {
            var u1 = (byte)((_value & 0xF0) >> 4);
            var u2 = (byte)(_value & 0xF);

            var r1 = (byte)((_index & 0xF0) >> 4);
            var r2 = (byte)(_index & 0xF);
            SetRegisters(r1, r2, 0x0, u1, u2, x2:_x2);
        }
    }
}