namespace vm.dev
{
    using exceptions;

    public class CorruptedDevice : IDevice
    {
        public string Name { get; }
        public short StartAddress { get; }
        public void write(short address, int data)
        {
            throw new CorruptedMemoryException();
        }

        public int read(short address)
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