namespace ancient.runtime
{
    public class ldi : Instruction
    {
        internal readonly byte _index;
        internal readonly ushort _value;

        public ldi(byte index, ushort value) : base(IID.ldi)
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
}