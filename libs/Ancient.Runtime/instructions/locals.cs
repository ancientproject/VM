namespace ancient.runtime
{
    // ReSharper disable ConditionIsAlwaysTrueOrFalse
    using System;
    using System.Linq;
    using emit.sys;
    using emit.@unsafe;
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 0x38|01|0000   0x00|00|0000 0x00|75|33|32|00|00|00|00
    ///   |  |         |     |      |    |  |  |
    ///   |  len       |   index    |    u  3  2
    ///  opcode      segment      typeof
    /// </remarks>
    public class locals : Instruction
    {
        private readonly byte _len;
        public locals(byte len) : base(IID.locals) => _len = len;

        protected override void OnCompile()
        {
            var (r1, r2) = new d8u(_len);
            Construct(r1, r2);
        }

        public static void Pin(in object[] host, out object[] pruned) => pruned = host.Select(x => x switch
        {
            u16_Type _ => default(ushort),
            u32_Type _ => default(uint),
            u8_Type  _ => default(sbyte),
            u2_Type  _ => default(bool),
            u64_Type _ => default(ulong),
            f64_Type _ => default(float),
            i8_Type  _ => default(byte),
            i16_Type _ => default(short),
            i32_Type _ => default(int),
            i64_Type _ => default(long),
            _          => new object(),
        }).ToArray();
    }
}