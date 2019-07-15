namespace ancient.runtime
{
    using emit.@unsafe;

    public class unlock : Instruction
    {
        private readonly ushort _handle;

        public unlock(ushort handle) => _handle = handle;

        protected override void OnCompile()
        {
            var (r1, r2, r3, u1) = new d16u(_handle);
            Construct(r1, r2, r3, u1);
        }
    }
}