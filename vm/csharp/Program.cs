namespace vm
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ancient.compiler.emit;
    using ancient.runtime;
    using ancient.runtime.context;
    using component;
    using ancient.runtime.emit;
    using ancient.runtime.hardware;
    using ancient.runtime.tools;
    using MoreLinq;
    using Pastel;
    using ancient.compiler.tokens;
    using ancient.runtime.emit.sys;
    using Sprache;

    internal class Program
    {
        public static void InitializeProcess()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Console.Title = "vm_host";
            IntConverter.Register<char>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Console.OutputEncoding = Encoding.Unicode;
        }

        public static void InitializeFlags(Bus bus)
        {
            /* @0x11 */
            bus.State.tc = AppFlag.GetVariable("VM_TRACE", true);
            /* @0x12 */
            bus.State.ec = AppFlag.GetVariable("VM_ERROR", false);
            /* @0x13 */
            bus.State.km = AppFlag.GetVariable("VM_KEEP_MEMORY");
            /* @0x14 */
            bus.State.fw = AppFlag.GetVariable("VM_MEM_FAST_WRITE");
        }

        private static void HandleDebugger()
        {
            Console.WriteLine("Waiting for debugger to attach".Pastel(Color.Red));
            while (!System.Diagnostics.Debugger.IsAttached)
                Thread.Sleep(100);
            Console.WriteLine("Debugger attached".Pastel(Color.Green));
        }

        public static void InitializeMemory(Bus bus, params string[] args)
        {
            if (bus.State.halt != 0)
                return;
            if (!args.Any())
            {
                //var ev = new SignatureEvolve(new List<string> {"u32", "f32"}, "FuckingXuy");

                //(ev as IEvolveEvent).OnBuild();

                //bus.State.Load("<exec>", ev
                //    .GetInstructions().Select(x =>(ulong)x).ToArray());
                var mem = new Instruction[]
                {
                    new ldx(0x11, 0x1), 
                    new sig("test1", 0, "void"), new lpstr("test1"),
                    new ldi(0x0, 0x5), 
                    new __static_extern_call("sys->DumpCallStack()"), 
                    new ret(),
                    new nop(),
                    new sig("test2", 0, "void"), new lpstr("test2"),
                    new ldi(0x1, 0x6), 
                    new call_i("test1()"), 
                    new ret(),
                    new nop(),
                    new sig("test3", 0, "void"), new lpstr("test3"),
                    new call_i("test2()"), 
                    new mul(0x3, 0x0, 0x1), 
                    new ret(),
                    new nop(),
                    new call_i("test3()")
                };
                bus.State.Load("<exec>", mem.Select(x => (ulong)x).ToArray());
                bus.State.LoadMeta(mem.SelectMany(x => x.GetMetaDataILBytes()).ToArray());
                //bus.State.Load("<chip>", 0xB00B50000);
            }
            else
            {
                var nameFile = Path.Combine(Path.GetDirectoryName(args.First()), Path.GetFileNameWithoutExtension(args.First()));
                var file = new FileInfo($"{nameFile}.dlx");
                var bios = new FileInfo($"{nameFile}.bios");
                var pdb = new FileInfo($"{nameFile}.pdb");

                if (AppFlag.GetVariable("VM_ATTACH_DEBUGGER") && pdb.Exists)
                    bus.AttachDebugger(new Debugger(DebugSymbols.Open(File.ReadAllBytes(pdb.FullName))));
                if (bios.Exists)
                {
                    var asm = AncientAssembly.LoadFrom(bios.FullName);
                    var bytes = asm.GetILCode();
                    var meta = asm.GetMetaILCode();
                    bus.State.Load("<bios>", CastFromBytes(bytes));
                    bus.State.LoadMeta(meta);
                }
                if (file.Exists)
                {
                    var asm = AncientAssembly.LoadFrom(file.FullName);
                    var bytes = asm.GetILCode();
                    var meta = asm.GetMetaILCode();
                    bus.State.Load("<exec>", CastFromBytes(bytes));
                    bus.State.LoadMeta(meta);
                }
                else
                    bus.State.Load("<chip>", 0xB00B5000);
            }
        }

        public static async Task Main(string[] args)
        {
            if (AppFlag.GetVariable("MANAGED_DEBUGGER_WAIT"))
                HandleDebugger();

            InitializeProcess();
            var bus = new Bus();

            InitializeFlags(bus);

            if (AppFlag.GetVariable("VM_TRACE"))
                DeviceLoader.OnTrace += Console.WriteLine;

            Module.Boot(bus);
            InternalVMFunctions.Boot(bus);
            DeviceLoader.Boot(bus.Add);

            if (AppFlag.GetVariable("REPL"))
            {
                Console.WriteLine("@ Ancient VM Interactive @".Pastel(Color.Green));
                InteractiveConstruction(bus);
            }
            else
                InitializeMemory(bus, args);

            while (bus.State.halt == 0) 
                bus.cpu.Step();
            bus.Unload();
        }

        public static ulong[] CastFromBytes(byte[] bytes)
        {
            if (bytes.Length % sizeof(ulong) == 0)
                return bytes.Batch(sizeof(ulong)).Select(x => BitConverter.ToUInt64(x.Reverse().ToArray())).Reverse().ToArray();
            throw new Exception("invalid offset file.");
        }

        public static void InteractiveConstruction(Bus bus)
        {
            bus.State.km = true;
            bus.State.fw = true;
            Console.WriteLine($"keem memory has {"enabled".Pastel(Color.GreenYellow)}");
            Console.WriteLine($"fast write has {"enabled".Pastel(Color.GreenYellow)}");
            while (true)
            {
                Console.Write("> ".Pastel(Color.Gray));
                var input = Console.ReadLine();
                if (input is null)
                    continue;
                if (input == "exit")
                    return;

                if (input.StartsWith("0x"))
                {
                    if (!ulong.TryParse(input.Remove(0, 2), NumberStyles.HexNumber, null, out _))
                    {
                        Console.WriteLine("Invalid op code".Pastel(Color.Red));
                        continue;
                    }

                    // shit))0
                    if (input.Length < 11)
                    {
                        var need = 12;
                        var current = input.Length;
                        var diff = (need - current);
                        input = $"{input}{new string('0', diff)}";
                    }

                    var decoded = ulong.Parse(input.Remove(0, 2), NumberStyles.HexNumber);
                    bus.State.halt = 0;
                    bus.State.Append(decoded);
                    bus.cpu.Step();
                }
                else
                {
                    var token = new AssemblerSyntax().Parser.Parse(input);
                    if (token is InstructionExpression instruction)
                    {
                        bus.State.halt = 0;
                        bus.State.Append(instruction.Instruction.Assembly());
                        bus.cpu.Step();
                    }

                    if (token is ErrorCompileToken error)
                    {
                        var exp = error.ErrorResult.Expectations.First();
                        var title = $"{error.ErrorResult.getWarningCode().To<string>().Pastel(Color.Orange)}";
                        var message = $"character '{exp}' expected".Pastel(Color.Orange);
                        Console.WriteLine($" :: {title} - {message}");
                    }
                }
               
            }
        }
    }
}