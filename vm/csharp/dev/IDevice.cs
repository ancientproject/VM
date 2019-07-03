namespace vm.dev
{
    public interface IDevice
    {
        string Name { get; }
        short StartAddress { get; }
        void write(long address, long data);
        long read(long address);

        long this[long address] { get; set; }

        void WarmUp();
        void Shutdown();
    }
}