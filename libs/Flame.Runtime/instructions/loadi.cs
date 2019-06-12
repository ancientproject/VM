namespace flame.runtime
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
            var u1 = (byte)((_value & 0xF0) >> 4);
            var u2 = (byte)(_value & 0xF);
            SetRegisters(_index, 0x0, 0x0, u1, u2);
        }
    }
}