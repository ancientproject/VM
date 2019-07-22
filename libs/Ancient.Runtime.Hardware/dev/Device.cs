namespace ancient.runtime
{
    using System;
    using System.Dynamic;
    using exceptions;
    using hardware;
    using Microsoft.CSharp.RuntimeBinder;

    public abstract class Device : IDevice
    {
        [StringAddress(0xE, 0xF)]
        public string name { get; private set; }
        public short startAddress { get; set; }


        protected Device(short address, string name)
        {
            startAddress = address;
            this.name = name;
        }

        public long this[long address]
        {
            get => (long)read(address);
            set => write(address, value);
        }

        #region read\write

        public virtual void write(long address, long data) =>
            write(address, (ulong) data);

        public virtual void write(long address, ulong data)
            =>(this as IDevice).WriteMemory(address, data);

        public virtual ulong read(long address) 
            => throw new DeviceReadonlyException();

        public virtual void warmUp() { }

        public virtual void shutdown() { }

        #endregion



        #region bus

        private dynamic connectedBus;

        private ulong currentAddress => connectedBus.State.curAddr;

        void IBusGate.assignBus(dynamic bus)
        {
            try
            {
                _ = $"{bus.State}";
                _ = $"{bus.cpu}";
            }
            catch (RuntimeBinderException e)
            {
                throw new CorruptedMemoryException(
                    $"Instruction at address <???> accessed memory <0x{startAddress:X}-?assignBus*>. Memory could not be write.", e);
            }
            connectedBus = bus;
        }

        #endregion

        



        protected CorruptedMemoryException ThrowMemoryWrite(ulong? curAddr, long? toAddr = null, Exception inner = null)
        {
            var current = curAddr switch { null => "???", _ => $"0x{curAddr:X4}" };
            var target  = toAddr   switch { null => $"0x{currentAddress:X4}", -1 => "???", _ => $"0x{toAddr:X4}" };


            return new CorruptedMemoryException(
                $"Instruction at address {current} accessed memory {target}. Memory could not be write.", inner);
        }

        protected CorruptedMemoryException ThrowMemoryRead(ulong? curAddr, long? toAddr = null, Exception inner = null)
        {
            var current = curAddr switch { null => "???", _ => $"0x{curAddr:X4}" };
            var target = toAddr   switch { null => $"0x{currentAddress:X4}", -1 => "???", _ => $"0x{toAddr:X4}" };

            return new CorruptedMemoryException(
                $"Instruction at address {current} accessed memory 0x{toAddr:X}. Memory could not be read.", inner);
        }


        #region def

        public int CompareTo(object obj)
        {
            if (!(obj is IDevice dev)) return 0;
            if (startAddress > dev.startAddress)
                return 1;
            return -1;
        }
        public override int GetHashCode()
        {
            var hash = unchecked(startAddress.GetHashCode() ^ 0x2A * name.GetHashCode() ^ 0x2A);
            return (hash & 0xFF) ^ ((hash >> 16) & 0xFF) ^ ((hash >> 8) & 0xFF) ^ ((hash >> 24) & 0xFF);
        }

        #endregion
    }
}