namespace ancient.runtime
{
    public class raw : Instruction
    {
        private readonly ulong _value;

        public raw(ulong value) : base(IID.raw) => _value = value;

        protected override void OnCompile()
        {

        }

        public override long Assembly() => (long) _value;
    }
}