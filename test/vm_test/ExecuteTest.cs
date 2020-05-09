namespace vm_test
{
    using ancient.runtime;
    using ancient.runtime.emit.sys;
    using ancient.runtime.hardware;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using vm.component;

    [TestFixture]
    public class ExecuteTest : VMBehaviour
    {

        [OneTimeSetUp]
        public void Setup() => IntConverter.Register<char>();

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("invert operation")]
        public void DifTest()
        {
            var mem = new ulong[]
            {
                new ceq(0x1, 0x0, 0x0),
                new dif_t(0x1, 1), 
                new ldi(0x5, 15), 
                new ldi(0x6, 15), 
            };
            load(mem);
            shot(mem.Length);
            AssertRegister(x => x.mem[0x5], 0ul);
            AssertRegister(x => x.mem[0x6], 15ul);
        }
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("invert operation")]
        public void InvertTest()
        {
            var mem = new ulong[]
            {
                new ldi(0x0, 0x32),
                new inv(0x0),
                new ldx(0x18, 0x1),
                new orb(0x1),
                new val(.14f),
                new pull(0x1),
                new inv(0x1), 
            };
            load(mem);
            shot(mem.Length);
            AssertRegister(x => x.mem[0x0], unchecked((ulong)-0x32));
            AssertRegister(x => x.mem[0x1], State.f32u64 & -.14f);
        }

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("ckft operation")]
        public void TestComplexCalculation()
        {
            var mem = new ulong[]
            {
                // 0x0 - var 200
                // 0x1 - int i
                // 0x2 - double target
                // 0x3 - int -1
                // 0x4 - int temp
                // 0x5 - Math.pow(-1,i+1)
                // 0x6 - (2*i - 1)
                // 0x9 - pi
                // 0xA - ref
                new ldx(0x11, 0x1), // set trace=ON
                new ldx(0x18, 0x1), // enable float-mode
                new ldi(0x0, 200),  // max steps 
                new nop(),
                new orb(1),
                new val(0.0f), // double target = 0;
                new pull(0x2),
                new orb(1),
                new val(-1f),// const float minusOne = -1;
                new pull(0x3),
                // i++
                new ldx(0x18, 0x1),
                new inc(0x1),
                

                // Math.pow(-1, i + 1)
                new dup(0x1, 0x5),
                new inc(0x5),
                new ldx(0x18, 0x1),
                new pow(0x5, 0x3, 0x5),

                // (2 * i - 1)
                new dup(0x1, 0x6),
                new orb(1),
                new val(2.0f), // double target = 0;
                new pull(0x4),
                new mul(0x6, 0x4, 0x6),
                new dec(0x6),

                new div(0x4, 0x5, 0x6),
                new swap(0x4, 0x9), 
                // if result 0x9 cell is not infinity
                new ckft(0x9),
                
            };
            load(mem);
            shot();
            AssertRegister<ulong>(x => x.mem[0x11], 0x1);
            shot();
            AssertRegister<ulong>(x => x.mem[0x18], 0x1);
            shot();
            AssertRegister<ulong>(x => x.mem[0x0], 200);
            shot(3);
            AssertRegister<ulong>(x => x.mem[0x2], State.f32u64 & 0.0f);
            shot(2);
            AssertRegister<ulong>(x => x.mem[0x3], State.f32u64 & -1.0f);
            shot(2);
            AssertRegister<ulong>(x => x.mem[0x1], State.f32u64 & 1f);
            shot();
            AssertRegister<ulong>(x => x.mem[0x1], state.mem[0x5]);
            shot();
            AssertRegister<ulong>(x => x.mem[0x5], State.f32u64 & (State.u64f32 & state.mem[0x1]) + 1f);
            shot(2);
            var t = State.u64f32 & state.mem[0x5];
            AssertRegister<ulong>(x => x.mem[0x5], State.f32u64 & MathF.Pow(-1, 1 + 1));
            shot(); // new dup(0x1, 0x6),
            AssertRegister<ulong>(x => x.mem[0x1], state.mem[0x5]);
            shot(2); // new val(2.0f), new pull(0x4),
            AssertRegister<ulong>(x => x.mem[0x4], State.f32u64 & 2f);
            shot(2); //  new mul(0x6, 0x4, 0x6), new dec(0x6),
            AssertRegister<ulong>(x => x.mem[0x6], State.f32u64 & (2 * 1 - 1));
            shot(); //  new div(0x4, 0x5, 0x6),
            AssertRegister<ulong>(x => x.mem[0x4], State.f32u64 & MathF.Pow(-1, 1 + 1) / (2 * 1 - 1));
        }

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("ckft operation")]
        public void ckftTest()
        {
            static ulong float2raw(float value) =>
                (ulong)BitConverter.ToInt32(BitConverter.GetBytes(value), 0);

            state.ff = true;
            var mem = new ulong[]
            {
                new orb(1),
                new val(1.0f),
                new orb(1),
                new val(float.PositiveInfinity),
                new pull(0x0), // pull PositiveInfinity from stack into 0x0 cell
                new pull(0x1), // pull 1.0f from stack into 0x1 cell

                // validate finity value
                new ckft(0x0), 
                new ckft(0x1),
            };
            load(mem);
            shot(mem.Length);
            AssertRegister(x => x.mem[0x0], float2raw(float.PositiveInfinity));
            AssertRegister(x => x.mem[0x1], float2raw(1.0f));
            Assert.True(IsHalt()); // validate halting with x87 float exception
        }

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("duplicate operation")]
        public void DupTest()
        {
            var mem = new ulong[]
            {
                new ldi(0x0, 0x32),
                new dup(0x0, 0x1), 
            };
            load(mem);
            shot(mem.Length);
            AssertRegister(x => x.mem[0x0], (ulong)0x32);
            AssertRegister(x => x.mem[0x1], (ulong)0x32);
        }

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
                new add(0x6, 0x1, 0x2),
                new sqrt(0x7, 0x1),
                new pow(0x8, 0x1, 0x2), 
            };

            load(mem);

            shot((uint)mem.Length);
            Assert.AreEqual((15.14f - 5.52f), State.u64f32 & state.mem[0x3]);
            Assert.AreEqual(15.14f * 5.52f, State.u64f32 & state.mem[0x4]);
            Assert.AreEqual(15.14f / 5.52f, State.u64f32 & state.mem[0x5]);
            Assert.AreEqual(15.14f + 5.52f, State.u64f32 & state.mem[0x6]);
            Assert.AreEqual(MathF.Sqrt(15.14f), State.u64f32 & state.mem[0x7]);
            Assert.AreEqual(MathF.Pow(15.14f, 5.52f), State.u64f32 & state.mem[0x8]);
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
            AssertRegister(x => x.mem[0x1], (ulong)0x5);
            AssertRegister(x => x.mem[0x2], (ulong)0x4);
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
            shot((uint)mem.Length);
            Assert.AreEqual(8, state.step);
            AssertRegister(x => State.u64f32 & x.mem[0x3], 3f);
        }
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("float load test")]
        public void OrbTest()
        {
            state.southFlag = false;
            bios.virtual_stack = false;
            var mem = new ulong[] {
                new ldx(0x11, 0x1), 
                // load to stack next 2 values
                new orb(0x1),
                // value
                new val(3.14f), 
                // load from stack to 0x2 cell memory
                new pull(0x2)
            };
            load(mem);
            shot((uint)mem.Length);
            AssertRegister(x => State.u64f32 & x.mem[0x2], 3.14f);
        }

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("divide test")]
        public void DivTest()
        {
            state.southFlag = false;
            bios.virtual_stack = false;
            state.ff = false;
            var mem = new ulong[] {
                new ldi(0x0, 0x1), 
                new ldi(0x1, 0x5), 
                new div(0x3, 0x1, 0x0),
            };
            load(mem);
            shot((ushort)mem.Length);
            AssertRegister(x => x.mem[0x3], (ulong)0x5);
        }

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("float divide test")]
        public void DivFloatTest()
        {
            state.southFlag = false;
            bios.virtual_stack = false;
            state.ff = true;
            var mem = new ulong[] {
                new orb(0x2),
                new val(1.0f),
                new val(5.4f), 
                new pull(0x0), 
                new pull(0x1),
                new div(0x2, 0x0, 0x1),
            };
            load(mem);
            shot((uint)mem.Length);
            AssertRegister(x => State.i64f32 & (State.i64 & x.mem[0x2]), 5.4f / 1f);
        }
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("float divide test")]
        public void DivFloatTestInvert()
        {
            state.southFlag = false;
            bios.virtual_stack = false;
            state.ff = false;
            var mem = new ulong[] {
                new orb(0x2),
                new val(1.0f),
                new val(5.4f), 
                new pull(0x0), 
                new pull(0x1), 
                new ldx(0x18, 0x1), 
                new div(0x2, 0x1, 0x0)
            };
            load(mem);
            shot((uint)mem.Length);
            AssertRegister(x => State.i64f32 & (State.i64 & x.mem[0x2]),  1f / 5.4f);
        }

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("call extern function test")]
        public void CallInnerTest()
        {
            var mem = new ulong[]
            {
                new call_i(Module.CompositeIndex("assert->value()")),
            };
            load(mem);
            shot();
            Assert.AreEqual(0xDDD, bus.State.stack.pop());
        }


        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("str load test")]
        public void lpStrTest()
        {
            var mem = new Instruction[]
            {
                new lpstr("test_value1"), 
                new lpstr("test_value2"), 
                new lpstr("test_value3"), 
            };
            state.LoadMeta(mem.SelectMany(x => x.GetMetaDataILBytes()).ToArray());
            load(mem.Select(x => (ulong)x).ToArray());
        }
    }
}
