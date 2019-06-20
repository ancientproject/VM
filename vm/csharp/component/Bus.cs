namespace vm.component
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using dev;

    public class Bus
    {
        public State State { get; }
        public CPU Cpu { get; }

        public List<IDevice> Devices = new List<IDevice>();
        public int[] boundaries = new int[0];

        public Bus()
        {
            State = new State(this);
            Cpu = new CPU(this);
        }

        public void Write(short address, int data)
        {
            var device = Find(address);
            device.write((short)(address - device.StartAddress), data);
        }

        public int Read(short address)
        {
            var device = Find(address);
            return device.read((short)(address - device.StartAddress)) & 0xff;
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
        }

        public IDevice Find(int address)
        {
            var idx = Array.BinarySearch(boundaries, address);
            if (idx < 0) idx = -idx - 2;
            if (idx < 0) return new CorruptedDevice(Cpu);
            return Devices[idx];
        }
    }
}