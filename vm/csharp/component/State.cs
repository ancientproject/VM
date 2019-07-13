namespace vm.component
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ancient.runtime;
    using ancient.runtime.exceptions;
    using static System.Console;

    /// <summary>
    /// <see cref="State"/> of <see cref="CPU"/>
    /// </summary>
    public partial class State
    {
        private readonly Bus bus;

        public State(Bus bus)
        {
            this.bus = bus;
            this.stack = new Stack(bus);
        }


        #region casters

        /// <summary><see cref="byte"/> to <see cref="long"/></summary>
        public static readonly Unicast<byte  , long > u8  = new Unicast<byte  , long>();
        /// <summary><see cref="ushort"/> to <see cref="ulong"/></summary>
        public static readonly Unicast<ushort, ulong> u16 = new Unicast<ushort, ulong>();
        /// <summary><see cref="uint"/> to <see cref="long"/></summary>
        public static readonly Unicast<uint  , long > u32 = new Unicast<uint  , long>();
        /// <summary><see cref="int"/> to <see cref="long"/></summary>
        public static readonly Unicast<int   , long > i32 = new Unicast<int   , long>();
        /// <summary><see cref="long"/> to <see cref="ulong"/></summary>
        public static readonly Unicast<long  , ulong> i64 = new Unicast<long  , ulong>();

        /// <summary>bytecast <see cref="float"/> to <see cref="long"/></summary>
        public static readonly Bitcast<float, long > i64f32 = new Bitcast<float, long>();
        /// <summary>bytecast <see cref="long"/> to <see cref="float"/></summary>
        public static readonly Bitcast<long , float> f32i64 = new Bitcast<long , float>();
        /// <summary><see cref="int"/> to <see cref="short"/></summary>
        public static readonly Unicast<int  , short> i32i16 = new Unicast<int  , short>();

        #endregion

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

        #region flags

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

        #endregion
        
        /// <summary>
        /// Current Address
        /// </summary>
        public ulong curAddr { get; set; } = 0xFFFF;
        /// <summary>
        /// Last executed address
        /// </summary>
        public ulong lastAddr { get; set; } = 0xFFFF;

        /// <summary>
        /// CPU Steps
        /// </summary>
        public virtual ulong step { get; set; }

        /// <summary>
        /// L1 Cache memory
        /// </summary>

        public long[] mem = new long[64];

        /// <summary>
        /// CPU Stack
        /// </summary>
        public Stack stack { get; set; }

        /// <summary>
        /// Halt flag
        /// </summary>
        public sbyte halt { get; set; } = 0;

        /// <summary>
        /// Section addressing information
        /// </summary>
        public List<(string block, uint address)> sectors = new List<(string block, uint address)>(16);

        /// <summary>
        /// Load to memory execution data
        /// </summary>
        /// <param name="name">name of execution memory</param>
        /// <param name="prog">execution memory</param>
        public void Load(string name, params ulong[] prog)
        {
            var pin = 0x600;
            var set = 0x599;
            if (AcceptOpCode(prog.First()) == 0x33)
            {
                Func<int> shift = ShiftFactory.Create(sizeof(int) * 0b100 - 0b100).Shift;
                Accept(prog.First());
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

        /// <summary>
        /// Read next address with <see cref="pc_ref"/>
        /// </summary>
        /// <param name="pc_ref">
        /// PC index
        /// </param>
        /// <returns>
        /// Fragment of execution memory
        /// </returns>
        public ulong? next(ulong pc_ref)
        {
            if (bus.Find(0x0).read(0x599) >= (i64 & pc) && ++step != 0x90000) 
                return i64 | bus.Find(0x0).read(0x600 + (i64 & pc_ref));
            return null;
        }

        /// <summary>
        /// Fetch next execution memory
        /// </summary>
        /// <returns>
        /// Fragment of execution memory
        /// </returns>
        /// <exception cref="CorruptedMemoryException">
        /// Memory instruction at address access to memory could not be read.
        /// </exception>
        /// <exception cref="StepOverflowException">
        /// Too many step count
        /// </exception>
        public ulong fetch()
        {
            try
            {
                if (halt != 0) return 0;
                lastAddr = curAddr;
                if (++step == 0x90000)
                    throw new StepOverflowException();
                if (bus.Find(0x0).read(0x599) != (i64 & pc) && ++step != 0x90000)
                    return (curAddr = i64 | bus.Find(0x0).read((i64 & pc++)));
                bus.cpu.halt(0x77);
                return 0xDEAD;
            }
            catch (StepOverflowException) { throw; }
            catch
            {
                if (!km) Array.Fill(mem, 0xDEADL, 0, 16);
                throw new CorruptedMemoryException($"Memory instruction at address 0x{curAddr:X4} access to memory 0x{pc:X4} could not be read.");
            }
        }
        /// <summary>
        /// Deconstruct <see cref="UInt64"/> to registres
        /// </summary>
        /// <remarks>
        /// ===
        /// :: current instruction map (x8 bit instruction size, x40bit data)
        /// reserved    r   u  x
        ///   | opCode 123 123 12
        ///   |     |   |   |  |
        /// 0xFFFF_FFCC_AAA_BBB_DD
        /// ===
        /// :: future instruction map (x16 bit instruction size, x48bit data)
        ///          r     u    x f
        ///  opCode 1234  1234  1212
        ///   |  |
        /// 0xFFFF__AAAA__DDDD__EEEE
        /// ===
        /// </remarks>
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
        /// <summary>
        /// Deconstruct <see cref="UInt64"/> to OpCode
        /// </summary>
        /// <param name="container"></param>
        /// <returns>
        /// 8 bit <see cref="ushort"/>
        /// </returns>
        public ushort AcceptOpCode(BitwiseContainer container)
        {
            var o1 = u16 & (container & 0xF00000000);
            var o2 = u16 & (container & 0x0F0000000);
            return u16 & (o1 << 0x4 | o2);
        }

        #region trace

        private void trace(string str)
        {
            if(tc) WriteLine(str);
            Trace.WriteLine(str);
            OnTrace?.Invoke(str);
        }

        private void Error(string str)
        {
            OnError?.Invoke(str);
            if (!ec) return;
            ForegroundColor = ConsoleColor.Red;
            WriteLine(str);
            ForegroundColor = ConsoleColor.White;
        }

        public event Action<string> OnTrace;
        public event Action<string> OnError;


        public override string ToString() => $"[{pc:X} {r1:X} {r2:X} {r3:X} {u1:X} {u2:X} {x1:X} {x2:X}]";

        #endregion
    }
}