namespace flame.runtime
{
    public class Unicast<TIn, TOut> where TIn : struct where TOut : struct
    {
        public static TIn operator &(Unicast<TIn, TOut> s, TOut q) => (TIn)(object)q;
    }
}