namespace ancient.runtime
{
    using emit.@unsafe;

    public class ixor : Instruction
    {
        private readonly byte _cell1;
        private readonly byte _cell2;

        public ixor(byte cell1, byte cell2) : base(IID.ixor)
        {
            _cell1 = cell1;
            _cell2 = cell2;
        }

        #region Overrides of Instruction

        protected override void OnCompile()
        {
            var (r1, r2) = new d8u(_cell1);
            var (u1, u2) = new d8u(_cell2);
            Construct(r1, r2, 0, u1, u2);
        }

        #endregion
    }
    public class ior : Instruction
    {
        private readonly byte _cell1;
        private readonly byte _cell2;

        public ior(byte cell1, byte cell2) : base(IID.ior)
        {
            _cell1 = cell1;
            _cell2 = cell2;
        }

        #region Overrides of Instruction

        protected override void OnCompile()
        {
            var (r1, r2) = new d8u(_cell1);
            var (u1, u2) = new d8u(_cell2);
            Construct(r1, r2, 0, u1, u2);
        }

        #endregion
    }
}
