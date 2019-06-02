namespace flame.runtime
{
    public class halt : Instruction
    {
        public halt() : base(InsID.halt) { }
        public override ulong Assembly()
            => (ulong)((OPCode << 24) | (0xE << 20) | (0xA << 16) | (0xD << 12) | (0 << 8) | (0 << 4) | 0);
    }
}