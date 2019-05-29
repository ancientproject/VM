namespace vm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static System.Console;
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Title = "cpu_host";

            var core = new Core();

            //core.stack.Load(new []
            //{
            //    0x1064, // set &r1
            //    0x11C8, // set &r2
            //    0x2201, // add &r2 &r3 -> r1
            //    0xA001, // print &r1
            //    0xA002, // print &r2
            //    0x3120, // swipe &r1 &r2
            //    0xA001, // print &r1
            //    0xA002, // print &r2
            //    0xDEAD  // halt
            //});
            core.stack.Load(new uint[]
            {
             // 0x rrruux
             // 0x 123121
                0xABCDEFE, // warm up
                0x1000600, // set &r1
                0x1100C00, // set &r2
                0x2021000, // add &r2 &r3 -> r1
                0xF000003, // dump last reg
                0xE000103, // print &r1
                0xE000203, // print &r2
                0x3120000, // swipe &r1 &r2
                0xE000103, // print &r1
                0xE000203, // print &r2
                0xDEAD     // halt
            });

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

        public async Task Step()
        {
            var nuget = stack.Fetch();
            
            var n1 = Convert.ToUInt32(nuget);
            stack.Accept(n1);
            stack.Eval();
            await Task.CompletedTask;
        }
    }
    public unsafe class Stack
    {
        public ulong pc { get; set; } = 0;

        public ulong r1, r2, r3;
        public ulong u1, u2;
        public ulong x1;
        public ulong instructionID;
        public ulong prev;

        public ulong[] regs = new ulong[16];
        public sbyte halt { get; set; } = 0;

        public List<uint> program { get; set; }

        public void Load(uint[] prog) => program = prog.ToList();

        public uint Fetch() => program.ElementAt((int)pc++);


        public void Eval()
        {
            Print();
            switch (instructionID)
            {
                case 0xA:
                    WriteLine($"r r r u u x");
                    WriteLine($"1 2 3 1 2 1");
                    WriteLine($"{r1:X} {r2:X} {r3:X} {u1:X} {u2:X} {x1:X}");
                    return;
                case 0x1:
                    WriteLine($"loadi ::0x{r1:X} &0x{u1:X}");
                    break;
                case 0x2:
                    WriteLine($"add ::0x{r1:X} ::0x{r2:X} ::&0x{r3:X}");
                    break;
                case 0x0:
                case 0xD when r1 == 0xE && r2 == 0xA && r3 == 0xD:
                    WriteLine("HALT");
                    break;
                case 0x3:
                    WriteLine($"swap ::0x{r1:X4} ::0x{r2:X4}");
                    break;
                case 0xF when x1 == 0x1:
                    WriteLine($"dump_l ::0x{prev:X2} -> hex 0x{regs[prev]:X}");
                    break;
                case 0xF when x1 == 0x2:
                    WriteLine($"dump_l ::0x{prev:X2} -> bin 0b{regs[prev]:2}");
                    break;
                case 0xF when x1 == 0x3:
                    WriteLine($"dump_l ::0x{prev:X2} -> dec   {regs[prev]}");
                    break;
                case 0xE when x1 == 0x1:
                    WriteLine($"dump_p ::0x{u1:X2} -> hex 0x{regs[u1]:X}");
                    break;
                case 0xE when x1 == 0x2:
                    WriteLine($"dump_p ::0x{u1:X2} -> bin 0b{regs[u1]:2}");
                    break;
                case 0xE when x1 == 0x3:
                    WriteLine($"dump_p ::0x{u1:X2} -> dec   {regs[u1]}");
                    break;
            }
            
            switch (instructionID)
            {
                case 0x1:
                    regs[r1] = u1;
                    break;
                case 0x2:
                    regs[r1] = regs[r2] + regs[r3];
                    break;
                case 0x0:
                case 0xD when r1 == 0xE && r2 == 0xA && r3 == 0xD:
                    halt = 1;
                    break;
                case 0x3:
                    regs[r1] ^= regs[r2];
                    regs[r2]  = regs[r1] ^ regs[r2];
                    regs[r1] ^= regs[r2];
                    break;
                case 0x4: break;
                case 0xA: break;
            }
            prev = r1;
        }

        public void Print()
        {
            //WriteLine($"*0x{instructionID:X4} :0x{r1:X4} :0x{r2:X4} :0x{r3:X4} &0x{u1:X4} &0x{u2:X4} %0x{x1:X4}");
        }

        public void Accept(ulong mem)
        {
            WriteLine($"fetch page {mem:x8}");
            instructionID = (mem & 0xF000000) >> 24;
            r1            = (mem & 0xF00000 ) >> 20;
            r2            = (mem & 0xF0000  ) >> 16;
            r3            = (mem & 0xF000   ) >> 12;
            u1            = (mem & 0xF00    ) >> 8 ;
            u2            = (mem & 0xF0     ) >> 4 ;
            x1            = (mem & 0xF      )      ;
        }
    }
}