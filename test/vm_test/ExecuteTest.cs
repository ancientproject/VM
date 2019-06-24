namespace vm_test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using flame.runtime;
    using Flame.Runtime.tools;
    using MoreLinq;
    using NUnit.Framework;
    using vm.component;
    using vm.dev;
    using vm.dev.Internal;

    [TestFixture]
    public class ExecuteTest : VMBehaviour
    {
        [OneTimeSetUp]
        public void Setup() => IntToCharConverter.Register<char>();

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("warm up load and assert registers")]
        public void WarmUpTest()
        {
            load(0xABCDEFE0);
            shot();
            AssertRegister(x => x.iid, 0xA);
            AssertRegister(x => x.r1, 0xB);
            AssertRegister(x => x.x1, 0xE);
        }
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("load instruction to device and assert result")]
        public void WriteToDevTest()
        {
            load(new push_a(0x1, 0x5, '!'));
            shot();
            Assert.AreEqual('!', dev.stack.Pop());
            AssertRegister(x => x.iid, 0xF);
            AssertRegister(x => x.x2, 0xC);
        }
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("load memory and assert it")]
        public void MemoryTest()
        {
            load(new loadi(0x1, 0x5), new loadi(0x2, 0x4));
            shot(2);
            AssertRegister(x => x.mem[0x1], 0x5);
            AssertRegister(x => x.mem[0x2], 0x4);
        }
    }

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

    public class TestDevice : AbstractDevice
    {
        public TestDevice() : base(0x1, "<???>") { }

        public Stack<char> stack = new Stack<char>();

        [ActionAddress(0x5)]
        public void Call(char t) => stack.Push(t);
    }
}
