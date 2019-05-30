namespace vm.dev
{
    using System.Threading.Tasks;

    public interface IDevice
    {
        string Name { get; }
        short StartAddress { get; }
        void write(short address, int data);
        int read(short address);


        void Init();
        void Shutdown();
    }
}