namespace flame.runtime
{
    public class swap : Instruction
    {
        internal readonly byte _index1;
        internal readonly byte _index2;

        public swap(byte index1, byte index2) : base(InsID.swap)
        {
            _index1 = index1;
            _index2 = index2;
        }

        protected override void OnCompile() => SetRegisters(_index1, _index2);
    }
}