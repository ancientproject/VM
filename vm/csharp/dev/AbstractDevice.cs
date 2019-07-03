namespace vm.dev
{
    using System;
    using ancient.runtime;
    using ancient.runtime.exceptions;
    using Internal;

    public abstract class AbstractDevice : IDevice, IComparable
    {
        [StringAddress(0xE, 0xF)]
        public string name { get; private set; }
        public short startAddress { get; private set; }


        protected AbstractDevice(short address, string name)
        {
            startAddress = address;
            this.name = name;
        }

        public long this[long address]
        {
            get => read(address);
            set => write(address, value);
        }

        public virtual void write(long address, long data) =>
            (this as IDevice).WriteMemory(address, data);

        public virtual long read(long address) 
            => throw new DeviceReadonlyException();

        public virtual void warmUp() { }

        public virtual void shutdown() { }

        public int CompareTo(object obj)
        {
            if (!(obj is IDevice dev)) return 0;
            if (startAddress > dev.startAddress)
                return 1;
            return -1;
        }
        public override int GetHashCode() => 
            startAddress.GetHashCode() ^ 42 * 
            name.GetHashCode() ^ 42;


        protected CorruptedMemoryException ThrowMemoryWrite(ulong curAddr, long toAddr) => 
            new CorruptedMemoryException($"Instruction at address 0x{curAddr:X4} accessed memory 0x{toAddr:X}. Memory could not be write.");
        protected CorruptedMemoryException ThrowMemoryRead(ulong curAddr, long toAddr) => 
            new CorruptedMemoryException($"Instruction at address 0x{curAddr:X4} accessed memory 0x{toAddr:X}. Memory could not be read.");

    }
}