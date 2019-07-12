namespace ancient.runtime
{
    using emit.@unsafe;

    public class ckft : Instruction
    {
        public ckft(byte cell) : base(IID.ckft) => v1 = cell;
        public byte v1 { get; set; }
        protected override void OnCompile()
        {
            var (r1, r2) = new d8u(v1);
            Construct(r1, r2);
        }
    }
}