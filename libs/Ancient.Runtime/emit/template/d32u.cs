namespace ancient.runtime.emit.@unsafe
{
    public class d32u : UnsafeDeconstruct<uint>
    {
        public d32u(uint value) : base(value) { }

        public void Deconstruct(out byte n1, out byte n2, out byte n3, out byte n4, out byte n5, out byte n6, out byte n7, out byte n8)
        {
            n1 = (byte)((_value & 0xF0000000) >> shift());
            n2 = (byte)((_value & 0x0F000000) >> shift());
            n3 = (byte)((_value & 0x00F00000) >> shift());
            n4 = (byte)((_value & 0x000F0000) >> shift());
            n5 = (byte)((_value & 0x0000F000) >> shift());
            n6 = (byte)((_value & 0x00000F00) >> shift());
            n7 = (byte)((_value & 0x000000F0) >> shift());
            n8 = (byte)((_value & 0x0000000F) >> shift());
        }
    }
}