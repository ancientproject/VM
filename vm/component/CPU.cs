namespace vm.component
{
    using System;
    using System.Threading.Tasks;

    public class CPU
    {
        private readonly Bus _bus;

        public CPU(Bus bus) => _bus = bus;

        public State State => _bus.State;

        public async Task Step()
        {
            var nuget = State.Fetch();

            var n1 = Convert.ToUInt32(nuget);
            State.Accept(n1);
            State.Eval();
            await Task.CompletedTask;
        }

        public uint Compile(uint opcode, uint r1 = 0, uint r2 = 0, uint r3 = 0, uint u1 = 0, uint u2 = 0, uint x1 = 0)
        {
            return (opcode << 0x18) | (r1 << 0x14) | (r2 << 0x10) | (r3 << 0xC) | (u1 << 0x8) | (u2 << 4) | x1;
        }
    }
}