namespace ancient.runtime.emit.@unsafe
{
    public class d8u : UnsafeDeconstruct<byte>
    {
        public d8u(byte value) : base(value) {}

        public void Deconstruct(out byte n1, out byte n2)
        {
            n1 = (byte)((_value & 0xF0) >> shift());
            n2 = (byte)((_value & 0x0F) >> shift());
        }
    }
}