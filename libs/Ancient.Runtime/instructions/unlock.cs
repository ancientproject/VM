namespace ancient.runtime
{
    using System;
    using System.Text;
    using emit.@unsafe;

    public class unlock : Instruction
    {
        private readonly byte _handle;
        private readonly string _type;

        public unlock(byte handle, string type) : base(IID.unlock)
        {
            _handle = handle;
            _type = type;
            if(_type.Length > 3)
                throw new InvalidOperationException($"Type len cannot more 3 symbols.");
        }

        protected override void OnCompile()
        {
            var bytes = Encoding.ASCII.GetBytes(_type);
            var (r1, r2) = new d8u(_handle);
            var (r3, u1) = new d8u(bytes[0]);
            var (u2, x1) = new d8u(bytes[1]);
            var (x2, x3) = new d8u(bytes[2]);
            Construct(r1, r2, r3, u1, u2, x1, x2, x3);
        }
    }
}
