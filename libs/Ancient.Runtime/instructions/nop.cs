namespace ancient.runtime
{
    public class nop : Instruction
    {
        public nop() : base(IID.nop)  { }
        protected override void OnCompile() { }
    }
}