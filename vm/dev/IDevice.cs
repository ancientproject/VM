namespace vm.dev
{
    using System.Threading.Tasks;

    public interface IDevice
    {
        string Name { get; }
        short StartAddress { get; }
        void write(int address, int data);
        int read(int address);


        void Init();
        void Shutdown();
    }
}