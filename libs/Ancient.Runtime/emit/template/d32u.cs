namespace ancient.runtime.emit.@unsafe
{
    public class d32u : UnsafeDeconstruct<int>
    {
        public d32u(int value) : base(value){}

        public void Deconstruct(out byte n1, out byte n2, out byte n3, out byte n4)
        {
            n1 = (byte)((_value & 0xF000) >> shift());
            n2 = (byte)((_value & 0x0F00) >> shift());
            n3 = (byte)((_value & 0x00F0) >> shift());
            n4 = (byte)((_value & 0x000F) >> shift());
        }
    }
}