namespace vm_test
{
    using System;
    using ancient.runtime.@base;
    using ancient.runtime.emit.sys;
    using ancient.runtime.hardware;
    using NUnit.Framework;
    using vm.component;

    [TestFixture]
    public class FunctionsTest : VMBehaviour
    {
        public class FunctionData
        {
            public Utb[] Args { get; set; }
            public string Name { get; set; }
            public ExternType ReturnType { get; set; }

            public VMRef Write(IMemoryRange range)
            {
                var start = (ushort)range.GetFreeAddress();
                var p = start;
                range.writeString(ref p, "t1.module");
                range.writeString(ref p, Name);
                range.write(p++, Args.Length);
                foreach (var arg in Args)
                {
                    range.writeString(ref p, arg.Type.Name.Replace("_Type", ""));
                    range.write(p++, arg.Value);
                }
                range.writeString(ref p, ReturnType.ShortName);

                return new VMRef(start, (ushort)(p - start));
            }
        }

        [OneTimeSetUp]
        public void Setup() => IntConverter.Register<char>();
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("read/write metadata test")]
        public void ReadWriteMetadataTest()
        {
            if(!(bus.find(0x0) is Memory memory))
                throw new Exception($"Device on [0x0] is not Memory table");
            ushort p = 0x900;
            // module
            memory.writeString(ref p, "test.module");
            // name
            memory.writeString(ref p, "func");
            // arg size
            memory.write(p++, 1);
            // arg[0] name
            memory.writeString(ref p, "u32");
            // arg[0] value
            memory.write(p++, 0);
            // return type
            memory.writeString(ref p, "void");

            var function = new Function((new VMRef(0x900, (ushort)(0x900u - p)), 
                new VMRef(0x900, 0)), memory);

            Assert.AreEqual("func", function.Name);
            Assert.AreEqual(1, function.Arguments.Length);
            Assert.True(function.Arguments[0].Is<u32_Type>());
            Assert.AreEqual(typeof(void_Type), function.ReturnType.GetType());
        }
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("memory test")]
        public void MemoryStructureTest()
        {
            var f1 = new FunctionData { Args = new []{ new Utb(typeof(u64_Type), 0) }, Name = "f1", ReturnType = new void_Type() };
            var f2 = new FunctionData { Args = new []{ new Utb(typeof(u32_Type), 0) }, Name = "f2", ReturnType = new void_Type() };
            var f3 = new FunctionData { Args = new []{ new Utb(typeof(u2_Type), 0) }, Name = "f3", ReturnType = new void_Type() };

            if(!(bus.find(0x0) is Memory memory))
                throw new Exception($"Device on [0x0] is not Memory table");

            var f1_ref = f1.Write(memory);
            var f2_ref = f2.Write(memory);
            var f3_ref = f3.Write(memory);


            var f1Result = new Function((f1_ref, default), memory);
            var f2Result = new Function((f2_ref, default), memory);
            var f3Result = new Function((f3_ref, default), memory);


            Assert.AreEqual(f1.Name, f1Result.Name);
            Assert.AreEqual(f2.Name, f2Result.Name);
            Assert.AreEqual(f3.Name, f3Result.Name);

        }
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("free memory test")]
        public void FreeMemoryTest()
        {
            if(!(bus.find(0x0) is Memory memory))
                throw new Exception($"Device on [0x0] is not Memory table");

            memory.write(0x0, 0x0);
            Assert.AreEqual(0x900, memory.GetFreeAddress());
            memory.write(0x500, 0x0);
            Assert.AreEqual(0x900, memory.GetFreeAddress());
            memory.write(0x901, 0x0);
            Assert.AreEqual(0x902, memory.GetFreeAddress());
            memory.write(0x1020, 0x0);
            Assert.AreEqual(0x1021, memory.GetFreeAddress());
        }
    }
}