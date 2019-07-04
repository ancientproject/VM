namespace vm.component
{
    using System.Collections.Generic;

    public class Stack
    {
        private readonly Bus _bus;
        private State getState() => _bus.State;
        private CPU getCPU() => _bus.cpu;
        public Stack(Bus bus) => _bus = bus;

        private readonly Stack<long> stack = new Stack<long>();

        
        public void push(long data)
        {
            switch (getState().northFlag, getState().eastFlag, getState().southFlag)
            {
                case (false, false, false):
                case (true, false, false):
                    push8(data);
                    break;
                case (true, true, false):
                    push4(data);
                    break;
                case (true, true, true):
                    push2(data);
                    break;
            }
            getCPU().halt(0xD6);
        }

        internal void push2(long data)
        {
            if (getState().SP <= 0)
                getCPU().halt(0xA3);
            if (getState().SP >= 0x400)
                getCPU().halt(0xA2);
            data &= 0xFF;
            getState().SP++;
            stack.Push(data);
            _bus.Find(0x0).write(getState().SP + 0x100, data);
        }
        internal void push4(long data)
        {
            push2((data >> 8) & 0xff);
            push2(data & 0xff);
        }
        internal void push8(long data)
        {
            push4((data >> 16) & 0xffff);
            push4(data & 0xffff);
        }

        public long pop()
        {
            switch (getState().northFlag, getState().eastFlag, getState().southFlag)
            {
                case (false, false, false):
                case (true, false, false):
                    return pop8();
                case (true, true, false):
                    return pop4();
                case (true, true, true):
                    return pop2();
            }
            return getCPU().halt(0xD6);
        }

        internal long pop2()
        {
            if (getState().SP <= 0)
                getCPU().halt(0xA3);
            if (getState().SP >= 0x400)
                getCPU().halt(0xA2);
            return _bus.Find(0x0).read(--getState().SP + 0x100);
        }

        internal long pop4() => pop2() | (pop2() << 8);
        internal long pop8() => pop4() | (pop4() << 16);
    }
}