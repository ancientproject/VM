namespace vm.component
{
    using System;
    using System.Threading.Tasks;
    using flame.runtime;
    using static System.Console;
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

        public void Halt(byte code)
        {
            _bus.State.halt = 1;
            switch (code)
            {
                case 0x0:
                    Error("HALT");
                    break;
                case 0x1:
                    Error("HALT: Bootable sector not found.");
                    break;
                case 0xC:
                    Error("HALT: Divide by zero.");
                    break;
                case 0xB:
                    Error("HALT: Divide by zero.");
                    break;
                case 0xF:
                    Error($"HALT: Corrupted memory.");
                    break;
                default:
                    Error($"HALT: Unknown state 0x{code:X}");
                    break;
            }
            var l1 = _bus.State.Registers.L1;
            Error($"L1 Cache, PC: 0x{l1.PC:X8}, OpCode: {l1.IID} [{l1.IID.getInstruction()}]");
            Error($"\t0x{l1.r1:X} 0x{l1.r2:X} 0x{l1.r3:X} 0x{l1.u1:X} 0x{l1.u2:X} 0x{l1.x1:X} 0x{l1.x2:X}");
            var l2 = _bus.State.Registers.L2;
            Error($"L2 Cache, PC: 0x{l2.PC:X8}, OpCode: {l2.IID} [{l2.IID.getInstruction()}]");
            Error($"\t0x{l2.r1:X} 0x{l2.r2:X} 0x{l2.r3:X} 0x{l2.u1:X} 0x{l2.u2:X} 0x{l2.x1:X} 0x{l2.x2:X}");
        }
        private void Error(string str)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine(str);
            ForegroundColor = ConsoleColor.White;
        }

    }
}