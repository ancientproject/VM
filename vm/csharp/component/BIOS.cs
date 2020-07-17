namespace vm.component
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Threading;
    using ancient.runtime;
    using ancient.runtime.emit.@unsafe;
    using Pastel;
    using static State;

    public class BIOS : Device
    {
        private readonly Bus _bus;
        public Stopwatch systemTimer;
        public DateTime startTime;

        private readonly ulong[] mem = new ulong[32];
        /// <summary>
        /// 0x1, using hpet timer
        /// </summary>
        public bool hpet
        {
            set => mem[0x1] = value ? 0x1UL : 0x0UL;
            get => mem[0x1] == 0x1;
        }
        /// <summary>
        /// 0x2, using virtual forwarding
        /// </summary>
        public bool virtual_stack
        {
            set => mem[0x2] = value ? 0x1UL : 0x0UL;
            get => mem[0x2] == 0x1;
        }
        /// <summary>
        /// 0x3, using forward in standalone memory sector
        /// </summary>
        public bool memory_stack_forward
        {
            set => mem[0x3] = value ? 0x1UL : 0x0UL;
            get => mem[0x3] == 0x1;
        }
        /// <summary>
        /// 0x6, using guarding with violation memory write
        /// </summary>
        public bool bios_guard
        {
            set => mem[0x6] = value ? 0x1UL : 0x0UL;
            get => mem[0x6] == 0x1;
        }
        
        /// <summary>
        /// Ticks count after started system
        /// </summary>
        public long Ticks 
            => hpet ? systemTimer.ElapsedTicks : Environment.TickCount;

        public BIOS(Bus bus) : base(0x45, "<chipset>")
        {
            _bus = bus;
            bios_guard = true;
            this.hpet = AppFlag.GetVariable("c69_bios_hpet");
        }
        /// <summary>
        /// read 64 bit data from internal bios memory
        /// </summary>
        public override ulong read(long address)
        {
            var (u1, u2) = new d8u((byte) address);
            return (u1, _bus.State.southFlag) switch
            {
                (0x0, _) => i64 | unchecked((int) Ticks),
                (0x1, _) => i64 | (hpet ? 0x1 : 0x0),
                (0x2, _) => i64 | _bus.State.memoryChannel,
                (0xF, true) => mem[u2],
                _ => throw ThrowMemoryRead(_bus.State.curAddr, address)
            };
        }
        /// <summary>
        /// write 64bit data to internal bios memory
        /// </summary>
        public override void write(long address, ulong data)
        {
            var (adr, u2) = new d8u((byte)address);
            _ = (adr, bios_guard, _bus.State.southFlag) switch {
                (0x1, _, _)         => X(() => hpet = data == 0x1),
                (0xA, _, _)         => X(warmUp),
                (0xC, true, false)  => X(clearRAM),
                (0xD, _, _)         => X(() => Thread.Sleep((int)data)),
                (0xF, _, true)      => X(() => mem[u2] = data),
                _                   => X(() => ThrowMemoryWrite(_bus.State.curAddr, address))
            };
        }
        /// <summary>
        /// clear memory in default slot
        /// </summary>
        private void clearRAM()
        {
            if(_bus.find(0x0) is Memory slot)
                Array.Fill(slot.mem, 0UL);
        }
        /// <summary>
        /// Shutdown system
        /// </summary>
        public override void shutdown()
        {
            systemTimer.Stop();
            var line = $"    VM is stopped, total operating time: {systemTimer.Elapsed:g}    ";

            var len = $"|{new string('-', line.Length)}|".Length;
            Console.SetCursorPosition((Console.WindowWidth - len) / 2, Console.CursorTop);
            Console.WriteLine($"|{new string('-', line.Length)}|".PastelBg(Color.DarkRed));
            Console.SetCursorPosition((Console.WindowWidth - len) / 2, Console.CursorTop);
            Console.WriteLine($"|{line}|".PastelBg(Color.DarkRed));
            Console.SetCursorPosition((Console.WindowWidth - len) / 2, Console.CursorTop);
            Console.WriteLine($"|{new string('-', line.Length)}|".PastelBg(Color.DarkRed));
        }

        public int X(Action _) // the fuck
        {
            _();
            return 0;
        }
        /// <summary>
        /// warm up system
        /// </summary>
        public override void warmUp()
        {
            systemTimer = Stopwatch.StartNew();
            startTime = DateTime.UtcNow;
        }
    }
}
