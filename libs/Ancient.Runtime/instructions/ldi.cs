namespace ancient.runtime
{
    using emit.@unsafe;

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
            var (u2, u1) = new d16u((short)_value);
            SetRegisters(_index, 0x0, 0x0, u1, u2);
        }
    }
}