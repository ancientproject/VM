namespace flame.runtime
{
    public class jump_t : Instruction
    {
        private readonly short _cell;

        public jump_t(short cell) : base(InsID.jump_t) => _cell = cell;

        public override ulong Assembly()
            => (ulong)((OPCode << 24) | (_cell << 20) | (0 << 16) | (0 << 12) | (0 << 8) | (0xF << 4) | 0);
    }
}