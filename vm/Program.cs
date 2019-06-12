namespace vm
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using component;
    using dev;
    using dev.Internal;
    using flame.runtime.emit;
    using MoreLinq;
    using TrueColorConsole;
    using static System.Console;
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Title = "cpu_host";
            OutputEncoding = Encoding.UTF8;
            ;
            WriteLine("\u2876\u2877\u2878");
            VTConsole.Enable();
            IntToCharConverter.Register<char>();

            var bus = new Bus();

            bus.Add(new Terminal(0x1));
            bus.Add(new AdvancedTerminal(0x2));

            var core = bus.Cpu;

            core.State.tc = Environment.GetEnvironmentVariable("FLAME_TRACE") == "1";

            if (!args.Any())
                core.State.Load(BIOS.GetILCode().ToArray());
            else
            {
                var file = new FileInfo(args.First());
                if (file.Exists)
                {
                    var bytes = FlameAssembly.LoadFrom(file.FullName).GetILCode();
                    core.State.Load(CastFromBytes(bytes));
                }
            }

            while (core.State.halt == 0)
            {
                await core.Step();
                await Task.Delay(1);
            }
        }

        public static uint[] CastFromBytes(byte[] bytes)
        {
            if(bytes.Length % sizeof(uint) != 0)
                throw new Exception("invalid offset file.");
            return bytes.Batch(sizeof(uint)).Select(x => BitConverter.ToUInt32(x.ToArray())).ToArray();
        }

    }
}