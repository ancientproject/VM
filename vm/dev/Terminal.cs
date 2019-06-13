namespace vm.dev
{
    using System;
    using System.Threading.Tasks;
    using Internal;
    using static System.Console;
    public class Terminal : IDevice, IComparable
    {
        [StringAddress(0xE, 0xF)]
        public string Name { get; } = "8bit-terminal";

        public Terminal(short startAddr) => StartAddress = startAddr;

        public short StartAddress { get; }

        [PropAddress(0x0)]
        public ConsoleColor foregroundColor { get; protected set; } = ConsoleColor.White;
        [PropAddress(0x1)]
        public ConsoleColor backgroundColor { get; protected set; } = ConsoleColor.Black;

        private string relMemory;

        [ActionAddress(0x5)]
        public void WriteChar(char c)
        {
            if(c == 0xA)
                Write(Environment.NewLine);
            else 
                Write(c);
        }
        [ActionAddress(0x6)]
        public void StageChar(char c)
        {
            if (c == 0xA)
                relMemory += Environment.NewLine;
            else 
                relMemory += c;
        }
        [ActionAddress(0x7)]
        public void PushRel() => Write(relMemory);
        [ActionAddress(0x8)]
        public void ClearRel() => relMemory = "";

        [ActionAddress(0x9)]
        public void Setting(char c)
        {
            var u1 = ((short)c & 0xF0) >> 4;
            var u2 = (short)c & 0xF;

            switch (u1)
            {
                case 0x1:
                    ForegroundColor = (ConsoleColor)u2;
                    break;
                case 0x2:
                    BackgroundColor = (ConsoleColor)u2;
                    break;
                case 0x3:
                    ForegroundColor = ConsoleColor.White;
                    break;
                case 0x4:
                    BackgroundColor = ConsoleColor.Black;
                    break;
            }
        }

        public void write(int address, int data) => (this as IDevice).WriteMemory(address, data);

        public int read(int address) => throw new NotImplementedException();

        [ActionAddress(0x3)]
        public void Init() => ClearRel();

        [ActionAddress(0x4)]
        public void Shutdown() => relMemory = null;

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