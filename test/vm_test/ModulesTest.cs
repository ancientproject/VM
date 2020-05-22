namespace vm_test
{
    using System.Linq;
    using ancient.runtime;
    using ancient.runtime.emit;
    using ancient.runtime.emit.sys;
    using ancient.runtime.hardware;
    using NUnit.Framework;

    [TestFixture]
    public class ModulesTest : VMBehaviour
    {
        [OneTimeSetUp]
        public void Setup() => IntConverter.Register<char>();


        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("parse functions in module class test")]
        public void ParseFunctionsTest()
        {
            Module.Boot(bus);
            var mem = new Instruction[]
            {
                new ldx(0x11, 0x1),
                new sig("test1", 0, "void"), 
                new lpstr("test1"),
                new ldi(0x0, 0x5),
                new ret(),
                new sig("test2", 0, "void"), 
                new lpstr("test2"),
                new ldi(0x1, 0x6),
                new call_i("test1()"),
                new ret(),
                new sig("test3", 0, "void"), new lpstr("test3"),
                new call_i("test2()"),
                new mul(0x3, 0x0, 0x1),
                new ret(),
                new call_i("test3()"),
                new nop(),
            }.Reverse().ToArray();
            
            var assembly = new DynamicAssembly("test");
            var ilGen = assembly.GetGenerator();
            ilGen.Emit(mem.Select(x => (OpCode)x).ToArray());
            var module = new Module("test.module");
            Module.modules.Add(module.GetHashCode(), module);
            state.LoadMeta(mem.Reverse().SelectMany(x => x.GetMetaDataILBytes()).ToArray());

            var functions = Module.ImportFunctions(assembly.GetILCode(), module);

            Assert.AreEqual(3, functions.Length);
        }
    }
}