namespace ancient.runtime.emit.@unsafe
{
    public class d16u : UnsafeDeconstruct<ushort>
    {
        public d16u(ushort value) : base(value){}

        public void Deconstruct(out byte n1, out byte n2, out byte n3, out byte n4)
        {
            n1 = (byte)((_value & 0xF000) >> shift());
            n2 = (byte)((_value & 0x0F00) >> shift());
            n3 = (byte)((_value & 0x00F0) >> shift());
            n4 = (byte)((_value & 0x000F) >> shift());
            resetShifter();
        }
        public (byte n1, byte n2, byte n3, byte n4) Deconstruct()
        {
            var (n1, n2, n3, n4) = this;
            return (n1, n2, n3, n4);
        }
        public d16u Construct(in byte n1, in byte n2, in byte n3, in byte n4)
        {
            resetShifter();
            this._value = (ushort) ((n1 << shift()) | (n2 << shift()) | (n3 << shift()) | (n4 << shift()));
            return this;
        }

        public static implicit operator (byte n1, byte n2, byte n3, byte n4)(d16u u) => u.Deconstruct();
        public static implicit operator d16u((byte n1, byte n2, byte n3, byte n4) u) => new d16u(0).Construct(u.n1, u.n2, u.n3, u.n4);

        public static implicit operator ushort(d16u u) => u.Value;

    }
}