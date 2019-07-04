namespace ancient.runtime.emit.@unsafe
{
    public class d16u : UnsafeDeconstruct<short>
    {
        public d16u(short value) : base(value) {}

        public void Deconstruct(out byte n1, out byte n2)
        {
            n1 = (byte)((_value & 0x00F0) >> shift());
            n2 = (byte)((_value & 0x000F) >> shift());
        }
    }
}