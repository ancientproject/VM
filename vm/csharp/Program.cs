namespace vm
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using component;
    using dev;
    using dev.Internal;
    using ancient.runtime.emit;
    using MoreLinq;

    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "cpu_host";
            IntToCharConverter.Register<char>();

            var bus = new Bus();
            /* @0x11 */
            bus.State.tc = Environment.GetEnvironmentVariable("FLAME_TRACE") == "1";
            /* @0x12 */
            bus.State.ec = Environment.GetEnvironmentVariable("FLAME_ERROR") != "0";
            /* @0x13 */
            bus.State.km = Environment.GetEnvironmentVariable("FLAME_KEEP_MEMORY") == "1";
            /* @0x14 */
            bus.State.fw = Environment.GetEnvironmentVariable("FLAME_MEM_FAST_WRITE") == "1";

            bus.Add(new Terminal(0x1));
            bus.Add(new AdvancedTerminal(0x2));

            var core = bus.cpu;

            //uint[] page = {
            //    0xABCDEFE0,
            //    new loadi(0x1, 0x50),  // x
            //    new loadi(0x2, 0x72),  // y
            //    new loadi(0x3, 0x03),  // z
            //};

            //core.State.Load(page);

            //core.State.fetch();

            //if(true) {}
            //else 
            //core.State.Load(BIOS._GetILCode().ToArray());
            if (!args.Any())
                core.State.Load(0xB00B5000);
            else
            {
                
                var file = new FileInfo(args.First());
                if (file.Exists)
                {
                    var bytes = AncientAssembly.LoadFrom(file.FullName).GetILCode();
                    core.State.Load(CastFromBytes(bytes));
                }
                else
                    core.State.Load(0xB00B5000);
            }

            while (core.State.halt == 0)
            {
                await core.Step();
                await Task.Delay(1);
            }
        }

        public static ulong[] CastFromBytes(byte[] bytes)
        {
            if (bytes.Length % sizeof(ulong) == 0)
                return bytes.Batch(sizeof(ulong)).Select(x => BitConverter.ToUInt64(x.ToArray())).Reverse().ToArray();
            if (bytes.Length % sizeof(uint) == 0)
                return bytes.Batch(sizeof(uint)).Select(x => BitConverter.ToUInt64(x.ToArray())).Reverse().ToArray();
            throw new Exception("invalid offset file.");
        }

    }
}