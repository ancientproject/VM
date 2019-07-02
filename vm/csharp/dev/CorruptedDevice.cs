namespace vm.dev
{
    using component;
    using ancient.runtime.exceptions;

    public class CorruptedDevice : AbstractDevice
    {
        private readonly CPU _cpu;

        public CorruptedDevice(CPU cpu) : base(0xFF, "<unknown-device>") => _cpu = cpu;

        public override void write(long address, long data)
        {
            throw new CorruptedMemoryException($"Instruction at address 0x{_cpu.State.curAddr:X4} accessed memory 0x{address:X}. Memory could not be write.");
        }

        
        public override long read(long address)
        {
            throw new CorruptedMemoryException($"Instruction at address 0x{_cpu.State.curAddr:X4} accessed memory 0x{address:X}. Memory could not be read.");
        }
    }
}