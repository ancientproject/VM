namespace vm
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using component;
    using dev;
    using dev.Internal;
    using MoreLinq;
    using static System.Console;
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Title = "cpu_host";

            IntToCharConverter.Register<char>();

            var bus = new Bus();

            bus.Add(new Terminal(0x1));

            var core = bus.Cpu;

            core.State.tc = Environment.GetEnvironmentVariable("FLAME_TRACE") == "1";

            if (!args.Any())
                core.State.Load(new uint[] { 0xABCDEFE, 0xDEAD });
            else
            {
                var file = new FileInfo(args.First());
                if (file.Exists)
                {
                    var bytes = File.ReadAllBytes(file.FullName);
                    core.State.Load(CastFromBytes(bytes));
                }
            }

            while (core.State.halt == 0)
            {
                await core.Step();
                await Task.Delay(10);
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