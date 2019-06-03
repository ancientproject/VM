namespace flame.runtime
{
    public class swap : Instruction
    {
        internal readonly short _index1;
        internal readonly short _index2;

        public swap(short index1, short index2) : base(InsID.swap)
        {
            _index1 = index1;
            _index2 = index2;
        }

        public override ulong Assembly()
            => (ulong)((OPCode << 24) | (_index1 << 20) | (_index2 << 16) | (0 << 12) | (0 << 8) | (0 << 4) | 0);
    }
}