namespace vm
{
    using System;
    using System.Threading.Tasks;
    using static System.Console;
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Title = "cpu_host";

            var core = new Core();

            core.stack.Load(new []{ 0x1064, 0x11C8, 0x2201, 0x0000 });

            while (core.stack.halt == 0)
            {
                await core.Step();
                await Task.Delay(400);
            }

            ReadKey();
        }
    }

    public class Core
    {
        public Stack stack { get; } = new Stack();

        public void Eval()
        {
            switch (stack.instructionID)
            {
                case 0:
                    WriteLine("HALT");
                    stack.halt = 1;
                    break;
                case 1:
                    WriteLine($"loadi 0x{stack.r4:X} 0x{stack.r4:X}");
                    stack.regs[stack.r1] = stack.r4;
                    break;
                case 2:
                    WriteLine($"add 0x{stack.r1:X} 0x{stack.r2:X} 0x{stack.r3:X}");
                    stack.regs[stack.r1] = stack.regs[stack.r2] + stack.regs[stack.r3];
                    break;
            }
        }

        public async Task Step()
        {
            var nuget = stack.Fetch();
            stack.Accept(Convert.ToUInt32(nuget));
            Eval();
            await Task.CompletedTask;
        }
    }
    public unsafe class Stack
    {
        public ulong pc { get; set; } = 0;

        public uint r1, r2, r3, r4;
        public uint instructionID;

        public uint[] regs = new uint[4];
        public sbyte halt { get; set; } = 0;

        public int[] program { get; set; }

        public void Load(int[] prog) => program = prog;

        public int Fetch() => program[pc++];

        public void Accept(uint mem)
        {
            instructionID = (mem & 0xF000) >> 12;
            r1            = (mem & 0xF00 ) >> 8 ;
            r2            = (mem & 0xF0  ) >> 4 ;
            r3            = (mem & 0xF   )      ;
            r4            = (mem & 0xFF  )      ;
        }
    }
}