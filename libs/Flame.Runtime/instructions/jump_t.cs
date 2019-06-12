namespace flame.runtime
{
    public class jump_t : Instruction
    {
        internal readonly byte _cell;

        public jump_t(byte cell) : base(InsID.jump_t) => _cell = cell;

        protected override void OnCompile() => SetRegisters(_cell);
    }
}