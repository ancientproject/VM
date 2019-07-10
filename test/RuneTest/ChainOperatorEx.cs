namespace Tests
{
    using System;
    using System.Collections.Generic;

    internal static class ChainOperatorEx
    {
        public static N As<N>(this object s) 
            => (N) s;
        public static T Return<T>(this ChainOperator<T> chain) 
            => chain.data[chain.index].As<T>();
        public static R Return<T, R>(this ChainOperator<T> chain, Func<T, R> actor) 
            => actor(chain.data[chain.index].As<T>());
        public static ChainOperator<T> Chain<T, S>(this List<S> o, T t, Action<List<S>, T> actor) 
            => new ChainOperator<T>(t).Pipe(x => actor(o, t));
        public static ChainOperator<T> And<T>(this ChainOperator<T> chain, Action<T> actor) 
            => chain.Pipe(actor);
        public static ChainOperator<N> Mutate<T, N>(this ChainOperator<T> chain, Func<T, N> actor) 
            => new ChainOperator<N>(chain, actor(chain.Return()));
    }
}