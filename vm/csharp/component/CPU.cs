namespace vm.component
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using ancient.runtime;
    using MoreLinq;
    using static System.Console;

    public class CPU : IHalter
    {
        private Bus _bus { get; }

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
                if(State.ec)
                    WriteLine(e.ToString());
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
            Error(AssociationHaltCode(reason, text));
            var l1 = _bus.State;
            Error($"L1 Cache, PC: 0x{l1.pc:X8}, OpCode: {l1.iid} [{l1.iid.getInstruction()}]");
            Error($"\t0x{l1.r1:X} 0x{l1.r2:X} 0x{l1.r3:X} 0x{l1.u1:X} 0x{l1.u2:X} 0x{l1.x1:X} 0x{l1.x2:X}");
            return reason;
        }

        public static string AssociationHaltCode(int reason, string text) => reason switch
        {
            0x0     => $"Power off",
            0x1     => $"Bootable sector not found.",
            0x2     => $"Reaching step limit exception.",
            0x4     => $"Bus offset conflict. {text}",
            0xC     => $"Divide by zero - YOU JUST CREATED A BLACK HOLE!",
            0xF     => $"Corrupted memory.",
            0xA1    => $"Overflow exception.",
            0xA2    => $"Overflow stack exception.",
            0xA3    => $"Low stack exception.",
            0xFC    => $"Invalid Opcode.",
            0xA9    => $"x87 float exception",
            0xBD    => $"Overflow heap memory exception",
            0xD6    => $"x9 segmentation fault",
            0xD7    => $"decompose opcode mode fault",
            0xD8    => $"Overflow argument opcode",
            0x77    => $"Unexpected end of executable memory",
            0xA18   => $"Signature fault, method '{text}' not found",
            0xA19   => $"Signature fault, method '{text}' not valid",
            0xA1A   => $"Signature fault, method '{text}' not static",
            0xA1B   => $"Signature fault, method '{text}' access denied",
            0xDE3   => $"Memory manage fault, {text}",
            0xA22   => $"Signature fault, '{text}'",
            0xFFFF  => $"Shift fault, {text}",
            _       => $"Unknown state 0x{reason:X}",
        };

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

        public override string ToString()
        {
            var haltText = State.halt != 0 ? "OFF" : "ON";
            return $"cpu [{haltText} - {State.step} ticks]";
        }
    }
}