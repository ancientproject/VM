namespace vm_test
{
    using flame.runtime;
    using NUnit.Framework;
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
}
