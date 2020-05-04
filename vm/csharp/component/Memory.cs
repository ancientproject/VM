namespace vm.component
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using ancient.runtime;
    using JetBrains.Annotations;
    using MoreLinq;

    
    // TODO add more memory range
    [DebuggerTypeProxy(typeof(MemoryView))]
    public class Memory : Device
    {
        private readonly CPU _cpu;
        internal readonly ulong[] mem;

        public Memory(int startAddress, int endAddress, Bus bus) : base(0x0, "<ddr>")
        {
            _cpu = bus.cpu;
            // 512kb max
            if (endAddress >= 0x100000)
                _cpu.halt(0xBD);
            mem = new ulong[endAddress - startAddress + 1];
        }


        public override void write(long address, ulong data)
        {
            if (address >= mem.Length)
            {
                _cpu.halt(0xBD);
                return;
            }
            mem[address] = data;
        }
        public override ulong read(long address)
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

        [UsedImplicitly]
        internal class MemoryView
        {
            private readonly Memory obj_ref;

            public MemoryView(Memory @ref) => obj_ref = @ref;


            public ulong[] all => obj_ref.mem;
            public ulong[] execute => obj_ref.mem.Select((i, z) => (i, z)).SkipWhile(x => x.z != 0x600).Select(x => x.i).ToArray();
            public ulong execute_len => obj_ref.mem[0x599];
            public ulong[] bios => obj_ref.mem.Select((i, z) => (i, z)).SkipWhile(x => x.z != 0x300).Select(x => x.i).ToArray();
            public ulong[] stack => obj_ref.mem.Select((i, z) => (i, z)).SkipWhile(x => x.z != 0x100).Select(x => x.i).ToArray();
        }
    }
}