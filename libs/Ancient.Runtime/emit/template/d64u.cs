namespace ancient.runtime.emit.@unsafe
{
    public class d64u : UnsafeDeconstruct<ulong>
    {
        public d64u(ulong value) : base(value) { }

        public void Deconstruct(
            out byte n1, out byte n2, out byte n3, out byte n4, out byte n5, out byte n6, out byte n7, out byte n8,
            out byte c1, out byte c2, out byte c3, out byte c4, out byte c5, out byte c6, out byte c7, out byte c8)
        {
            n1 = (byte)((_value & 0xF000000000000000) >> shift());
            n2 = (byte)((_value & 0x0F00000000000000) >> shift());
            n3 = (byte)((_value & 0x00F0000000000000) >> shift());
            n4 = (byte)((_value & 0x000F000000000000) >> shift());
            n5 = (byte)((_value & 0x0000F00000000000) >> shift());
            n6 = (byte)((_value & 0x00000F0000000000) >> shift());
            n7 = (byte)((_value & 0x000000F000000000) >> shift());
            n8 = (byte)((_value & 0x0000000F00000000) >> shift());
            c1 = (byte)((_value & 0x00000000F0000000) >> shift());
            c2 = (byte)((_value & 0x000000000F000000) >> shift());
            c3 = (byte)((_value & 0x0000000000F00000) >> shift());
            c4 = (byte)((_value & 0x00000000000F0000) >> shift());
            c5 = (byte)((_value & 0x000000000000F000) >> shift());
            c6 = (byte)((_value & 0x0000000000000F00) >> shift());
            c7 = (byte)((_value & 0x00000000000000F0) >> shift());
            c8 = (byte)((_value & 0x000000000000000F) >> shift());
        }
    }
}