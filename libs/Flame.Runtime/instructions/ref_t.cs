namespace flame.runtime
{
    public class ref_t : Instruction
    {
        private readonly short _cell;

        public ref_t(short cell) : base(InsID.ref_t) => _cell = cell;

        public override ulong Assembly()
            => (ulong)((OPCode << 24) | (_cell << 20) | (0 << 16) | (0 << 12) | (0 << 8) | (0xC << 4) | 0);
    }
}