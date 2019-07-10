namespace Tests
{
    using System;
    using System.Collections.Generic;

    internal sealed class ChainOperator<T> : ChainOperator
    {
        public ChainOperator(ChainOperator outerChain, T value)
        {
            this.data = outerChain.data;
            this.index = outerChain.index + 1;
            this.data.Add(value);
        }

        public ChainOperator(T value)
        {
            this.index = 0;
            this.data = new List<object> { value };
        }

        internal ChainOperator<T> Pipe(Action<T> act)
        {
            act(data[index].As<T>());
            return this;
        }
    }

    internal abstract class ChainOperator
    {
        protected internal IList<object> data { get; set; }
        protected internal int index { get; set; }
    }
}