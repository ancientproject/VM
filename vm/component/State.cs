﻿namespace vm.component
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static System.Console;

    public interface ShadowCache<out T> where T : Cache, ICloneable
    {
        T L1 { get; }
        T L2 { get; }

        void Reflect();
    }

    public class ShadowCacheFactory : ShadowCache<Cache>
    {
        public Cache L1 { get; } = new Cache();
        public Cache L2 { get; private set; } = new Cache();

        public void Reflect() => L2 = L1.Clone() as Cache;

        public static ShadowCache<Cache> Create() => new ShadowCacheFactory();
    }

    public class Cache : ICloneable
    {
        /// <summary>
        /// base register cell
        /// </summary>
        public ushort r1, r2, r3;
        /// <summary>
        /// value register cell
        /// </summary>
        public ushort u1, u2;
        /// <summary>
        /// magic cell
        /// </summary>
        public ushort x1, x2;

        /// <summary>
        /// id
        /// </summary>
        public ushort IID;

        public ulong PC { get; set; }

        public object Clone() => new Cache 
            { r1 = r1, r2 = r2, r3 = r3, u1 = u1, u2 = u2, x1 = x1, x2 = x2, IID = IID, PC = PC };
    }

    public unsafe class State
    {
        private readonly Bus _bus;

        public State(Bus bus) => _bus = bus;

        public ulong pc
        {
            get => Registers.L1.PC;
            set => Registers.L1.PC = value;
        }

        public ShadowCache<Cache> Registers = ShadowCacheFactory.Create();

        /// <summary>
        /// base register cell
        /// </summary>
        public ushort r1
        {
            get => Registers.L1.r1;
            set => Registers.L1.r1 = value;
        }
        public ushort r2
        {
            get => Registers.L1.r2;
            set => Registers.L1.r2 = value;
        }
        public ushort r3
        {
            get => Registers.L1.r3;
            set => Registers.L1.r3 = value;
        }
        /// <summary>
        /// value register cell
        /// </summary>
        public ushort u1
        {
            get => Registers.L1.u1;
            set => Registers.L1.u1 = value;
        }
        public ushort u2
        {
            get => Registers.L1.u2;
            set => Registers.L1.u2 = value;
        }
        /// <summary>
        /// magic cell
        /// </summary>
        public ushort x1
        {
            get => Registers.L1.x1;
            set => Registers.L1.x1 = value;
        }
        public ushort x2
        {
            get => Registers.L1.x2;
            set => Registers.L1.x2 = value;
        }
        
        /// <summary>
        /// id
        /// </summary>
        public ushort instructionID
        {
            get => Registers.L1.IID;
            set => Registers.L1.IID = value;
        }
        /// <summary>
        /// trace flag
        /// </summary>
        public bool tc = false;
        /// <summary>
        /// Error flag
        /// </summary>
        public bool ec = true;



        public ulong[] regs = new ulong[16];

        public sbyte halt { get; set; } = 0;

        public List<uint> program { get; set; } = new List<uint>();

        public void Load(params uint[] prog) => program.AddRange(prog);

        public uint Fetch()
        {
            if (program.Count == (int) pc && halt == 0)
            {
                Array.Fill(regs, (ulong)0xDEAD);
                Load(0xFFFFFFFF);
            }
            return program.ElementAt((int)pc++);
        }

        public string pX = "";
        public void Eval()
        {
            if (instructionID == 0xA)
            {
                WriteLine($"  r   r   r   u   u   x   x");
                WriteLine($"  1   2   3   1   2   1   2");
                WriteLine($"0x{r1:X} 0x{r2:X} 0x{r3:X} 0x{u1:X} 0x{u2:X} 0x{x1:X} 0x{x2:X}");
            }
            Trace($"0x{r1:X} 0x{r2:X} 0x{r3:X} 0x{u1:X} 0x{u2:X} 0x{x1:X} 0x{x2:X}");
            switch (instructionID)
            {
                case 0xF when r1 == 0xF && r2 == 0xF && r3 == 0xF && u1 == 0xF && u2 == 0xF && x1 == 0xF:
                    _bus.Cpu.Halt(0xF);
                    break;
                case 0x1:
                    Trace($"loadi 0x{u1:X}, 0x{u2:X}");
                    if (u2 != 0)
                        regs[r1] = (ulong)((u1 << 4) | u2);
                    else
                        regs[r1] = u1;
                    break;
                case 0x2:
                    Trace($"sum 0x{r2:X}, 0x{r3:X}");
                    regs[r1] = regs[r2] + regs[r3];
                    break;
                case 0x3:
                    Trace($"swap 0x{r1:X}, 0x{r2:X}");
                    regs[r1] ^= regs[r2];
                    regs[r2] = regs[r1] ^ regs[r2];
                    regs[r1] ^= regs[r2];
                    break;
                case 0x4:
                    Trace($"sub 0x{r2:X}, 0x{r3:X}");
                    regs[r1] = regs[r2] - regs[r3];
                    break;
                case 0x5:
                    Trace($"mul 0x{r2:X}, 0x{r3:X}");
                    regs[r1] = regs[r2] * regs[r3];
                    break;
                case 0x6:
                    Trace($"div 0x{r2:X}, 0x{r3:X}");
                    if (regs[r3] == 0)
                    {
                        _bus.Cpu.Halt(0xC);
                        break;
                    }
                    regs[r1] = regs[r2] / regs[r3];
                    break;
                case 0x7 when u2 == 0x0:
                    Trace($"pow 0x{r2:X}, 0x{r3:X}");
                    regs[r1] = (uint)Math.Pow(regs[r2], regs[r3]);
                    break;
                case 0x7 when u2 == 0xA:
                    Trace($"sqrt 0x{r2:X}");
                    regs[r1] = (uint)Math.Sqrt(regs[r2]);
                    break;
                case 0x8 when u2 == 0xF: // 0x8F000F0
                    Trace($"jump_t 0x{r1:X}");
                    pc = regs[r1];
                    break;
                case 0x8 when u2 == 0xC: // 0x8F000C0
                    Trace($"ref_t 0x{r1:X}");
                    regs[r1] = pc;
                    break;
                case 0xD when r1 == 0xE && r2 == 0xA && r3 == 0xD:
                    _bus.Cpu.Halt(0x0);
                    break;
                case 0xB when r1 == 0x0 && r2 == 0x0 && r3 == 0xB && u1 == 0x5:
                    _bus.Cpu.Halt(0x1);
                    break;
                case 0xA: break;
                case 0xF when x2 == 0xC: // push_a
                    Trace($"push_a 0x{r1:X} 0x{r2:X} 0x{u1:X} 0x{u2:X}");
                    _bus.Find(r1 & 0xFF).write(r2 & 0xFF, (r3 << 12 | u1 << 8 | u2 << 4 | x1) & 0xFFFFFFF);
                    break;
                case 0xF when x2 == 0xE: // push_d
                    Trace($"push_d 0x{r1:X} 0x{r2:X} 0x{u1:X}");
                    _bus.Find(r1 & 0xFF).write(r2 & 0xFF, (int)regs[u1]);
                    break;
                case 0xF when x2 == 0xF: // push_x
                    Trace($"push_x 0x{r1:X} 0x{r2:X} 0x{u1:X}");
                    var x = regs[u1].ToString();
                    short[] cast(string str)
                    {
                        var list = new List<int>();
                        foreach (var c in str)
                        {
                            var uu1 = (c & 0xF0) >> 4;
                            var uu2 = (c & 0xF);
                            list.Add((uu1 << 4 | uu2) & 0xFFFFFFF);
                        }
                        return list.Select(x => (short)x).ToArray();
                    }
                    foreach (var uuu in cast(x))
                        _bus.Find(r1 & 0xFF).write(r2 & 0xFF, uuu);
                    break;
                default:
                    Error(
                        $"Unk OpCode: {instructionID:X2} {Environment.NewLine}0x{r1:X} 0x{r2:X} 0x{r3:X} 0x{u1:X} 0x{u2:X} 0x{x1:X} 0x{x2:X}");
                    break;
            }
            Registers.Reflect();
        }
       
        public void Accept(ulong mem)
        {
            Trace($"fetch 0x{mem:X}");
            instructionID 
                = (ushort)((mem & 0xF0000000) >> 28);
            r1  = (ushort)((mem & 0xF000000 ) >> 24);
            r2  = (ushort)((mem & 0xF00000  ) >> 20);
            r3  = (ushort)((mem & 0xF0000   ) >> 16);
            u1  = (ushort)((mem & 0xF000    ) >> 12);
            u2  = (ushort)((mem & 0xF00     ) >> 8);
            x1  = (ushort)((mem & 0xF0      ) >> 4);
            x2  = (ushort) (mem & 0xF             );
        }

        private void Trace(string str)
        {
            if(tc)
                WriteLine(str);
        }

        private void Error(string str)
        {
            if (ec)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine(str);
                ForegroundColor = ConsoleColor.White;
            }
        }
    }
}