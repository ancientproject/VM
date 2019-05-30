namespace vm.models
{
    public abstract class Instruction
    {
        public InsID ID { get; }
        public short OPCode { get; }

        protected Instruction(InsID id, short opCode)
        {
            ID = id;
            OPCode = opCode;
        }

        public abstract ulong Assembly();

        public static implicit operator ulong(Instruction i) => i.Assembly();
        public static implicit operator uint(Instruction i) => (uint)i.Assembly();
    }
}