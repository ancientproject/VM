namespace vm.component
{
    using System.Collections.Generic;
    public sealed class Stack
    {
        private readonly Bus _bus;
        private readonly IHalting _cpuHalter;
        private readonly State _provider;
        public Stack(Bus bus)
        {
            _bus = bus;
            _cpuHalter = bus.cpu;
            _provider = bus.State;
        }

        public Stack(Bus bus, IHalting halt, State provider)
        {
            _bus = bus;
            _cpuHalter = halt;
            _provider = provider;
        }

        internal readonly Stack<long> cells = new Stack<long>();

        
        public void push(long data)
        {
            if (_provider.SP >= 0x400)
                _cpuHalter.halt(0xA2);

            if (_provider.southFlag && _bus.Find(0x45).read(0xA3) == 0x1)
            {
                _bus.Find(0x0).write(  _provider.SP+ 0x100, data);
                _provider.SP++;
                return;
            }
            if (_provider.southFlag && _bus.Find(0x45).read(0xA2) == 0x1)
            {
                cells.Push(data);
                _provider.SP++;
                return;
            }
            push16(data);
        }

        internal void push2(long data)
        {
            if (_provider.SP >= 0x400)
                _cpuHalter.halt(0xA2);
            data &= 0xFF;
            _provider.SP++;
            _bus.Find(0x0).write(_provider.SP + 0x100, data);
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
        internal void push16(long data)
        {
            push8((data >> 32) & 0xffff_ffff);
            push8(data & 0xffff_ffff);
        }

        public long pop()
        {
            if (_provider.SP <= 0)
                _cpuHalter.halt(0xA3);
            if (_provider.southFlag && _bus.Find(0x45).read(0xA3) == 0x1)
            {
                _provider.SP--;
                return _bus.Find(0x0).read(_provider.SP + 0x100);
            }
            if (_provider.southFlag && _bus.Find(0x45).read(0xA2) == 0x1)
            {
                _provider.SP--;
                return cells.Pop();
            }
            return pop16();
        }

        internal long pop2()
        {
            if (_provider.SP <= 0)
                _cpuHalter.halt(0xA3);
            if (_provider.SP >= 0x400)
                _cpuHalter.halt(0xA2);
            var res = _bus.Find(0x0).read(_provider.SP--+ 0x100);
            return res;
        }

        internal long pop4() => pop2() | (pop2() << 8);
        internal long pop8() =>  pop4() | (pop4() << 16);
        internal long pop16() => pop8() | (pop8() << 32) ;
    }
}