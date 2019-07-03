namespace vm.dev
{
    using System;
    using System.Text;
    using Internal;
    using static System.Console;
    public class Terminal : AbstractDevice
    {
        public Terminal(short addr) : base(addr, "8bit-terminal") {}


        [PropAddress(0x0)]
        public ConsoleColor foregroundColor { get; protected set; } = ConsoleColor.White;
        [PropAddress(0x1)]
        public ConsoleColor backgroundColor { get; protected set; } = ConsoleColor.Black;

        private StringBuilder relMemory = new StringBuilder();

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
                relMemory.Append(Environment.NewLine);
            else 
                relMemory.Append(c);
        }
        [ActionAddress(0x7)]
        public void PushRel() => Out.Write(relMemory);
        [ActionAddress(0x8)]
        public void ClearRel() => relMemory.Clear();

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

        [ActionAddress(0x3)]
        public override void WarmUp() => ClearRel();

        [ActionAddress(0x4)]
        public override void Shutdown() => relMemory = null;
    }
}