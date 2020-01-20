namespace ancient.runtime
{
    public class ldx : Instruction
    {
        internal readonly ushort _index;
        internal readonly byte _x2 = 0xA;
        internal readonly ushort _value;

        public ldx(ushort index, ushort value) : base(IID.ldx)
        {
            _index = index;
            _value = value;
        }

        public ldx(ushort index, bool value) : base(IID.ldx)
        {
            _index = index;
            _value = (ushort)(value ? 1 : 0);
        }

        public ldx(ushort index, ushort value, byte x2) : base(IID.ldx)
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
            Construct(r1, r2, 0x0, u1, u2, x2: _x2);
        }
    }
}