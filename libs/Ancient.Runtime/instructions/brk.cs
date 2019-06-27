namespace ancient.runtime
{
    public enum BreakType : byte
    {
        Now   = 1,
        Next  = 2,
        After = 3
    }

    public class brk_s : brk
    {
        public brk_s() : base(BreakType.Now, InsID.brk_s) { }
    }
    public class brk_n : brk
    {
        public brk_n() : base(BreakType.Next, InsID.brk_n) { }
    }
    public class brk_a : brk
    {
        public brk_a() : base(BreakType.After, InsID.brk_a) { }
    }
    public abstract class brk : Instruction
    {
        private readonly BreakType _type;
        protected brk(BreakType type, InsID id) : base(id) => _type = type;
        protected override void OnCompile() => SetRegisters(x2: (byte)_type);
    }
}