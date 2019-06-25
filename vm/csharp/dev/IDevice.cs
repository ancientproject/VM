namespace vm.dev
{
    public interface IDevice
    {
        string Name { get; }
        short StartAddress { get; }
        void write(int address, int data);
        int read(int address);

        int this[int address] { get; set; }

        void Init();
        void Shutdown();
    }
}