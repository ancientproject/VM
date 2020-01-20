namespace ancient.runtime
{
    using System;

    public class orb : Instruction
    {
        internal readonly byte _next;

        public orb(byte next) : base(IID.orb)
        {
            if (next >= 16)
                throw new InvalidCastException($"[orb] too many next read instruction");
            _next = next;
        }

        protected override void OnCompile() => Construct(r1: _next);
    }
}