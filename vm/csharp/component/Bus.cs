namespace vm.component
{
    using System;
    using System.Collections.Generic;
    using dev;

    public class Bus
    {
        public State State { get; }
        public CPU cpu { get; }

        public Debugger debugger { get; set; }

        public List<IDevice> Devices = new List<IDevice>();
        public int[] boundaries = new int[0];

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
            device.Init();
        }

        public IDevice Find(int address)
        {
            var idx = Array.BinarySearch(boundaries, address);
            if (idx < 0) idx = -idx - 2;
            if (idx < 0) return new CorruptedDevice(cpu);
            return Devices[idx];
        }

        internal void AttachDebugger(Debugger dbg) => this.debugger = dbg;
    }
}