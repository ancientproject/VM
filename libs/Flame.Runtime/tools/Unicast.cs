namespace flame.runtime
{
    using System;
    using System.ComponentModel;

    public class Unicast<TOut, TIn> where TOut : struct where TIn : struct
    {
        public static TOut operator &(Unicast<TOut, TIn> _, TIn q) => q.To<TOut>();



        public static TOut operator &(Unicast<TOut, TIn> _, int q) => q.To<TOut>();
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