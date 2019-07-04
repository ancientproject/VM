namespace ancient.runtime.emit.@unsafe
{
    using System;

    public unsafe class UnsafeDeconstruct<TValue> where TValue : unmanaged
    {
        protected TValue _value { get; }
        protected Func<int> shift { get; }

        protected UnsafeDeconstruct(TValue value)
        {
            _value = value;
            shift = ShiftFactory.Create((sizeof(TValue) << 0b10)-(sizeof(TValue) % 0b1010 >> 0b1)).Shift;
        }
    }
}