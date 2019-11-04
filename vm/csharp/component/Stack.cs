namespace vm.component
{
    using System.Collections.Generic;
    public sealed class Stack
    {
        internal IHalting __halter;

        private readonly Bus _bus;
        private IHalting _cpuHalter => __halter ?? _bus.cpu;
        private State _provider => _bus.State;
        public Stack(Bus bus) => _bus = bus;

        internal readonly Stack<ulong> cells = new Stack<ulong>();

        
        public void push(ulong data)
        {
            if (_provider.SP >= 0x400)
                _cpuHalter.halt(0xA2);

            if (_provider.southFlag && _bus.find(0x45).read(0xA3) == 0x1)
            {
                _bus.find(0x0).write(  _provider.SP+ 0x100, data);
                _provider.SP++;
                return;
            }
            if (_provider.southFlag && _bus.find(0x45).read(0xA2) == 0x1)
            {
                cells.Push(data);
                _provider.SP++;
                return;
            }
            push16(data);
        }

        internal void push2(ulong data)
        {
            if (_provider.SP >= 0x400)
                _cpuHalter.halt(0xA2);
            data &= 0xFF;
            _provider.SP++;
            _bus.find(0x0).write(_provider.SP + 0x100, data);
        }
        internal void push4(ulong data)
        {
            push2((data >> 8) & 0xff);
            push2(data & 0xff);
        }
        internal void push8(ulong data)
        {
            push4((data >> 16) & 0xffff);
            push4(data & 0xffff);
        }
        internal void push16(ulong data)
        {
            push8((data >> 32) & 0xffff_ffff);
            push8(data & 0xffff_ffff);
        }

        public ulong pop()
        {
            if (_provider.SP <= 0)
                _cpuHalter.halt(0xA3);
            if (_provider.southFlag && _bus.find(0x45).read(0xA3) == 0x1)
            {
                _provider.SP--;
                return _bus.find(0x0).read(_provider.SP + 0x100);
            }
            if (_provider.southFlag && _bus.find(0x45).read(0xA2) == 0x1)
            {
                _provider.SP--;
                return cells.Pop();
            }
            return pop16();
        }

        internal ulong pop2()
        {
            if (_provider.SP <= 0)
                _cpuHalter.halt(0xA3);
            if (_provider.SP >= 0x400)
                _cpuHalter.halt(0xA2);
            var res = _bus.find(0x0).read(_provider.SP--+ 0x100);
            return res;
        }

        internal ulong pop4() => pop2() | (pop2() << 8);
        internal ulong pop8() =>  pop4() | (pop4() << 16);
        internal ulong pop16() => pop8() | (pop8() << 32) ;
    }
}