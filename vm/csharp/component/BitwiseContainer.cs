namespace vm.component
{
    using System;
    using System.Collections;
    using System.Linq;

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
            return (_._value & mask) >> shift;
        }

        public override string ToString() => _value.ToString();
        public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);
    }
}