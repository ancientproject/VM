namespace vm_test
{
    using System.Collections.Generic;
    using ancient.runtime;
    using ancient.runtime.hardware;

    public class TestDevice : Device
    {
        public TestDevice() : base(0x1, "<???>") { }

        public Stack<char> stack = new Stack<char>();

        [ActionAddress(0x5)]
        public void Call(char t) => stack.Push(t);
    }
}