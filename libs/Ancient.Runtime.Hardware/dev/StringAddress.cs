namespace ancient.runtime.hardware
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class StringAddress : Attribute
    {
        public readonly short StartAddress;
        public readonly short ContentAddress;

        public StringAddress(short startAddress, short contentAddress)
        {
            StartAddress = startAddress;
            ContentAddress = contentAddress;
        }
    }
}