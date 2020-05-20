namespace vm.component
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using ancient.runtime;
    using ancient.runtime.@base;
    using ancient.runtime.emit.sys;
    using ancient.runtime.emit.@unsafe;
    using ancient.runtime.exceptions;
    using ancient.runtime.@unsafe;
    using Pastel;
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
            Array.Fill(mem_types, new Unknown_Type());
            this.pipe = new ArgumentPipe(this);
            this.cr = new ControlRegister(this);
            this.CallStack = new CallStack(bus.debugger.GetSymbols());
            this.CallStack.Enter(null, pc);
        }


        #region casters

        /// <summary><see cref="byte"/> to <see cref="long"/></summary>
        public static readonly Unicast<byte  , ulong > u8  = new Unicast<byte  , ulong>();
        /// <summary><see cref="ushort"/> to <see cref="ulong"/></summary>
        public static readonly Unicast<ushort, ulong> u16 = new Unicast<ushort, ulong>();
        /// <summary><see cref="uint"/> to <see cref="long"/></summary>
        public static readonly Unicast<uint  , ulong > u32 = new Unicast<uint  , ulong>();
        /// <summary><see cref="int"/> to <see cref="long"/></summary>
        public static readonly Unicast<int   , ulong > i32 = new Unicast<int   , ulong>();
        /// <summary><see cref="long"/> to <see cref="ulong"/></summary>
        public static readonly Unicast<long  , ulong> i64 = new Unicast<long  , ulong>();

        /// <summary>bytecast <see cref="float"/> to <see cref="long"/></summary>
        public static readonly Bitcast<float, long > i64f32 = new Bitcast<float, long>();
        public static readonly Bitcast<float, ulong > u64f32 = new Bitcast<float, ulong>();
        /// <summary>bytecast <see cref="long"/> to <see cref="float"/></summary>
        public static readonly Bitcast<long , float> f32i64 = new Bitcast<long , float>();
        public static readonly Bitcast<ulong , float> f32u64 = new Bitcast<ulong , float>();
        /// <summary><see cref="int"/> to <see cref="short"/></summary>
        public static readonly Unicast<int  , short> i32i16 = new Unicast<int  , short>();

        #endregion

        #region Registers
        public long SP { get; set; }
        
        public ulong pc { get; set; }

        /// <summary>
        /// base register
        /// </summary>
        public ushort r1 { get; set; }
        public ushort r2 { get; set; }
        public ushort r3 { get; set; }
        /// <summary>
        /// value register
        /// </summary>
        public ushort u1 { get; set; }
        public ushort u2 { get; set; }
        /// <summary>
        /// magic registers
        /// </summary>
        public ushort x1 { get; set; }
        public ushort x2 { get; set; }
        public ushort x3 { get; set; }
        public ushort x4 { get; set; }
        /// <summary>
        /// meta registers
        /// </summary>
        public ushort o1 { get; set; }
        public ushort o2 { get; set; }
        public ushort o3 { get; set; }

        public ControlRegister cr { get; }
        
        /// <summary>
        /// id
        /// </summary>
        public ushort iid { get; set; }

        internal unsafe void LoadMeta(byte[] meta)
        {
            using var steam = new MemoryStream(meta);
            while (steam.Position != steam.Length)
            {
                var template = MetaTemplate.FromBytes(steam.ReadBytes(sizeof(MetaTemplate)));

                if (template.type == TemplateType.STR)
                    StringLiteralMap.InternString(NativeString.Wrap(Encoding.UTF8.GetString(steam.ReadBytes(template.len))));
                if (template.type == TemplateType.RND)
                    steam.ReadBytes(template.len);
            }
        }

        public sbyte memoryChannel { get; set; }

        #endregion

        #region flags

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
        /// <summary>
        /// negative flag
        /// </summary>
        public bool nf
        {
            get => mem[0x16] == 1;
            set => mem[0x16] = value ? 0x1UL : 0x0UL;
        }
        /// <summary>
        /// break flag (for next execute)
        /// </summary>
        public bool bf
        {
            get => mem[0x17] == 1;
            set => mem[0x17] = value ? 0x1UL : 0x0UL;
        }
        /// <summary>
        /// float flag
        /// </summary>
        public bool ff
        {
            get => mem[0x18] == 1;
            set => mem[0x18] = value ? 0x1UL : 0x0UL;
        }
        /// <summary>
        /// stack forward flag
        /// </summary>
        public bool sf
        {
            get => mem[0x19] == 1;
            set => mem[0x19] = value ? 0x1UL : 0x0UL;
        }
        /// <summary>
        /// control stack flag
        /// </summary>
        public bool northFlag
        {
            get => mem[0x20] == 1;
            set => mem[0x20] = value ? 0x1UL : 0x0UL;
        }
        /// <summary>
        /// control stack flag
        /// </summary>
        public bool eastFlag
        {
            get => mem[0x21] == 1;
            set => mem[0x21] = value ? 0x1UL : 0x0UL;
        }
        /// <summary>
        /// bios read-access
        /// </summary>
        public bool southFlag
        {
            get => mem[0x22] == 1;
            set => mem[0x22] = value ? 0x1UL : 0x0UL;
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
        /// L1 Memory
        /// </summary>
        public ulong[] mem { get; } = new ulong[64];
        /// <summary>
        /// L1 Types 
        /// </summary>
        public ExternType[] mem_types { get; } = new ExternType[64];
        /// <summary>
        /// L1 Memory flags
        /// </summary>
        private ulong[] flags = new ulong[64];

        /// <summary>
        /// CPU Stack
        /// </summary>
        public Stack stack { get; set; }

        /// <summary>
        /// Halt flag
        /// </summary>
        public sbyte halt { get; set; } = 0;

        public CallStack CallStack { get; }


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
                bus.find(0x0).write(pin + index, @ulong);
            bus.find(0x0).write(set, prog.Length);
            sectors.Add((name, u32 & pin));
            if(pc == 0x0) pc = i64 | pin;
        }

        /// <summary>
        /// Append new instruction to state
        /// for REPL
        /// </summary>
        /// <remarks>
        /// 1. read len of current program set from memory
        /// 2. add to len one
        /// 3. write new len of program set to memory
        /// 4. write new instruction to end in program set
        /// </remarks>
        public void Append(ulong instruction)
        {
            const int set = 0x599;
            var target = 0x600 + pc;
            var len = bus.find(0x0).read(set);
            bus.find(0x0).write(set, ++len);
            bus.find(0x0).write(i64 & ++target, instruction);
            pc = target;
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
            if (bus.find(0x0).read(0x599) >= pc && ++step != 0x90000) 
                return bus.find(0x0).read(0x600 + (i64 & pc_ref));
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
        public ulong fetch()
        {
            try
            {
                if (halt != 0) return 0;
                lastAddr = curAddr;
                if (++step == 0x90000)
                    return i64 | bus.cpu.halt(0x2);
                if (bus.find(0x0).read(0x599) != pc - 0x600)
                    return curAddr = bus.find(0x0).read(i64 & pc++);
                return i64 | bus.cpu.halt(0x77);
            }
            catch
            {
                if (!km) 
                    Array.Fill(mem, 0xDEADUL, 0, 16);
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
            ushort 
            pfx = u16 & (container & 0xF0000000000000);
            iid = u16 & (container & 0x0F000000000000);
            r1  = u16 & (container & 0x00F00000000000);
            r2  = u16 & (container & 0x000F0000000000);
            r3  = u16 & (container & 0x0000F000000000);
            u1  = u16 & (container & 0x00000F00000000);
            u2  = u16 & (container & 0x000000F0000000);
            x1  = u16 & (container & 0x0000000F000000);
            x2  = u16 & (container & 0x00000000F00000);
            x3  = u16 & (container & 0x000000000F0000);
            x4  = u16 & (container & 0x0000000000F000);
            o1  = u16 & (container & 0x00000000000F00);
            o2  = u16 & (container & 0x000000000000F0);
            o3  = u16 & (container & 0x0000000000000F);
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
            OnTrace?.Invoke(str);
        }

        private void warn(string str)
        {
            if(!AppFlag.GetVariable("SKIP_WARNING"))
            WriteLine($"-  {str}  -".PastelBg(Color.OrangeRed));
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


        private ArgumentPipe pipe { get; }

        public class ArgumentPipe
        {
            private readonly State _state;

            private ushort x3 => _state.x3;
            internal ushort result => (d8u)(u8 & _state.r1, u8 & _state.r2);
            internal ushort arg1 => (d8u)(u8 & _state.r3, u8 & _state.u1);
            internal ushort arg2 => (d8u)(u8 & _state.u2, u8 & _state.x1);

            private Stack stack => _state.stack;
            private ulong[] mem => _state.mem;
            private CPU cpu => _state.bus.cpu;

            public ArgumentPipe(State state) => _state = state;

            public ulong this[byte index]
            {
                get => x3 switch
                {
                    _ when x3 == 0x0 || x3 == 0x1 => stack.pop(),
                    _ when x3 == 0x2 || x3 == 0x3 => index switch
                    {
                        (0x1) => mem[arg1],
                        (0x2) => mem[arg2],
                        (0x3) => mem[result],
                        _ => (ulong) cpu.halt(0xD8)
                    },
                    _ => (ulong) cpu.halt(0xD7)
                };
                set => _ = x3 switch
                {
                    _ when x3 == 0x0 || x3 == 0x2 => i(() => stack.push(value)),
                    (0x1) => mem[arg1] = value,
                    (0x3) => index switch
                    {
                        (0x1) => mem[arg1] = value,
                        (0x2) => mem[arg2] = value,
                        (0x3) => mem[result] = value,
                        _ => (ulong) cpu.halt(0xD8)
                    },
                    _ => (ulong) cpu.halt(0xD7)
                };
            }

            private ulong i(Action action)
            {
                action();
                return 0;
            }
        }
    }
    public enum Mode : byte
    {
        StackOnly = 0x0,
        ResultCellOnly = 0x1,
        ArgCellAndStack = 0x2,
        All = 0x3,
    }
}