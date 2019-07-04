namespace vm.dev
{
    using System;
    using ancient.runtime;
    using ancient.runtime.hardware;

    public class AdvancedTerminal : Device
    {
        public AdvancedTerminal(short startAddr) : base(startAddr, "24bit-terminal") {}

        [ActionAddress(0x5)]
        public void WriteChar(char c) => Console.Out.Write(c);
    }
}