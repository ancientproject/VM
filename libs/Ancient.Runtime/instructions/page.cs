namespace ancient.runtime
{
    using emit.@unsafe;

    public class page : Instruction
    {
        private readonly int _point;

        public page(int point) : base(IID.page) => _point = point;

        protected override void OnCompile()
        {
            var (n1, n2, n3, n4) = new d32u(_point);
            SetRegisters(n1, n2, n3, n4, 0, 0xE, 0xF);
        }
    }
}