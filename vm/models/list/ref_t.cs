namespace vm.models.list
{
    public class ref_t : Instruction
    {
        private readonly ushort _cell;

        public ref_t(ushort cell) : base(InsID.ref_t, 0x8) => _cell = cell;

        public override ulong Assembly()
            => (ulong)((OPCode << 24) | (_cell << 20) | (0 << 16) | (0 << 12) | (0 << 8) | (0xC << 4) | 0);
    }
}