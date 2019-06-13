namespace vm.dev
{
    using System;
    using System.Runtime.InteropServices;
    using Internal;

    public class AdvancedTerminal : IDevice, IComparable
    {
        [StringAddress(0xE, 0xF)] 
        public string Name => "24bit-terminal";

        public AdvancedTerminal(short startAddr) => StartAddress = startAddr;

        public short StartAddress { get; }

        public void write(int address, int data) => (this as IDevice).WriteMemory(address, data);

        public int read(int address) => throw new NotImplementedException();
        
        [ActionAddress(0x5)]
        public void WriteChar(char c) => Console.Write(c);

        public void Init()
        {
        }

        public void Shutdown()
        {
        }

        public override int GetHashCode() => StartAddress ^ 42;
        public int CompareTo(object obj)
        {
            if (obj is IDevice dev)
            {
                if (StartAddress > dev.StartAddress)
                    return 1;
                return -1;
            }
            return 0;
        }

    }
}