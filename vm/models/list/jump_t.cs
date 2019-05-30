namespace vm.models.list
{
    public class jump_t : Instruction
    {
        private readonly ushort _cell;

        public jump_t(ushort cell) : base(InsID.jump_t, 0x8) => _cell = cell;

        public override ulong Assembly()
            => (ulong)((OPCode << 24) | (_cell << 20) | (0 << 16) | (0 << 12) | (0 << 8) | (0xF << 4) | 0);
    }
}