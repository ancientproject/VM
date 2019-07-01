namespace vm.component
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using dev.Internal;
    using ancient.runtime;
    using ancient.runtime.exceptions;
    using static System.Console;
    using static System.MathF;

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
            set => mem[0x11] = value ? 0x1L : 0x0L;
        }
        /// <summary>
        /// Error flag
        /// </summary>
        public bool ec
        {
            get => mem[0x12] == 1;
            set => mem[0x12] = value ? 0x1L : 0x0L;
        }

        /// <summary>
        /// Keep memory flag
        /// </summary>
        public bool km
        {
            get => mem[0x13] == 1;
            set => mem[0x13] = value ? 0x1L : 0x0L;
        }
        /// <summary>
        /// fast write flag
        /// </summary>
        public bool fw
        {
            get => mem[0x14] == 0x0;
            set => mem[0x14] = value ? 0x1L : 0x0L;
        }
        /// <summary>
        /// overflow flag
        /// </summary>
        public bool of
        {
            get => mem[0x15] == 1;
            set => mem[0x15] = value ? 0x1L : 0x0L;
        }
        /// <summary>
        /// negative flag
        /// </summary>
        public bool nf
        {
            get => mem[0x16] == 1;
            set => mem[0x16] = value ? 0x1L : 0x0L;
        }
        /// <summary>
        /// break flag (for next execute)
        /// </summary>
        public bool bf
        {
            get => mem[0x17] == 1;
            set => mem[0x17] = value ? 0x1L : 0x0L;
        }
        /// <summary>
        /// float flag
        /// </summary>
        public bool ff
        {
            get => mem[0x18] == 1;
            set => mem[0x18] = value ? 0x1L : 0x0L;
        }
        /// <summary>
        /// stack forward flag
        /// </summary>
        public bool sf
        {
            get => mem[0x19] == 1;
            set => mem[0x19] = value ? 0x1L : 0x0L;
        }

        public ulong curAddr { get; set; } = 0xFFFF;
        public ulong lastAddr { get; set; } = 0xFFFF;

        public long[] mem = new long[32];

        public Stack<long> stack = new Stack<long>();

        public sbyte halt { get; set; } = 0;

        public List<ulong> program { get; set; } = new List<ulong>();

        public void Load(params ulong[] prog) => program.AddRange(prog);

        public ulong? next(ulong pc_ref)
        {
            if (program.Count >= (int) pc) 
                return program.ElementAt((int) pc_ref);
            return null;
        }
        public ulong fetch()
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
                    Array.Fill(mem, 0xDEADL, 0, 16);
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
            if (bf)
            {
                bus.debugger.handleBreak(u16 & pc, bus.cpu);
                mem[0x17] = 0x0;
            }
            trace($"0x{r1:X} 0x{r2:X} 0x{r3:X} 0x{u1:X} 0x{u2:X} 0x{x1:X} 0x{x2:X}");
           
            switch (iid)
            {
                case ushort opcode when opcode.In(0xD0..0xE8):
                    /* need @float-flag */
                    if(mem[0x18] != 0x0) bus.cpu.halt(0xA9);
                    trace($"call [0xD0..0xE8]::0x{iid:X}");
                    var result = iid switch {
                        0xD0 => f32i64 & Abs    (i64f32 & mem[r1]),
                        0xD1 => f32i64 & Acos   (i64f32 & mem[r1]),
                        0xD2 => f32i64 & Atan   (i64f32 & mem[r1]),
                        0xD3 => f32i64 & Acosh  (i64f32 & mem[r1]),
                        0xD4 => f32i64 & Atanh  (i64f32 & mem[r1]),
                        0xD5 => f32i64 & Asin   (i64f32 & mem[r1]),
                        0xD6 => f32i64 & Asinh  (i64f32 & mem[r1]),
                        0xD7 => f32i64 & Cbrt   (i64f32 & mem[r1]),
                        0xD8 => f32i64 & Ceiling(i64f32 & mem[r1]),
                        0xD9 => f32i64 & Cos    (i64f32 & mem[r1]),
                        0xDA => f32i64 & Cosh   (i64f32 & mem[r1]),
                       
                        0xDB => f32i64 & Floor  (i64f32 & mem[r1]),
                        0xDC => f32i64 & Exp    (i64f32 & mem[r1]),
                        0xDD => f32i64 & Log    (i64f32 & mem[r1]),
                        0xDE => f32i64 & Log10  (i64f32 & mem[r1]),
                        0xDF => f32i64 & Tan    (i64f32 & mem[r1]),
                        0xE0 => f32i64 & Tanh   (i64f32 & mem[r1]),
                        
                        0xE4 => f32i64 & Atan2  (i64f32 & mem[r1], i64f32 & mem[r2]),
                        0xE5 => f32i64 & Min    (i64f32 & mem[r1], i64f32 & mem[r2]),
                        0xE6 => f32i64 & Max    (i64f32 & mem[r1], i64f32 & mem[r2]),

                        0xE7 => f32i64 & Sin    (i64f32 & mem[r1]),
                        0xE8 => f32i64 & Sinh   (i64f32 & mem[r1]),

                        0xE1 => f32i64 & Truncate(i64f32 & mem[r1]),
                        0xE2 => f32i64 & BitDecrement(i64f32 & mem[r1]),
                        0xE3 => f32i64 & BitIncrement(i64f32 & mem[r1]),
                    };
                    /* @stack-forward-flag */
                    if (sf)  stack.Push(result);
                    else     mem[r1] = result;
                    break;
                case 0xA:
                    break;
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
                    trace($"ldi 0x{u1:X}, 0x{u2:X} -> 0x{r1:X}");
                    _ = u2 switch
                    {
                        0x0 => mem[r1] = u1,
                        _   => mem[r1] = i64 & ((u2 << 4) | u1)
                    };
                    break;
                case 0x1 when x2 == 0xA:
                    trace($"ldx 0x{u1:X}, 0x{u2:X} -> 0x{r1:X}-0x{r2:X}");
                    mem[((r1 << 4) | r2)] = i64 & ((u1 << 4) | u2);
                    break;
                case 0x3: // 0x3120000
                    trace($".swap 0x{r1:X}, 0x{r2:X}");
                    mem[r1] ^= mem[r2];
                    mem[r2] =  mem[r1] ^ mem[r2];
                    mem[r1] ^= mem[r2];
                    break;
                case 0xF when x2 == 0xE: // 0xF9988E0
                    trace($".mv_u4 0x{r1:X} -> 0x{r2:X} -> 0x{u1:X}");
                    bus.Find(r1 & 0xFF).write(r2 & 0xFF, i32 & mem[u1] & 0xFF);
                    break;
                case 0xF when x2 == 0xC: // 0xF00000C
                    trace($"mv_u8 0x{r1:X} -> 0x{r2:X} -> [0x{u1:X}-0x{u2:X}]");
                    bus.Find(r1 & 0xFF).write(r2 & 0xFF, (r3 << 12 | u1 << 8 | u2 << 4 | x1) & 0xFFFFFFF);
                    break;
                case 0x8 when u2 == 0xC: // 0x8F000C0
                    trace($"ref_t 0x{r1:X}");
                    mem[r1] = i64 & pc;
                    break;
                case 0xA0:
                    for (var i = pc + r1; pc != i;)
                        stack.Push(i64 & fetch());
                    break;
                case 0xA1:
                    mem[r1] = stack.Pop();
                    break;
                case 0xA5: /* @sig */
                    stack.Push(mem[r2] = i64 & pc);
                    var frag = default(ulong?);
                    while (AcceptOpCode(frag) != IID.ret.getOpCode())
                    {
                        var pc_r = stack.Pop();
                        frag = next(i64 | pc_r++);
                        if (frag is null) 
                            bus.cpu.halt(0xA1);
                        stack.Push(pc_r);
                    }
                    mem[r2 + 1] = stack.Pop();
                    break;
                case 0xB1: mem[r1]++; break; /* @inc */
                case 0xB2: mem[r1]--; break; /* @dec */


                #region debug

                case 0xF when x2 == 0xF: // mvx
                    trace($"mvx 0x{r1:X} 0x{r2:X} 0x{u1:X}");
                    var x = mem[u1].ToString();

                    if (ff)
                        x = (i64f32 & mem[u1]).ToString(CultureInfo.InvariantCulture);

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

                case 0xC1 when x2 == 0x1: /* @break :: now */
                    bus.debugger.handleBreak(u16 & pc, bus.cpu);
                    mem[0x17] = 0x0;
                    break;
                case 0xC1 when x2 == 0x2: /* @break :: next */
                    mem[0x17] = 0x1;
                    break;
                case 0xC1 when x2 == 0x3: /* @break :: after */
                    mem[0x17] = 0x3;
                    break;
                #endregion
                #region jumps

                case 0x8 when u2 == 0xF && x1 == 0x0: // 0x8F000F00
                    trace($"jump_t 0x{r1:X}");
                    pc = (ulong)mem[r1];
                    break;
                case 0x8 when u2 == 0xF && x1 == 0x1: // 0x8FCD0F10
                    trace(mem[r2] >= mem[r3]
                        ? $"jump_e 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> apl"
                        : $"jump_e 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> skip");
                    if(mem[r2] >= mem[r3]) 
                        pc = i64 | mem[r1];
                    break;
                case 0x8 when u2 == 0xF && x1 == 0x2: // 0x8FCD0F20
                    trace(mem[r2] > mem[r3]
                        ? $"jump_g 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> apl"
                        : $"jump_g 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> skip");
                    if(mem[r2] > mem[r3])
                        pc = (ulong)mem[r1];
                    break;
                case 0x8 when u2 == 0xF && x1 == 0x3: // 0x8FCD0F30
                    trace(mem[r2] < mem[r3]
                        ? $"jump_u 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> apl"
                        : $"jump_u 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> skip");
                    if(mem[r2] < mem[r3])
                        pc = (ulong)mem[r1];
                    break;
                case 0x8 when u2 == 0xF && x1 == 0x4: // 0x8FCD0F40
                    trace(mem[r2] <= mem[r3]
                        ? $"jump_y 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> apl"
                        : $"jump_y 0x{r1:X} -> 0x{r2:X} 0x{r3:X} -> skip");
                    if(mem[r2] <= mem[r3]) pc = (ulong)mem[r1];
                    break;

                #endregion
                // 0xD0-0xEC
                #region math 
                case 0xCA:
                    trace($"sum 0x{r2:X}, 0x{r3:X}");
                    if (ff)
                        mem[r1] = f32i64 & (i64f32 & mem[r2]) + (i64f32 & mem[r3]);
                    else
                        mem[r1] = mem[r2] + mem[r3];
                    break;
                case 0xCB:
                    trace($".sub 0x{r2:X}, 0x{r3:X}");
                    if (ff)
                        mem[r1] = f32i64 & (i64f32 & mem[r2]) - (i64f32 & mem[r3]);
                    else
                        mem[r1] = mem[r2] - mem[r3];
                    break;
                case 0xCC:
                    trace($".div 0x{r2:X}, 0x{r3:X}");
                    _ = (mem[r3], ff) switch {
                        (0x0, _    ) => bus.cpu.halt(0xC),
                        (_  , false) => mem[r1] = mem[r2] / mem[r3],
                        (_  , true ) => mem[r1] = f32i64 & (i64f32 & mem[r2]) / (i64f32 & mem[r3])
                        };
                    break;
                case 0xCD:
                    trace($".mul 0x{r2:X}, 0x{r3:X}");
                    if (ff)
                        mem[r1] = f32i64 & (i64f32 & mem[r2]) * (i64f32 & mem[r3]);
                    else
                        mem[r1] = mem[r2] * mem[r3];
                    break;
                
                case 0xCE:
                    trace($".pow 0x{r2:X}, 0x{r3:X}");
                    if (ff)
                        mem[r1] = f32i64 & Pow(i64f32 & mem[r2], i64f32 & mem[r3]);
                    else
                        mem[r1] = (uint)Math.Pow(mem[r2], mem[r3]);
                    break;
                case 0xCF:
                    trace($"sqrt 0x{r2:X}");
                    if (ff)
                        mem[r1] = f32i64 & Sqrt(i64f32 & mem[r2]);
                    else
                        mem[r1] = (uint)Math.Sqrt(mem[r2]);
                    break;
                
                #endregion
                default:
                    Error(
                        $"Unk OpCode: {iid:X2} {Environment.NewLine}0x{r1:X} 0x{r2:X} 0x{r3:X} 0x{u1:X} 0x{u2:X} 0x{x1:X} 0x{x2:X}");
                    break;
            }

            if (mem[0x17] == 0x3) mem[0x17] = 0x2;
            if (mem[0x17] == 0x2)
            {
                bus.debugger.handleBreak(u16 & pc, bus.cpu);
                mem[0x17] = 0x0;
            }

            Registers.Reflect();
        }
        // ===
        // :: current instruction map (x8 bit instruction size, x40bit data)
        // reserved    r   u  x
        //   | opCode 123 123 12
        //   |     |   |   |  |
        // 0xFFFF_FFCC_AAA_BBB_DD
        // ===
        // :: future instruction map (x16 bit instruction size, x40bit data)
        //          r     u    x f
        //  opCode 1234  1234  1212 
        //   |  |
        // 0xFFFF__AAAA__DDDD__EEEE
        // ===
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

        public ushort AcceptOpCode(BitwiseContainer mem)
        {
            var o1 = u16 & (mem & 0xF00000000);
            var o2 = u16 & (mem & 0x0F0000000);
            return u16 & (o1 << 0x4 | o2);
        }

        

        public static Unicast<byte  , long > u8  = new Unicast<byte  , long>();
        public static Unicast<ushort, ulong> u16 = new Unicast<ushort, ulong>();
        public static Unicast<uint  , long > u32 = new Unicast<uint  , long>();
        public static Unicast<int   , long > i32 = new Unicast<int   , long>();
        public static Unicast<ulong , ulong> u64 = new Unicast<ulong , ulong>();
        public static Unicast<long  , ulong> i64 = new Unicast<long  , ulong>();

        public static Bitcast<float, long > i64f32 = new Bitcast<float, long>();
        public static Bitcast<long , float> f32i64 = new Bitcast<long , float>();

        

        private void trace(string str)
        {
            if(tc)
                WriteLine(str);
            Trace.WriteLine(str);
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
        public static implicit operator BitwiseContainer(long value) => new BitwiseContainer((ulong)value);
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