namespace vm.component
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using ancient.runtime;
    using ancient.runtime.exceptions;
    using ancient.runtime.hardware;
    using MoreLinq;
    using static System.Console;
    using static System.MathF;

    public class State
    {
        private readonly Bus bus;

        public State(Bus bus)
        {
            this.bus = bus;
            this.stack = new Stack(bus);
        }


        #region Registers
        public long SP { get; set; }

        public ulong pc { get; set; }

        /// <summary>
        /// base register cell
        /// </summary>
        public ushort r1 { get; set; }
        public ushort r2 { get; set; }
        public ushort r3 { get; set; }
        /// <summary>
        /// value register cell
        /// </summary>
        public ushort u1 { get; set; }
        public ushort u2 { get; set; }
        /// <summary>
        /// magic cell
        /// </summary>
        public ushort x1 { get; set; }
        public ushort x2 { get; set; }
        
        /// <summary>
        /// id
        /// </summary>
        public ushort iid { get; set; }


        public sbyte memoryChannel { get; set; }

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
        /// <summary>
        /// control stack flag
        /// </summary>
        public bool northFlag
        {
            get => mem[0x20] == 1;
            set => mem[0x20] = value ? 0x1L : 0x0L;
        }
        /// <summary>
        /// control stack flag
        /// </summary>
        public bool eastFlag
        {
            get => mem[0x21] == 1;
            set => mem[0x21] = value ? 0x1L : 0x0L;
        }
        /// <summary>
        /// bios read-access
        /// </summary>
        public bool southFlag
        {
            get => mem[0x22] == 1;
            set => mem[0x22] = value ? 0x1L : 0x0L;
        }

        public ulong curAddr { get; set; } = 0xFFFF;
        public ulong lastAddr { get; set; } = 0xFFFF;

        public ulong step { get; set; } = 0x0;

        public long[] mem = new long[64];

        public Stack stack { get; set; }

        public sbyte halt { get; set; } = 0;


        public List<(string block, uint address)> sectors = new List<(string block, uint address)>(16);


        public void Load(string name, params ulong[] prog)
        {
            var pin = 0x600;
            var set = 0x599;
            if (AcceptOpCode(prog.First()) == 0x33)
            {
                Func<int> shift = ShiftFactory.Create(sizeof(int) * 0b100 - 0b100).Shift;
                Accept((ulong)prog.First());
                prog = prog.Skip(1).ToArray();
                pin = (r1 << shift()) | (r2 << shift()) | (r3 << shift()) | (u1 << shift());
                set = pin - 0b1;
            }
            foreach (var (@ulong, index) in prog.Select((x, i) => (x, i)))
                bus.Find(0x0).write(pin + index, i64 & @ulong);
            bus.Find(0x0).write(set, prog.Length);
            sectors.Add((name, u32 & pin));
            if(pc == 0x0) pc = i64 | pin;
        }

        public ulong? next(ulong pc_ref)
        {
            if (bus.Find(0x0).read(0x599) >= (i64 & pc) && ++step != 0x90000) 
                return i64 | bus.Find(0x0).read(0x600 + (i64 & pc_ref));
            return null;
        }
        public ulong fetch()
        {
            try
            {
                if (halt != 0) return 0;
                lastAddr = curAddr;
                if (bus.Find(0x0).read(0x599) != (i64 & pc) && ++step != 0x90000) 
                    return (curAddr = i64 | bus.Find(0x0).read((i64 & pc++)));
                throw new Exception();
            }
            catch
            {
                if (!km)
                    Array.Fill(mem, 0xDEADL, 0, 16);
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
                trace($"0x{r1:X} 0x{r2:X} 0x{r3:X} 0x{u1:X} 0x{u2:X} 0x{x1:X} 0x{x2:X}");
            }
            if (bf)
            {
                bus.debugger.handleBreak(u16 & pc, bus.cpu);
                mem[0x17] = 0x0;
            }
           
            switch (iid)
            {
                //
                case ushort opcode when opcode.In(0xD0..0xE8):
                    /* need @float-flag */
                    if(!ff) bus.cpu.halt(0xA9);
                    trace($"call :: [0xD0..0xE8]::0x{iid:X}");
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
                        _ => throw new CorruptedMemoryException($"")
                    };
                    /* @stack-forward-flag */
                    if (sf)  stack.push(result);
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
                    trace($"call :: ldi 0x{u1:X}, 0x{u2:X} -> 0x{r1:X}");
                    _ = u2 switch
                    {
                        0x0 => mem[r1] = u1,
                        _   => mem[r1] = i64 & ((u2 << 4) | u1)
                    };
                    break;
                case 0x1 when x2 == 0xA:
                    trace($"call :: ldx 0x{u1:X}, 0x{u2:X} -> 0x{r1:X}-0x{r2:X}");
                    mem[((r1 << 4) | r2)] = i64 & ((u1 << 4) | u2);
                    break;
                case 0x3: // 0x3120000
                    trace($"call :: swap, 0x{r1:X}, 0x{r2:X}");
                    mem[r1] ^= mem[r2];
                    mem[r2] =  mem[r1] ^ mem[r2];
                    mem[r1] ^= mem[r2];
                    break;
                case 0xF when x2 == 0xE: // 0xF9988E0
                    trace($"call :: move, 0x{r1:X} -> 0x{r2:X} -> 0x{u1:X}");
                    bus.Find(r1 & 0xFF).write(r2 & 0xFF, i32 & mem[u1] & 0xFF);
                    break;
                case 0xF when x2 == 0xC: // 0xF00000C
                    trace($"call :: move, 0x{r1:X} -> 0x{r2:X} -> [0x{u1:X}-0x{u2:X}]");
                    bus.Find(r1 & 0xFF).write(r2 & 0xFF, (r3 << 12 | u1 << 8 | u2 << 4 | x1) & 0xFFFFFFF);
                    break;
                case 0xA4:
                    trace($"call :: rfd 0x{r1:X}, 0x{r2:X}");
                    stack.push(bus.Find(r1 & 0xFF).read(r2 & 0xFF));
                    break;
                case 0x8 when u2 == 0xC: // 0x8F000C0
                    trace($"call :: ref_t 0x{r1:X}");
                    mem[r1] = i64 & pc;
                    break;
                case 0xA0:
                    trace($"call :: orb '{r1}' times");
                    for (var i = pc + r1; pc != i;)
                        stack.push(i64 & fetch());
                    break;
                case 0xA1:
                    trace($"call :: pull -> 0x{r1:X}");
                    mem[r1] = stack.pop();
                    break;
                case 0xA5: /* @sig */
                    stack.push(mem[r2] = i64 & pc);
                    var frag = default(ulong?);
                    while (AcceptOpCode(frag) != 0xA6 /* @ret */)
                    {
                        var pc_r = stack.pop();
                        frag = next(i64 | pc_r++);
                        if (frag is null) 
                            bus.cpu.halt(0xA1);
                        stack.push(pc_r);
                    }
                    mem[r2 + 1] = stack.pop();
                    break;
                case 0xB1: /* @inc */
                    trace($"call :: increment 0x{r1:X}++");
                    unchecked { mem[r1]++; } 
                    break; 
                case 0xB2:  /* @dec */
                    trace($"call :: decrement 0x{r1:X}--");
                    unchecked { mem[r1]--; } 
                    break; 


                #region debug

                case 0xF when x2 == 0xF:
                    var x = mem[u1].ToString();

                    if (ff) x = (i64f32 & mem[u1]).ToString(CultureInfo.InvariantCulture);

                    short[] cast(string str)
                    {
                        var list = new List<int>();
                        foreach (var c in str)
                        {
                            var uu1 = (c & 0xF0) >> 4;
                            var uu2 = (c & 0xF);
                            list.Add((uu1 << 4 | uu2) & 0xFFFFFFF);
                        }
                        return list.Select(i32i16.auto).ToArray();
                    }
                    foreach (var uuu in cast(x))
                        bus.Find(r1 & 0xFF).write(r2 & 0xFF, uuu);
                    break;

                case 0xC1 when x2 == 0x1: /* @break :: now */
                    trace($"[0x{iid:X}] @break :: now");
                    bus.debugger.handleBreak(u16 & pc, bus.cpu);
                    mem[0x17] = 0x0;
                    break;
                case 0xC1 when x2 == 0x2: /* @break :: next */
                    trace($"[0x{iid:X}] @break :: next");
                    mem[0x17] = 0x1;
                    break;
                case 0xC1 when x2 == 0x3: /* @break :: after */
                    trace($"[0x{iid:X}] @break :: after");
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
                case 0x09:
                    pc = (ulong)((r1 << 4) | r2);
                    break;

                #endregion
                #region math 
                case 0xCA:
                    trace($"call :: sum 0x{r2:X}, 0x{r3:X}");
                    if (ff)
                        mem[r1] = f32i64 & (i64f32 & mem[r2]) + (i64f32 & mem[r3]);
                    else
                        mem[r1] = mem[r2] + mem[r3];
                    break;
                case 0xCB:
                    trace($"call :: sub 0x{r2:X}, 0x{r3:X}");
                    if (ff)
                        mem[r1] = f32i64 & (i64f32 & mem[r2]) - (i64f32 & mem[r3]);
                    else
                        mem[r1] = mem[r2] - mem[r3];
                    break;
                case 0xCC:
                    trace($"call :: div 0x{r2:X}, 0x{r3:X}");
                    _ = (mem[r3], ff) switch {
                        (0x0, _    ) => bus.cpu.halt(0xC),
                        (_  , false) => mem[r1] = mem[r2] / mem[r3],
                        (_  , true ) => mem[r1] = f32i64 & (i64f32 & mem[r2]) / (i64f32 & mem[r3])
                    };
                    break;
                case 0xCD:
                    trace($"call :: mul 0x{r2:X}, 0x{r3:X}");
                    if (ff)
                        mem[r1] = f32i64 & (i64f32 & mem[r2]) * (i64f32 & mem[r3]);
                    else
                        mem[r1] = mem[r2] * mem[r3];
                    break;
                
                case 0xCE:
                    trace($"call :: pow 0x{r2:X}, 0x{r3:X}");
                    if (ff)
                        mem[r1] = f32i64 & Pow(i64f32 & mem[r2], i64f32 & mem[r3]);
                    else
                        mem[r1] = (uint)Math.Pow(mem[r2], mem[r3]);
                    break;
                case 0xCF:
                    trace($"call :: sqrt 0x{r2:X}");
                    if (ff)
                        mem[r1] = f32i64 & Sqrt(i64f32 & mem[r2]);
                    else
                        mem[r1] = (uint)Math.Sqrt(mem[r2]);
                    break;
                
                #endregion
                default:
                    bus.cpu.halt(0xFC);
                    Error($"call :: unknown opCode -> {iid:X2}");
                    break;
            }

            if (mem[0x17] == 0x3) mem[0x17] = 0x2;
            if (mem[0x17] == 0x2)
            {
                bus.debugger.handleBreak(u16 & pc, bus.cpu);
                mem[0x17] = 0x0;
            }
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
        public void Accept(BitwiseContainer container)
        {
            trace($"fetch 0x{container:X}");
            var 
            pfx = u16 & (container & 0xF00000000);
            iid = u16 & (container & 0x0F0000000);
            r1  = u16 & (container & 0x00F000000);
            r2  = u16 & (container & 0x000F00000);
            r3  = u16 & (container & 0x0000F0000);
            u1  = u16 & (container & 0x00000F000);
            u2  = u16 & (container & 0x000000F00);
            x1  = u16 & (container & 0x0000000F0);
            x2  = u16 & (container & 0x00000000F);
            iid = u16 & (pfx << 0x4 | iid );
        }

        public ushort AcceptOpCode(BitwiseContainer container)
        {
            var o1 = u16 & (container & 0xF00000000);
            var o2 = u16 & (container & 0x0F0000000);
            return u16 & (o1 << 0x4 | o2);
        }

        
        /// <summary><see cref="byte"/> to <see cref="long"/></summary>
        public static Unicast<byte  , long > u8  = new Unicast<byte  , long>();
        /// <summary><see cref="ushort"/> to <see cref="ulong"/></summary>
        public static Unicast<ushort, ulong> u16 = new Unicast<ushort, ulong>();
        /// <summary><see cref="uint"/> to <see cref="long"/></summary>
        public static Unicast<uint  , long > u32 = new Unicast<uint  , long>();
        /// <summary><see cref="int"/> to <see cref="long"/></summary>
        public static Unicast<int   , long > i32 = new Unicast<int   , long>();
        /// <summary><see cref="long"/> to <see cref="ulong"/></summary>
        public static Unicast<long  , ulong> i64 = new Unicast<long  , ulong>();

        /// <summary>bytecast <see cref="float"/> to <see cref="long"/></summary>
        public static Bitcast<float, long > i64f32 = new Bitcast<float, long>();
        /// <summary>bytecast <see cref="long"/> to <see cref="float"/></summary>
        public static Bitcast<long , float> f32i64 = new Bitcast<long , float>();


        /// <summary><see cref="int"/> to <see cref="short"/></summary>
        public static Unicast<int  , short> i32i16 = new Unicast<int  , short>();


        

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