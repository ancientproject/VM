namespace vm.component
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using ancient.runtime;
    using dev;
    public class BIOS : AbstractDevice
    {
        private readonly CPU _cpu;
        private readonly Bus _bus;
        public Stopwatch systemTimer;
        public DateTime startTime;

        private readonly ulong[] mem = new ulong[32];

        public bool hpet
        {
            set => mem[0x1] = value ? 0x1UL : 0x0UL;
            get => mem[0x1] == 0x1;
        }

        public long Ticks 
            => hpet ? systemTimer.ElapsedTicks : Environment.TickCount;

        public BIOS(CPU cpu, Bus bus) : base(0x0, "<chipset>")
        {
            _cpu = cpu;
            _bus = bus;
            this.hpet = AppFlag.GetVariable("c69_bios_hpet");
        }

        public override long read(long address) => (address, _bus.State.ff) switch
            {
                (0x0, _) => unchecked((int)Ticks),
            _        => throw ThrowMemoryRead(_bus.State.curAddr, address)
            };

        public override void write(long address, long data)
        {
            _ = address switch {
                0x1 => X(() => hpet = data == 0x1),
                0xF => X(warmUp),
                0xD => X(() => Thread.Sleep((int)data)),
                _   => X(() => ThrowMemoryWrite(_bus.State.curAddr, address))
            };
        }

        public int X(Action _)
        {
            _();
            return 0;
        }

        public override void warmUp()
        {
            systemTimer = Stopwatch.StartNew();
            startTime = DateTime.UtcNow;
        }
    }
}