namespace vm.dev
{
    using System;
    using ancient.runtime.exceptions;
    using Internal;

    public abstract class AbstractDevice : IDevice, IComparable
    {
        [StringAddress(0xE, 0xF)]
        public string Name { get; private set; }
        public short StartAddress { get; private set; }


        protected AbstractDevice(short address, string name)
        {
            StartAddress = address;
            Name = name;
        }

        public int this[int address]
        {
            get => read(address);
            set => write(address, value);
        }

        public virtual void write(int address, int data) =>
            (this as IDevice).WriteMemory(address, data);

        public virtual int read(int address) 
            => throw new DeviceReadonlyException();

        public virtual void Init() { }

        public virtual void Shutdown() { }

        public int CompareTo(object obj)
        {
            if (!(obj is IDevice dev)) return 0;
            if (StartAddress > dev.StartAddress)
                return 1;
            return -1;
        }
        public override int GetHashCode() => 
            StartAddress.GetHashCode() ^ 42 * 
            Name.GetHashCode() ^ 42;
    }
}