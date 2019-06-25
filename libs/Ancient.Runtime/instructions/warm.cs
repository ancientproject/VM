namespace ancient.runtime
{
    public class warm : Instruction
    {
        public warm() : base(InsID.warm) { }

        protected override void OnCompile() 
            => SetRegisters(0xB, 0xC, 0xD, 0xE, 0xF, 0xE);
    }
}