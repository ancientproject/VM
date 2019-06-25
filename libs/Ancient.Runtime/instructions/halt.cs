namespace ancient.runtime
{
    public class halt : Instruction
    {
        public halt() : base(InsID.halt) { }

        protected override void OnCompile() 
            => SetRegisters(0xE, 0xA, 0xD);

    }
}