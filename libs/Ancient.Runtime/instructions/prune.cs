namespace ancient.runtime
{
    public class prune : Instruction
    {
        public prune() : base(IID.prune) {}
        protected override void OnCompile()
        {
            Construct(0x1, 0x7, 0xD, 0x2, 0x6, 0xF, 0xF);
        }
    }
}