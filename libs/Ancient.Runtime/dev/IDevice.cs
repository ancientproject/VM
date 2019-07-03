namespace ancient.runtime
{
    using System;

    public interface IDevice : IComparable
    {
        string name { get; }
        short startAddress { get; }
        void write(long address, long data);
        long read(long address);

        long this[long address] { get; set; }

        void warmUp();
        void shutdown();
    }
}