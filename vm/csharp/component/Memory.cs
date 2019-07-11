namespace vm.component
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using ancient.runtime;
    using MoreLinq;

    

    [DebuggerTypeProxy(typeof(MemoryView))]
    public class Memory : Device
    {
        private readonly CPU _cpu;
        internal readonly long[] mem;

        public Memory(int startAddress, int endAddress, Bus bus) : base(0x0, "<ddr>")
        {
            _cpu = bus.cpu;
            // 512kb max
            if (endAddress >= 0x100000)
                _cpu.halt(0xBD);
            mem = new long[endAddress - startAddress + 1];
        }


        public override void write(long address, long data)
        {
            if (address >= mem.Length)
            {
                _cpu.halt(0xBD);
                return;
            }
            mem[address] = data;
        }
        public override long read(long address)
        {
            if (address < mem.Length) 
                return mem[address];
            _cpu.halt(0xBD);
            return 0;
        }

        public void load(byte[] binary, int memOffset, int maxLen)
        {
            if (binary.Length % sizeof(long) != 0)
                _cpu.halt(0xD6);
            var bin = binary.Batch(sizeof(long)).Select(x => BitConverter.ToInt64(x.ToArray())).Reverse().ToArray();
            Array.Copy(bin, 0, mem, memOffset, maxLen);
        }


        internal class MemoryView
        {
            private readonly Memory _mem;

            public MemoryView(Memory mem) => _mem = mem;


            public long[] all => _mem.mem;
            public long[] execute => _mem.mem.Select((i, z) => (i, z)).SkipWhile(x => x.z != 0x600).Select(x => x.i).ToArray();
            public long execute_len => _mem.mem[0x599];
            public long[] bios => _mem.mem.Select((i, z) => (i, z)).SkipWhile(x => x.z != 0x300).Select(x => x.i).ToArray();
            public long[] stack => _mem.mem.Select((i, z) => (i, z)).SkipWhile(x => x.z != 0x100).Select(x => x.i).ToArray();
        }
    }
}