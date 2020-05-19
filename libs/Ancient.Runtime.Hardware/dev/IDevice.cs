namespace ancient.runtime
{
    using System;
    using @base;

    public interface IDevice : IComparable, IBusGate, IMemoryRange
    {
        string name { get; }
        short startAddress { get; set; }

        long this[long address] { get; set; }

        void warmUp();
        void shutdown();
    }
    public interface IBusGate
    {
        void assignBus(dynamic bus);
        dynamic getBus();
    }
}