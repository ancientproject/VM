namespace vm.dev
{
    using component;
    using flame.runtime.exceptions;

    public class CorruptedDevice : IDevice
    {
        private readonly CPU _cpu;

        public CorruptedDevice(CPU cpu) => _cpu = cpu;
        public string Name => "<unknown-device>";
        public short StartAddress => 0xFF;
        public void write(int address, int data)
        {
            throw new CorruptedMemoryException($"Instruction at address 0x{_cpu.State.curAddr:X4} accessed memory 0x{address:X4}. Memory could not be write.");
        }

        public int read(int address)
        {
            throw new CorruptedMemoryException($"Instruction at address 0x{_cpu.State.curAddr:X4} accessed memory 0x{address:X4}. Memory could not be read.");
        }

        public void Init()
        {
        }

        public void Shutdown()
        {
        }
    }
}