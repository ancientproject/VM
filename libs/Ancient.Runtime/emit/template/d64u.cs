namespace ancient.runtime.emit.@unsafe
{
    public class d64u : UnsafeDeconstruct<long>
    {
        public d64u(long value) : base(value) { }

        public void Deconstruct(out byte n1, out byte n2, out byte n3, out byte n4, out byte n5, out byte n6, out byte n7, out byte n8)
        {
            n1 = (byte)((_value & 0xF000) >> shift());
            n2 = (byte)((_value & 0x0F00) >> shift());
            n3 = (byte)((_value & 0x00F0) >> shift());
            n4 = (byte)((_value & 0x000F) >> shift());
            n5 = (byte)((_value & 0x000F) >> shift());
            n6 = (byte)((_value & 0x000F) >> shift());
            n7 = (byte)((_value & 0x000F) >> shift());
            n8 = (byte)((_value & 0x000F) >> shift());
        }
    }
}