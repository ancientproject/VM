namespace vm.component
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using dev.Internal;
    using flame.runtime;
    using flame.runtime.exceptions;
    using static System.Console;

    public class State
    {
        private readonly Bus bus;

        public State(Bus bus) => this.bus = bus;

        

        

        #region Registers
        public ShadowCache<Cache> Registers = ShadowCacheFactory.Create();
        public ulong pc
        {
            get => Registers.L1.PC;
            set => Registers.L1.PC = value != 0xffff ? value : 0UL;
        }

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
        public ushort iid
        {
            get => Registers.L1.IID;
            set => Registers.L1.IID = value;
        }

        #endregion
        
        /// <summary>
        /// trace flag
        /// </summary>
        public bool tc 
        {
            get => mem[0x11] == 1;
            set => mem[0x11] = value ? 0x1UL : 0x0UL;
        }
        /// <summary>
        /// Error flag
        /// </summary>
        public bool ec
        {
            get => mem[0x12] == 1;
            set => mem[0x12] = value ? 0x1UL : 0x0UL;
        }

        /// <summary>
        /// Keep memory flag
        /// </summary>
        public bool km
        {
            get => mem[0x13] == 1;
            set => mem[0x13] = value ? 0x1UL : 0x0UL;
        }
        /// <summary>
        /// fast write flag
        /// </summary>
        public bool fw
        {
            get => mem[0x14] == 0x0;
            set => mem[0x14] = value ? 0x1UL : 0x0UL;
        }
        /// <summary>
        /// overflow flag
        /// </summary>
        public bool of
        {
            get => mem[0x15] == 1;
            set => mem[0x15] = value ? 0x1UL : 0x0UL;
        }

        public uint curAddr { get; set; } = 0xFFFF;
        public uint lastAddr { get; set; } = 0xFFFF;

        public ulong[] mem = new ulong[32];

        public sbyte halt { get; set; } = 0;

        public List<uint> program { get; set; } = new List<uint>();

        public void Load(params uint[] prog) => program.AddRange(prog);

        public uint Fetch()
        {
            using var watcher = new StopwatchOperation("fetch operation");
            try
            {
                lastAddr = curAddr;
                if (program.Count != (int) pc || halt != 0) 
                    return (curAddr = program.ElementAt((int) pc++));
                return (curAddr = program.ElementAt((int) pc++));
            }
            catch
            {
                if (!km)
                {
                    Array.Fill(mem, 0xDEADUL, 0, 16);
                    Load(0xFFFFFFFF);
                }
                throw new CorruptedMemoryException($"Memory instruction at address 0x{curAddr:X4} access to memory 0x{pc:X4} could not be read.");
            }
        }

        public void Eval()
        {
            using var watcher = new StopwatchOperation("eval operation");
            MemoryManagement.FastWrite = fw;
            if (iid == 0xA)
            {
                trace($"  r   r   r   u   u   x   x");
                trace($"  1   2   3   1   2   1   2");
                trace($"0x{Registers.L2.r1:X} 0x{Registers.L2.r2:X} 0x{Registers.L2.r3:X} 0x{Registers.L2.u1:X} 0x{Registers.L2.u2:X} 0x{x1:X} 0x{Registers.L2.x2:X}");
            }
            trace($"0x{r1:X} 0x{r2:X} 0x{r3:X} 0x{u1:X} 0x{u2:X} 0x{x1:X} 0x{x2:X}");
            switch (iid)
            {

                #region halt
                case 0xF when r1 == 0xF && r2 == 0xF && r3 == 0xF && u1 == 0xF && u2 == 0xF && x1 == 0xF:
                    bus.cpu.halt(0xF);
                    break;
                case 0xD when r1 == 0xE && r2 == 0xA && r3 == 0xD:
                    bus.cpu.halt(0x0);
                    break;
                case 0xB when r1 == 0x0 && r2 == 0x0 && r3 == 0xB && u1 == 0x5:
                    bus.cpu.halt(0x1);
                    break;
                #endregion
                
                case 0x1 when x2 == 0x0:
                    trace($"loadi 0x{u1:X}, 0x{u2:X} -> 0x{r1:X}");
                    _ = u2 switch
                    {
                        0x0 => mem[r1] = u1,
                        _ => mem[r1] = u64 & ((u1 << 4) | u2)
                    };
                    break;
                case 0x1 when x2 == 0xA:
                    trace($"loadi_x 0x{u1:X}, 0x{u2:X} -> 0x{r1:X}-0x{r2:X}");
                    mem[((r1 << 4) | r2)] = (ulong)((u1 << 4) | u2);
                    break;
                case 0x2:
                    trace($"sum 0x{r2:X}, 0x{r3:X}");
                    mem[r1] = mem[r2] + mem[r3];
                    break;
                case 0x4:
                    trace($".sub 0x{r2:X}, 0x{r3:X}");
                    mem[r1] = mem[r2] - mem[r3];
                    break;
                case 0x5:
                    trace($".mul 0x{r2:X}, 0x{r3:X}");
                    mem[r1] = mem[r2] * mem[r3];
                    break;
                case 0x6: // 0x6123000
                    trace($".div 0x{r2:X}, 0x{r3:X}");
                    auto <<= mem[r3] switch {
                        0x0 => bus.cpu.halt(0xC),
                        _   => mem[r1] = mem[r2] / mem[r3]
                    };
                    break;
                case 0x3: // 0x3120000
                    trace($".swap 0x{r1:X}, 0x{r2:X}");
                    mem[r1] ^= mem[r2];
                    mem[r2] =  mem[r1] ^ mem[r2];
                    mem[r1] ^= mem[r2];
                    break;
                case 0xF when x2 == 0xE: // 0xF9988E0
                    trace($".mv_u4 0x{r1:X} -> 0x{r2:X} -> 0x{u1:X}");
                    bus.Find(r1 & 0xFF).write(r2 & 0xFF, (int)mem[u1] & 0xFF);
                    break;
                case 0xF when x2 == 0xC: // 0xF00000C
                    trace($"mv_u8 0x{r1:X} -> 0x{r2:X} -> [0x{u1:X}-0x{u2:X}]");
                    bus.Find(r1 & 0xFF).write(r2 & 0xFF, (r3 << 12 | u1 << 8 | u2 << 4 | x1) & 0xFFFFFFF);
                    break;
                case 0xD when x2 == 0x3 && of && fw:
                    trace($".ncall 0x{r1:X} 0x{r2:X} 0x{u1:X}");
                    bus.Find(u1 & 0xFF).write(r3 & 0xF0, 0xFF);
                    Array.Fill(mem, 0xFUL, 0, 16);
                    break;
                case 0x8 when u2 == 0xF && x1 == 0x0: // 0x8F000F00
                    trace($"jump_t 0x{r1:X}");
                    pc = mem[r1];
                    break;
                case 0x8 when u2 == 0xF && x1 == 0x1: // 0x8FCD0F10
                    trace(mem[r2] >= mem[r3]
                        ? $"jump_e 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> apl"
                        : $"jump_e 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> skip");
                    if(mem[r2] >= mem[r3]) 
                        pc = mem[r1];
                    break;
                case 0x8 when u2 == 0xF && x1 == 0x2: // 0x8FCD0F20
                    trace(mem[r2] > mem[r3]
                        ? $"jump_g 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> apl"
                        : $"jump_g 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> skip");
                    if(mem[r2] > mem[r3])
                        pc = mem[r1];
                    break;
                case 0x8 when u2 == 0xF && x1 == 0x3: // 0x8FCD0F30
                    trace(mem[r2] < mem[r3]
                        ? $"jump_u 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> apl"
                        : $"jump_u 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> skip");
                    if(mem[r2] < mem[r3])
                        pc = mem[r1];
                    break;
                case 0x8 when u2 == 0xF && x1 == 0x4: // 0x8FCD0F40
                    trace(mem[r2] <= mem[r3]
                        ? $"jump_y 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> apl"
                        : $"jump_y 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> skip");
                    if(mem[r2] <= mem[r3]) pc = mem[r1];
                    break;
                case 0xA: break;
                case 0x8 when u2 == 0xC: // 0x8F000C0
                    trace($"ref_t 0x{r1:X}");
                    mem[r1] = pc;
                    break;
                case 0xF when x2 == 0xF: // push_x
                    trace($"push_x 0x{r1:X} 0x{r2:X} 0x{u1:X}");
                    var x = mem[u1].ToString();
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
                        bus.Find(r1 & 0xFF).write(r2 & 0xFF, uuu);
                    break;
                #region legacy
                case 0x7 when u2 == 0xA:
                    trace($"sqrt 0x{r2:X}");
                    mem[r1] = (uint)Math.Sqrt(mem[r2]);
                    break;
                case 0x7 when u2 == 0x0:
                    trace($".pow 0x{r2:X}, 0x{r3:X}");
                    mem[r1] = (uint)Math.Pow(mem[r2], mem[r3]);
                    break;
                #endregion
                default:
                    Error(
                        $"Unk OpCode: {iid:X2} {Environment.NewLine}0x{r1:X} 0x{r2:X} 0x{r3:X} 0x{u1:X} 0x{u2:X} 0x{x1:X} 0x{x2:X}");
                    break;
            }
            Registers.Reflect();
        }
       
        public void Accept(BitwiseContainer mem)
        {
            trace($"fetch 0x{mem:X}");
            var 
            pfx = u16 & (mem & 0xF00000000);
            iid = u16 & (mem & 0x0F0000000);
            r1  = u16 & (mem & 0x00F000000);
            r2  = u16 & (mem & 0x000F00000);
            r3  = u16 & (mem & 0x0000F0000);
            u1  = u16 & (mem & 0x00000F000);
            u2  = u16 & (mem & 0x000000F00);
            x1  = u16 & (mem & 0x0000000F0);
            x2  = u16 & (mem & 0x00000000F);
            iid = u16 & (pfx << 0x4 | iid );
        }

        

        public static Unicast<byte, ulong> u8 = new Unicast<byte, ulong>();
        public static Unicast<ushort, ulong> u16 = new Unicast<ushort, ulong>();
        public static Unicast<uint, ulong> u32 = new Unicast<uint, ulong>();
        public static Unicast<ulong, ulong> u64 = new Unicast<ulong, ulong>();

        

        private void trace(string str)
        {
            if(tc)
                WriteLine(str);
            OnTrace?.Invoke(str);
        }

        private void Error(string str)
        {
            OnError?.Invoke(str);
            if (ec)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine(str);
                ForegroundColor = ConsoleColor.White;
            }
        }

        public event Action<string> OnTrace;
        public event Action<string> OnError;
    }

    public class BitwiseContainer : IFormattable
    {
        private readonly ulong _value;

        public BitwiseContainer(ulong mem) => _value = mem;

        public static implicit operator BitwiseContainer(ulong value) => new BitwiseContainer(value);
        public static implicit operator ulong(BitwiseContainer value) => value._value;
        
        public static ulong operator &(BitwiseContainer _, ulong mask)
        {
            var shift = new BitArray(BitConverter.GetBytes(mask)).Cast<bool>().TakeWhile(bit => !bit).Count();
            return (_._value & mask) >> shift;
        }

        public override string ToString() => _value.ToString();
        public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);
    }
}