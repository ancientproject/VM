namespace ancient.runtime
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString()}")]
    public class Unicast<TOut, TIn> where TOut : struct where TIn : struct
    {
        public static TOut operator &(Unicast<TOut, TIn> _, TIn q) => q.To<TOut>();
        public static TOut operator &(Unicast<TOut, TIn> _, int q) => q.To<TOut>();

        public override string ToString() => $"static_cast<{typeof(TIn).Name}, {typeof(TOut).Name}>";
    }

    [DebuggerDisplay("{ToString()}")]
    public class Bitcast<TOut, TIn> where TOut : struct where TIn : struct 
    {
        public static TOut operator &(Bitcast<TOut, TIn> _, TIn q)
        {
            if (typeof(TOut) == typeof(long) && typeof(TIn) == typeof(float))
                return (TOut)(object)(long)BitConverter.ToInt32(BitConverter.GetBytes((float) (object) q), 0);
            if (typeof(TOut) == typeof(float) && typeof(TIn) == typeof(long))
                return (TOut)(object)BitConverter.ToSingle(BitConverter.GetBytes((long) (object) q), 0);
            throw new InvalidCastException();
        }
        public override string ToString() => $"bit_cast<{typeof(TIn).Name}, {typeof(TOut).Name}>";
    }

    public static class ObjectCaster
    {
        public static T To<T>(this object @this)
        {
            if (@this == null) 
                return default;
            var type = typeof (T);
            if (@this.GetType() == type)
                return (T) @this;
            var converter1 = TypeDescriptor.GetConverter(@this);
            if (converter1.CanConvertTo(type))
                return (T) converter1.ConvertTo(@this, type);
            var converter2 = TypeDescriptor.GetConverter(type);
            if (converter2.CanConvertFrom(@this.GetType()))
                return (T) converter2.ConvertFrom(@this);
            if (@this == DBNull.Value)
                return default;
            return (T) @this;
        }
    }
}