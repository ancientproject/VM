namespace vm.component
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ancient.runtime;
    using dev;
    using MoreLinq;

    public class Bus
    {
        public State State { get; }
        public CPU cpu { get; }

        public Debugger debugger { get; set; }

        public List<IDevice> Devices = new List<IDevice>();
        public int[] boundaries = new int[0];

        public bool WarmUpDevices { get; set; } = AppFlag.GetVariable("VM_WARMUP_DEV", true);
        public bool ShutdownDevices { get; set; } = AppFlag.GetVariable("VM_SHUTDOWN_DEV", true);

        public Bus()
        {
            State = new State(this);
            cpu = new CPU(this);
            Add(new BIOS(cpu,this));
        }

        public void Add(IDevice device)
        {
            Devices.Add(device);
            var newBoundaries = new int[boundaries.Length + 1];
            Array.Copy(boundaries, 0, newBoundaries, 1, boundaries.Length);
            newBoundaries[0] = device.StartAddress;
            Array.Sort(newBoundaries);
            Devices.Sort();
            boundaries = newBoundaries;
            if(WarmUpDevices)
                device.WarmUp();
        }

        public IDevice Find(int address)
        {
            var idx = Array.BinarySearch(boundaries, address);
            if (idx < 0) idx = -idx - 2;
            if (idx < 0) return new CorruptedDevice(cpu);
            return Devices[idx];
        }

        internal void AttachDebugger(Debugger dbg) => this.debugger = dbg;

        public void Unload()
        {
            if(ShutdownDevices)
                Devices.Pipe(x => x.Shutdown()).ToArray();
        }
    }
}