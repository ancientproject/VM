namespace vm.component
{
    using System.Collections.Generic;
    using System.Linq;
    using Ancient.ProjectSystem;
    using ancient.runtime;
    using MoreLinq;

    public class Bus
    {
        public State State { get; set; }
        public CPU cpu { get; }
        public DeviceScheme scheme { get; }

        public Debugger debugger { get; set; }

        public List<IDevice> Devices = new List<IDevice>(16);

        public bool WarmUpDevices { get; set; } = AppFlag.GetVariable("VM_WARMUP_DEV", true);
        public bool ShutdownDevices { get; set; } = AppFlag.GetVariable("VM_SHUTDOWN_DEV", true);

        public Bus()
        {
            State = new State(this);
            cpu = new CPU(this);

            scheme = DeviceScheme.Default;

            Add(new BIOS(cpu,this));
            Add(new Memory(0x0, 0x90000, this));
        }

        public void Add(IDevice device)
        {
            device.startAddress = scheme.getOffsetByDevice(device.name, device.startAddress);

            if (Devices.FirstOrDefault(x => x.startAddress == device.startAddress) != null)
            {
                cpu.halt(0x4, $"<0x{device.startAddress:X}>");
                return;
            }
            Devices.Add(device);
            Devices.Sort();
            device.assignBus(this);
            if(WarmUpDevices) device.warmUp();
        }

        public IDevice Find(int address) 
            => Devices.FirstOrDefault(x => x.startAddress == address) 
               ?? new CorruptedDevice(cpu);

        internal void AttachDebugger(Debugger dbg) => this.debugger = dbg;

        public void Unload()
        {
            if(ShutdownDevices)
                Devices.Pipe(x => x.shutdown()).ToArray();
        }

        public override string ToString() => $"bus [{Devices.Count:00}/{Devices.Capacity:00}]";
    }
}