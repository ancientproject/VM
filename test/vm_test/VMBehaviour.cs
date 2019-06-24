namespace vm_test
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Flame.Runtime.tools;
    using MoreLinq;
    using NUnit.Framework;
    using vm.component;

    public abstract class VMBehaviour
    {
        public Bus bus { get; } = new Bus();
        public CPU cpu => bus.cpu;
        public State state => bus.State;
        public TestDevice dev => bus.Find(0x1) as TestDevice;

        protected VMBehaviour() => bus.Add(new TestDevice());

        public void load(params uint[] values) => bus.State.Load(values);

        public bool IsHalt() => state.halt == 1;
        public bool IsFastWrite() => state.fw;
        public bool IsOverflow() => state.of;
        public bool IsNegative() => state.nf;

        public void shot(uint count = 1) => 
            Enumerable.Range(0, (int) count).Pipe(x => cpu.Step().Wait()).ToArray();

        public void AssertRegister<T>(Expression<Func<State, T>> exp, T value) where T : struct, IFormattable
        {
            var name = RuntimeUtilities.GetFieldPath(exp);
            var selector = exp.Compile();
            Assert.AreEqual($"[{name}] 0x{value:X}", $"[{name}] 0x{selector(state):X}");
        }
    }
}