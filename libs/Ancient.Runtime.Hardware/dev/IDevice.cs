namespace ancient.runtime
{
    using System;

    public interface IDevice : IComparable, IBusGate
    {
        string name { get; }
        short startAddress { get; set; }
        void write(long address, long data);
        void write(long address, ulong data);
        ulong read(long address);

        long this[long address] { get; set; }

        void warmUp();
        void shutdown();
    }
    public interface IBusGate
    {
        void assignBus(dynamic bus);
    }
}