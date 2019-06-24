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
    using flame.runtime;
    using flame.runtime.emit;
    using MoreLinq;
    using TrueColorConsole;
    using static System.Console;
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => { VTConsole.Disable(); };
            Title = "cpu_host";
            OutputEncoding = Encoding.UTF8;
            VTConsole.Enable();
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

            //core.State.Fetch();

            //if(true) {}
            //else 
            if (!args.Any())
                core.State.Load(0xB00B5000);
            else
            {
                
                var file = new FileInfo(args.First());
                if (file.Exists)
                {
                    var bytes = FlameAssembly.LoadFrom(file.FullName).GetILCode();
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

        public static uint[] CastFromBytes(byte[] bytes)
        {
            if(bytes.Length % sizeof(uint) != 0)
                throw new Exception("invalid offset file.");
            return bytes.Batch(sizeof(uint)).Select(x => BitConverter.ToUInt32(x.ToArray())).Reverse().ToArray();
        }

    }
}