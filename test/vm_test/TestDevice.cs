namespace vm_test
{
    using System.Collections.Generic;
    using vm.dev;
    using vm.dev.Internal;

    public class TestDevice : AbstractDevice
    {
        public TestDevice() : base(0x1, "<???>") { }

        public Stack<char> stack = new Stack<char>();

        [ActionAddress(0x5)]
        public void Call(char t) => stack.Push(t);
    }
}