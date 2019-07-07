namespace vm
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using ancient.runtime;
    using ancient.runtime.context;
    using component;
    using dev;
    using ancient.runtime.emit;
    using ancient.runtime.hardware;
    using ancient.runtime.tools;
    using MoreLinq;

    internal class Program
    {
        public static void InitializeProcess()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Console.Title = "cpu_host";
            IntToCharConverter.Register<char>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static void InitializeFlags(Bus bus)
        {
            /* @0x11 */
            bus.State.tc = AppFlag.GetVariable("VM_TRACE");
            /* @0x12 */
            bus.State.ec = AppFlag.GetVariable("VM_ERROR", true);
            /* @0x13 */
            bus.State.km = AppFlag.GetVariable("VM_KEEP_MEMORY");
            /* @0x14 */
            bus.State.fw = AppFlag.GetVariable("VM_MEM_FAST_WRITE");
        }

        public static void InitializeMemory(Bus bus, params string[] args)
        {
            if(bus.State.halt != 0)
                return;
            if (!args.Any())
                bus.State.Load("<chip>", 0xB00B5000);
            else
            {
                var nameFile = Path.GetFileNameWithoutExtension(args.First());
                var file = new FileInfo($"{nameFile}.dlx");
                var bios = new FileInfo($"{nameFile}.bios");
                var pdb = new FileInfo($"{nameFile}.pdb");

                if (AppFlag.GetVariable("VM_ATTACH_DEBUGGER") && pdb.Exists)
                    bus.AttachDebugger(new Debugger(DebugSymbols.Open(File.ReadAllBytes(pdb.FullName))));
                if (bios.Exists)
                {
                    var bytes = AncientAssembly.LoadFrom(bios.FullName).GetILCode();
                    bus.State.Load("<bios>",CastFromBytes(bytes));
                }
                if (file.Exists)
                {
                    var bytes = AncientAssembly.LoadFrom(file.FullName).GetILCode();
                    bus.State.Load("<exec>",CastFromBytes(bytes));
                }
                else
                    bus.State.Load("<chip>",0xB00B5000);
            }
        }

        public static async Task Main(string[] args)
        {
            InitializeProcess();
            var bus = new Bus();

            InitializeFlags(bus);

            //bus.Add(new Terminal(0x1));
            //bus.Add(new AdvancedTerminal(0x2));

            if(AppFlag.GetVariable("VM_TRACE"))
                DeviceLoader.OnTrace += Console.WriteLine;

            DeviceLoader.AutoGrub(bus.Add);

            InitializeMemory(bus, args);


            while (bus.State.halt == 0)
            {
                bus.cpu.Step();
                await Task.Delay(1);
            }

            bus.Unload();
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