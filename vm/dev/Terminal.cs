namespace vm.dev
{
    using System;
    using System.Threading.Tasks;
    using Internal;
    using static System.Console;
    public class Terminal : IDevice
    {
        [StringAddress(0xE, 0xF)]
        public string Name { get; } = "Term228";

        public Terminal(short startAddr) => StartAddress = startAddr;

        public short StartAddress { get; }

        [PropAddress(0x0)]
        public ConsoleColor foregroundColor { get; protected set; } = ConsoleColor.White;
        [PropAddress(0x1)]
        public ConsoleColor backgroundColor { get; protected set; } = ConsoleColor.Black;

        private string relMemory;

        [ActionAddress(0x5)]
        public void WriteChar(char c) => Write(c);
        [ActionAddress(0x6)]
        public void StageChar(char c) => relMemory += c;
        [ActionAddress(0x7)]
        public void PushRel() => Write(relMemory);
        [ActionAddress(0x8)]
        public void ClearRel() => relMemory = "";

        public void write(int address, int data) => (this as IDevice).WriteMemory(address, data);

        public int read(int address) => throw new NotImplementedException();

        [ActionAddress(0x3)]
        public void Init() => ClearRel();

        [ActionAddress(0x4)]
        public void Shutdown() => relMemory = null;
    }
}