namespace ancient.runtime
{
    using emit.@unsafe;

    public class page : Instruction
    {
        private readonly ushort _point;

        public page(ushort point) : base(IID.page) => _point = point;

        protected override void OnCompile()
        {
            var (n1, n2, n3, n4) = new d16u(_point);
            SetRegisters(n1, n2, n3, n4, 0, 0xE, 0xF);
        }
    }
}