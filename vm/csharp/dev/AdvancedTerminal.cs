namespace vm.dev
{
    using System;
    using System.Runtime.InteropServices;
    using Internal;

    public class AdvancedTerminal : AbstractDevice
    {
        public AdvancedTerminal(short startAddr) : base(startAddr, "24bit-terminal") {}

        [ActionAddress(0x5)]
        public void WriteChar(char c) => Console.Out.Write(c);
    }
}