namespace vm.component
{
    using System;
    using System.Diagnostics;
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

        public void Step()
        {
            try
            {
                State.Accept(State.fetch());
                State.Eval();
            }
            catch (Exception e)
            {
                OnError?.Invoke(e);
                halt(0xFFFF, e.Message.ToLowerInvariant());
                Trace.TraceError(e.ToString());
            }
        }


        public void Step(ulong address)
        {
            State.Accept(address);
            State.Eval();
        }

        public int halt(int reason, string text = "")
        {
            if (State.halt != 0) return reason;
            Error(Environment.NewLine);
            _bus.State.halt = 1;
            switch (reason)
            {
                case 0x0:
                    Error("HALT: Power off");
                    break;
                case 0x1:
                    Error($"HALT: Bootable sector not found.");
                    break;
                case 0xC:
                    Error($"HALT: Divide by zero - YOU JUST CREATED A BLACK HOLE!");
                    break;
                case 0xF:
                    Error($"HALT: Corrupted memory.");
                    break;
                case 0xA1:
                    Error($"HALT: Overflow exception.");
                    break;
                case 0xA2:
                    Error($"HALT: Overflow stack exception.");
                    break;
                case 0xA3:
                    Error($"HALT: Low stack exception.");
                    break;
                case 0xFC:
                    Error($"HALT: Invalid Opcode.");
                    break;
                case 0xA9:
                    Error($"HALT: x87 float exception");
                    break;
                case 0xBD:
                    Error($"HALT: Overflow heap memory exception");
                    break;
                case 0xD6:
                    Error($"HALT: x9 segmentation fault");
                    break;
                case 0xFFFF:
                    Error($"HALT: shift fault, {text}");
                    break;
                default:
                    Error($"HALT: Unknown state 0x{reason:X}");
                    break;
            }
            var l1 = _bus.State;
            Error($"L1 Cache, PC: 0x{l1.pc:X8}, OpCode: {l1.iid} [{l1.iid.getInstruction()}]");
            Error($"\t0x{l1.r1:X} 0x{l1.r2:X} 0x{l1.r3:X} 0x{l1.u1:X} 0x{l1.u2:X} 0x{l1.x1:X} 0x{l1.x2:X}");
            return reason;
        }

        public string getStateOfCPU()
        {
            var str = new StringBuilder();
            var l1 = _bus.State;

            str.AppendLine($"L1 Cache, PC: 0x{l1.pc:X8}, OpCode: {l1.iid} [{l1.iid.getInstruction()}]");
            str.AppendLine($"\t0x{l1.r1:X} 0x{l1.r2:X} 0x{l1.r3:X} 0x{l1.u1:X} 0x{l1.u2:X} 0x{l1.x1:X} 0x{l1.x2:X}");

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
            Trace.TraceError(str);
            ForegroundColor = ConsoleColor.Red;
            WriteLine(str);
            ForegroundColor = ConsoleColor.White;
        }
    }
}