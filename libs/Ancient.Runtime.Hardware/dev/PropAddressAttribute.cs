namespace ancient.runtime.hardware
{
    using System;
    [AttributeUsage(AttributeTargets.Property)]
    public class PropAddressAttribute : Attribute
    {
        public short Address { get; }
        public PropAddressAttribute(short address) => Address = address;
    }
}