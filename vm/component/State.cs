namespace vm.component
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using flame.runtime;
    using static System.Console;
    public unsafe class State
    {
        private readonly Bus _bus;

        public State(Bus bus) => _bus = bus;

        public ulong pc { get; set; } = 0;

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
        public ushort x1;
        /// <summary>
        /// id
        /// </summary>
        public ushort instructionID;
        /// <summary>
        /// last <see cref="r1"/>
        /// </summary>
        public ulong prev;

        public ulong[] regs = new ulong[16];
        public sbyte halt { get; set; } = 0;

        public List<uint> program { get; set; }

        public void Load(uint[] prog) => program = prog.ToList();

        public uint Fetch() => program.ElementAt((int)pc++);

        public string pX = "";
        public void Eval()
        {
            if (instructionID == 0xA)
            {
                WriteLine($"r r r u u x");
                WriteLine($"1 2 3 1 2 1");
                WriteLine($"{r1:X} {r2:X} {r3:X} {u1:X} {u2:X} {x1:X}");
            }

            switch (instructionID)
            {
                case 0x1:
                    if (u2 != 0)
                        regs[r1] = (ulong)((u1 << 4) | u2);
                    else
                        regs[r1] = u1;
                    break;
                case 0x2:
                    regs[r1] = regs[r2] + regs[r3];
                    break;
                case 0x3:
                    regs[r1] ^= regs[r2];
                    regs[r2] = regs[r1] ^ regs[r2];
                    regs[r1] ^= regs[r2];
                    break;
                case 0x4:
                    regs[r1] = regs[r2] - regs[r3];
                    break;
                case 0x5:
                    regs[r1] = regs[r2] * regs[r3];
                    break;
                case 0x6:
                    regs[r1] = regs[r2] / regs[r3];
                    break;
                case 0x7:
                    regs[r1] = (uint)Math.Pow(regs[r2], regs[r3]);
                    break;
                case 0x8 when u2 == 0xF: // 0x8F000F0
                    pc = regs[r1];
                    break;
                case 0x8 when u2 == 0xC: // 0x8F000C0
                    regs[r1] = pc;
                    break;
                case 0x0:
                case 0xD when r1 == 0xE && r2 == 0xA && r3 == 0xD:
                    halt = 1;
                    break;
                case 0xA: break;
                case 0xF when x1 == 0xC && r3 == 0xE: // push_a
                    _bus.Find(r1 & 0xFF).write(r2 & 0xFF, (u1 << 4 | u2) & 0xFFFFFFF);
                    break;
                case 0xF when x1 == 0xE && r3 == 0x0: // push_d
                    _bus.Find(r1 & 0xFF).write(r2 & 0xFF, (int)regs[u1]);
                    break;
                case 0xF when x1 == 0xF && r3 == 0x0: // push_x_debug
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
            }
            prev = r1;
        }
        public void Accept(ulong mem)
        {
            instructionID = (ushort)((mem & 0xF000000) >> 24);
            r1 = (ushort)((mem & 0xF00000) >> 20);
            r2 = (ushort)((mem & 0xF0000) >> 16);
            r3 = (ushort)((mem & 0xF000) >> 12);
            u1 = (ushort)((mem & 0xF00) >> 8);
            u2 = (ushort)((mem & 0xF0) >> 4);
            x1 = (ushort)(mem & 0xF);
        }
    }
}