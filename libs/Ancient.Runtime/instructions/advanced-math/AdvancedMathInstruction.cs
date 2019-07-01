namespace ancient.runtime
{
    public abstract class AdvancedMathInstruction : Instruction
    {
        private readonly byte? _r2;
        private readonly byte _r1;

        protected AdvancedMathInstruction(IID id, byte r1, byte? r2 = null) : base(id)
        {
            _r1 = r1;
            _r2 = r2;
        }

        protected override void OnCompile()
        {
            if(_r2 == null)
                SetRegisters(_r1);
            else
                SetRegisters(_r1, _r2.Value);
        }
    }
}