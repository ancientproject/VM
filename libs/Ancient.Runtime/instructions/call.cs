namespace ancient.runtime
{
    using emit.@unsafe;

    public class call_i : Instruction
    {
        protected internal readonly ushort _sign;
        public call_i(ushort sign) : base(IID.call_i) => _sign = sign;

        protected override void OnCompile()
        {
            var (u1, u2, u3, u4) = new d16u(_sign);
            Construct(0xD, 0x5, u1, u2, u3, u4, 0xC);
        }
    }
}