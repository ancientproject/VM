namespace ancient.runtime
{
    using System;

    public class stage_n : Instruction
    {
        internal readonly byte _next;

        public stage_n(byte next) : base(InsID.stage_n)
        {
            if(next >= 16)
                throw new InvalidCastException($"[stage_n] too many next read instruction");
            _next = next;
        }

        protected override void OnCompile()
        {
            SetRegisters(r1: _next);
        }
    }
}