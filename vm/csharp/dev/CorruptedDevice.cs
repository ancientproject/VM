namespace vm.dev
{
    using component;
    using ancient.runtime.exceptions;

    public class CorruptedDevice : AbstractDevice
    {
        private readonly CPU _cpu;

        public CorruptedDevice(CPU cpu) : base(0xFF, "<unknown-device>") => _cpu = cpu;

        public override void write(int address, int data)
        {
            throw new CorruptedMemoryException($"Instruction at address 0x{_cpu.State.curAddr:X4} accessed memory 0x{address:X4}. Memory could not be write.");
        }

        
        public override int read(int address)
        {
            throw new CorruptedMemoryException($"Instruction at address 0x{_cpu.State.curAddr:X4} accessed memory 0x{address:X4}. Memory could not be read.");
        }
    }
}