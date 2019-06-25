namespace vm_test
{
    using ancient.runtime;
    using NUnit.Framework;
    using vm.component;
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
            AssertRegister(x => x.iid, 0xF);
            AssertRegister(x => x.x2, 0xC);
            Assert.AreEqual('!', dev.stack.Pop());
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

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("float testing")]
        public void FloatTest()
        {
            var mem = new ulong[] {
                // load to stack next 2 values
                new stage_n(0x2),
                // value
                new n_value(3.14f), 
                // value
                new n_value(0.14f), 
                // load from stack to 0x2 cell memory
                new loadi_s(0x2),
                // load from stack to 0x2 cell memory
                new loadi_s(0x1),
                // enable float-point-flag  
                new loadi_x(0x18, 0x1),
                // substract 0x1 cell value & 0x2 cell value, and stage result to 0x3 cell value
                new sub(0x3, 0x1, 0x2)
            };
            load(mem);
            shot(7 - 2);
            AssertRegister(x => State.i64f32 & x.mem[0x3], 3f);
        }
    }
}
