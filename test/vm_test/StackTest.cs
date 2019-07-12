namespace vm_test
{
    using NUnit.Framework;
    using vm.component;

    public class StackTest : VMBehaviour
    {
        public Stack stack;

        [SetUp]
        public void setUp() => stack = new Stack(bus) {__halter = this};

        [Test]
        public void UseVirtualForward()
        {
            // use virtual stack
            state.southFlag = true;
            bios.virtual_stack = true;
            bios.memory_stack_forward = false;
            stack.push(0x1234567890ABCDEF);
            Assert.AreEqual(1, state.SP);
            Assert.AreEqual(0x1234567890ABCDEF, stack.pop());
           
        }
        [Test]
        public void UseMemStackForward()
        {
            state.southFlag = true;
            bios.virtual_stack = false;
            bios.memory_stack_forward = true;
            stack.push(0x1234567890ABCDEF);
            Assert.AreEqual(1, state.SP);
            Assert.AreEqual(0x1234567890ABCDEF, stack.pop());
        }
        [Test]
        public void WithoutVirtualForward()
        {
            // using memory stack forward
            state.southFlag = true;
            bios.virtual_stack = false;
            bios.memory_stack_forward = false;
            stack.push(0x1234567890ABCDEF);
            Assert.AreEqual(8, state.SP);
            Assert.AreEqual(0x1234567890ABCDEF, stack.pop());
        }

    }
}