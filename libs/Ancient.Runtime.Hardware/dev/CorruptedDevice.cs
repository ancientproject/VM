namespace ancient.runtime
{
    using exceptions;

    public class CorruptedDevice : Device
    {
        private readonly dynamic _cpu;

        public CorruptedDevice(dynamic cpu) : base(0xFF, "<???>") => _cpu = cpu;

        public override void write(long address, long data) 
            => throw new CorruptedMemoryException($"Instruction at address 0x{_cpu.State.curAddr:X4} accessed memory 0x{address:X}. Memory could not be write.");

        public override long read(long address) 
            => throw new CorruptedMemoryException($"Instruction at address 0x{_cpu.State.curAddr:X4} accessed memory 0x{address:X}. Memory could not be read.");
    }
}