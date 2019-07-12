namespace vm_test
{
    using System;
    using ancient.runtime;
    using ancient.runtime.hardware;
    using NUnit.Framework;
    using vm.component;

    [TestFixture]
    public class ExecuteTest : VMBehaviour
    {

        [OneTimeSetUp]
        public void Setup() => IntToCharConverter.Register<char>();

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("float math operation")]
        public void FloatMathTest()
        {
            var mem = new ulong[] {
                // load to stack next 2 values
                new orb(0x2),
                // value
                new val(15.14f), 
                // value
                new val(5.52f), 
                // load from stack to 0x2 cell memory
                new pull(0x2),
                // load from stack to 0x2 cell memory
                new pull(0x1),
                // enable float-point-flag  
                new ldx(0x18, 0x1),
                // substract 0x1 cell value & 0x2 cell value, and stage result to 0x3 cell value
                new sub(0x3, 0x1, 0x2),
                new mul(0x4, 0x1, 0x2),
                new div(0x5, 0x1, 0x2),
                new sum(0x6, 0x1, 0x2),
                new sqrt(0x7, 0x1),
                new pow(0x8, 0x1, 0x2), 
            };

            load(mem);

            shot(12 - 2);
            Assert.AreEqual((15.14f - 5.52f), State.i64f32 & state.mem[0x3]);
            Assert.AreEqual(15.14f * 5.52f, State.i64f32 & state.mem[0x4]);
            Assert.AreEqual(15.14f / 5.52f, State.i64f32 & state.mem[0x5]);
            Assert.AreEqual(15.14f + 5.52f, State.i64f32 & state.mem[0x6]);
            Assert.AreEqual(MathF.Sqrt(15.14f), State.i64f32 & state.mem[0x7]);
            Assert.AreEqual(MathF.Pow(15.14f, 5.52f), State.i64f32 & state.mem[0x8]);
        }
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
            load(new mva(0x1, 0x5, '!'));
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
            load(new ldi(0x1, 0x5), new ldi(0x2, 0x4));
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
                new orb(0x2),
                // value
                new val(3.14f), 
                // value
                new val(0.14f), 
                // load from stack to 0x2 cell memory
                new pull(0x2),
                // load from stack to 0x2 cell memory
                new pull(0x1),
                // enable float-point-flag  
                new ldx(0x18, 0x1),
                // substract 0x1 cell value & 0x2 cell value, and stage result to 0x3 cell value
                new sub(0x3, 0x1, 0x2)
            };
            load(mem);
            shot(5);
            Assert.AreEqual(14, state.step);
            AssertRegister(x => State.i64f32 & x.mem[0x3], 3f);
        }
        [Test]
        public void OrbTest()
        {
            state.southFlag = false;
            bios.virtual_stack = false;
            var mem = new ulong[] {
                new warm(),
                new ldx(0x11, 0x1), 
                // load to stack next 2 values
                new orb(0x1),
                // value
                new val(3.14f), 
                // load from stack to 0x2 cell memory
                new pull(0x2)
            };
            load(mem);
            shot(5);
            AssertRegister(x => State.i64f32 & x.mem[0x2], 3.14f);
        }

        [Test]
        public void DivTest()
        {
            state.southFlag = false;
            bios.virtual_stack = false;
            state.ff = false;
            var mem = new ulong[] {
                new warm(),
                new ldi(0x0, 0x1), 
                new ldi(0x1, 0x5), 
                new div(0x3, 0x1, 0x0),
            };
            load(mem);
            shot(5);
            var s = this;
            AssertRegister(x => x.mem[0x3], 0x5);
        }

        [Test]
        public void DivFloatTest()
        {
            state.southFlag = false;
            bios.virtual_stack = false;
            state.ff = false;
            var mem = new ulong[] {
                new warm(),
                new orb(0x2),
                new val(1.0f),
                new val(5.4f), 
                new pull(0x0), 
                new pull(0x1), 
                new ldx(0x18, 0x1), 
                new div(0x2, 0x0, 0x1),
            };
            load(mem);
            shot((uint)mem.Length);
            var s = this;
            AssertRegister(x => State.i64f32 & x.mem[0x2], 5.4f / 1f);
        }
        [Test]
        public void DivFloatTestInvert()
        {
            state.southFlag = false;
            bios.virtual_stack = false;
            state.ff = false;
            var mem = new ulong[] {
                new warm(),
                new orb(0x2),
                new val(1.0f),
                new val(5.4f), 
                new pull(0x0), 
                new pull(0x1), 
                new ldx(0x18, 0x1), 
                new div(0x2, 0x1, 0x0),
            };
            load(mem);
            shot((uint)mem.Length);
            var s = this;
            AssertRegister(x => State.i64f32 & x.mem[0x2],  1f / 5.4f);
        }
    }
}
