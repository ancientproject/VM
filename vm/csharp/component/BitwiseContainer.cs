namespace vm.component
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class BitwiseContainer : IFormattable
    {
        private readonly ulong _value;

        public BitwiseContainer(ulong mem) => _value = mem;

        public static implicit operator BitwiseContainer(ulong value) => new BitwiseContainer(value);
        public static implicit operator BitwiseContainer(long value) => new BitwiseContainer((ulong)value);
        public static implicit operator ulong(BitwiseContainer value) => value._value;

        
        public static ulong operator &(BitwiseContainer _, ulong mask)
        {
            var shift = new BitArray(BitConverter.GetBytes(mask)).Cast<bool>().TakeWhile(bit => !bit).Count();
            return _sh(_._value, mask, shift);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong _sh(ulong value, ulong mask, int bit) => (value & mask) >> bit;

        public override string ToString() => _value.ToString();
        public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);
    }
}