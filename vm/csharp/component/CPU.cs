namespace vm.component
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ancient.runtime;
    using MoreLinq;
    using static System.Console;
    public class CPU
    {
        private readonly Bus _bus;

        public CPU(Bus bus) => _bus = bus;

        public State State => _bus.State;

        public event Action<Exception> OnError;

        public async Task Step()
        {
            try
            {
                State.Accept(State.fetch());
                State.Eval();
            }
            catch (Exception e)
            {
                OnError?.Invoke(e);
            }
            await Task.CompletedTask;
        }

        public async Task StepToEnd(bool clearMemory = true)
        {
            foreach (var u in State.program)
            {
                await Step(u);
            }
            if (clearMemory)
            {
                State.program.Clear();
                State.pc = 0x0;
            }
        }

        public async Task Step(ulong address)
        {
            State.Accept(address);
            State.Eval();
            await Task.CompletedTask;
        }

        public int halt(int reason)
        {
            Error(Environment.NewLine);
            _bus.State.halt = 1;
            switch (reason)
            {
                case 0x0:
                    Error("HALT");
                    break;
                case 0x1:
                    Error("HALT: Bootable sector not found.");
                    break;
                case 0xC:
                    Error("HALT: Divide by zero - YOU JUST CREATED A BLACK HOLE!");
                    break;
                case 0xF:
                    Error($"HALT: Corrupted memory.");
                    break;
                case 0xA1:
                    Error($"HALT: Overflow exception.");
                    break;
                case 0xFC:
                    Error($"HALT: Invalid Opcode.");
                    break;
                case 0xA9:
                    Error($"HALT: x87 float exception");
                    break;
                default:
                    Error($"HALT: Unknown state 0x{reason:X}");
                    break;
            }
            var l1 = _bus.State.Registers.L1;
            Error($"L1 Cache, PC: 0x{l1.PC:X8}, OpCode: {l1.IID} [{l1.IID.getInstruction()}]");
            Error($"\t0x{l1.r1:X} 0x{l1.r2:X} 0x{l1.r3:X} 0x{l1.u1:X} 0x{l1.u2:X} 0x{l1.x1:X} 0x{l1.x2:X}");
            var l2 = _bus.State.Registers.L2;
            Error($"L2 Cache, PC: 0x{l2.PC:X8}, OpCode: {l2.IID} [{l2.IID.getInstruction()}]");
            Error($"\t0x{l2.r1:X} 0x{l2.r2:X} 0x{l2.r3:X} 0x{l2.u1:X} 0x{l2.u2:X} 0x{l2.x1:X} 0x{l2.x2:X}");
            return reason;
        }

        public string getStateOfCPU()
        {
            var str = new StringBuilder();
            var l1 = _bus.State.Registers.L1;
            var l2 = _bus.State.Registers.L2;

            str.AppendLine($"L1 Cache, PC: 0x{l1.PC:X8}, OpCode: {l1.IID} [{l1.IID.getInstruction()}]");
            str.AppendLine($"\t0x{l1.r1:X} 0x{l1.r2:X} 0x{l1.r3:X} 0x{l1.u1:X} 0x{l1.u2:X} 0x{l1.x1:X} 0x{l1.x2:X}");
            str.AppendLine($"L2 Cache, PC: 0x{l2.PC:X8}, OpCode: {l2.IID} [{l2.IID.getInstruction()}]");
            str.AppendLine($"\t0x{l2.r1:X} 0x{l2.r2:X} 0x{l2.r3:X} 0x{l2.u1:X} 0x{l2.u2:X} 0x{l2.x1:X} 0x{l2.x2:X}");

            str.AppendLine("Table of memory:");

            foreach (var v in _bus.State.mem.Batch(4).Select(x => x.ToArray()))
                str.AppendLine($"\t 0x{v[0]:X4},0x{v[1]:X4},0x{v[2]:X4},0x{v[3]:X4}");

            str.AppendLine();

            str.AppendLine("bus:");

            foreach (var v in _bus.Devices)
                str.AppendLine($"\tDevice: {v.name}, 0x{v.startAddress:X8}");

            str.AppendLine();


            return str.ToString();
        }
        private void Error(string str)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine(str);
            ForegroundColor = ConsoleColor.White;
        }


        public void ResetCache() => _bus.State.Registers.L1 = new Cache();
        public void ResetMemory() => _bus.State.program.Clear();
    }
}