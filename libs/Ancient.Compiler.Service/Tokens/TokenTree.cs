namespace ancient.compiler.tokens
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IAssemblyFactory
    {
        void Bind<TResult>(IChainSegment<TResult> segment, Action<TResult> complete);
    }

    public class AssemblyFactoryDefault : IAssemblyFactory
    {
        private readonly IReadOnlyCollection<IInputToken> _tokens;
        private readonly ConcurrentQueue<Expression<Action<IReadOnlyCollection<IInputToken>>>> queue = new ConcurrentQueue<Expression<Action<IReadOnlyCollection<IInputToken>>>>();

        public AssemblyFactoryDefault(IReadOnlyCollection<IInputToken> tokens) => _tokens = tokens;

        public void Bind<TResult>(IChainSegment<TResult> segment, Action<TResult> complete) 
            => queue.Enqueue((x) => complete(segment.Transform(x)));

        public void Process()
        {
            foreach (var segment in queue)
                segment.Compile()(_tokens);
        }
    }
    public interface IChainSegment<out TOut>
    {
        TOut Transform(IReadOnlyCollection<IInputToken> tokens);
    }
}