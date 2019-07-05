namespace vm.component
{
    using System.Collections.Generic;

    public class Stack
    {
        private readonly Bus _bus;
        private State state => _bus.State;
        private CPU getCPU() => _bus.cpu;
        public Stack(Bus bus) => _bus = bus;

        private readonly Stack<long> stack = new Stack<long>();

        
        public void push(long data)
        {
            if (state.SP >= 0x400)
                getCPU().halt(0xA2);

            state.southFlag = true;
            if (state.southFlag && _bus.Find(0x45).read(0xA3) == 0x1)
            {
                _bus.Find(0x0).write( 0x100 + state.SP++, data);
                return;
            }
            if (state.southFlag && _bus.Find(0x45).read(0xA2) == 0x1)
            {
                stack.Push(data);
                state.SP++;
                return;
            }

            switch (state.northFlag, state.eastFlag)
            {
                case (true, false):
                    push8(data);
                    return;
                case (true, true):
                    push4(data);
                    return;
                case (false, false):
                    push2(data);
                    return;
            }
            getCPU().halt(0xD6);
        }

        internal void push2(long data)
        {
            if (state.SP >= 0x400)
                getCPU().halt(0xA2);
            data &= 0xFF;
            state.SP++;
            stack.Push(data);
            _bus.Find(0x0).write(state.SP + 0x100, data);
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
            if (state.SP <= 0)
                getCPU().halt(0xA3);
            if (state.southFlag && _bus.Find(0x45).read(0xA3) == 0x1)
                return _bus.Find(0x0).read( 0x100 + state.SP--);
            if (state.southFlag && _bus.Find(0x45).read(0xA2) == 0x1)
            {
                state.SP--;
                return stack.Pop();
            }
            switch (state.northFlag, state.eastFlag)
            {
                case (true, false):
                    return pop8();
                case (true, true):
                    return pop4();
                case (false, false):
                    return pop2();
            }
            return getCPU().halt(0xD6);
        }

        internal long pop2()
        {
            if (state.SP <= 0)
                getCPU().halt(0xA3);
            if (state.SP >= 0x400)
                getCPU().halt(0xA2);
            return _bus.Find(0x0).read(--state.SP + 0x100);
        }

        internal long pop4() => pop2() | (pop2() << 8);
        internal long pop8() => pop4() | (pop4() << 16);
    }
}