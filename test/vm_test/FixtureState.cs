namespace vm_test
{
    using ancient.runtime.@base;
    using ancient.runtime.emit.sys;

    public class FixtureState : IState
    {
        public long SP { get; set; }
        public ulong pc { get; set; }
        public ushort r1 { get; set; }
        public ushort r2 { get; set; }
        public ushort r3 { get; set; }
        public ushort u1 { get; set; }
        public ushort u2 { get; set; }
        public ushort x1 { get; set; }
        public ushort x2 { get; set; }
        public ushort x3 { get; set; }
        public ushort x4 { get; set; }
        public ushort o1 { get; set; }
        public ushort o2 { get; set; }
        public ushort o3 { get; set; }
        public ushort iid { get; set; }
        public bool tc { get; set; }
        public bool ec { get; set; }
        public bool km { get; set; }
        public bool fw { get; set; }
        public bool of { get; set; }
        public bool nf { get; set; }
        public bool bf { get; set; }
        public bool ff { get; set; }
        public bool sf { get; set; }
        public bool northFlag { get; set; }
        public bool eastFlag { get; set; }
        public bool southFlag { get; set; }
        public ulong curAddr { get; set; }
        public ulong lastAddr { get; set; }
        public ulong step { get; set; }
        public sbyte halt { get; set; }
        public ulong[] mem { get; } = new ulong[64];
        public ExternType[] mem_types { get; } = new ExternType[64];
    }
}