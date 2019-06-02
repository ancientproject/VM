namespace vm.dev
{
    using flame.runtime.exceptions;

    public class CorruptedDevice : IDevice
    {
        public string Name { get; }
        public short StartAddress { get; }
        public void write(int address, int data)
        {
            throw new CorruptedMemoryException();
        }

        public int read(int address)
        {
            throw new CorruptedMemoryException();
        }

        public void Init()
        {
            throw new CorruptedMemoryException();
        }

        public void Shutdown()
        {
            throw new CorruptedMemoryException();
        }
    }
}