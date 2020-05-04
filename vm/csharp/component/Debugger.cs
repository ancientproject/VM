namespace vm.component
{
    using System;
    using System.Linq;
    using ancient.runtime.tools;
    using Newtonsoft.Json;

    public class Debugger
    {
        public static readonly Debugger Default = new Debugger(null);

        private readonly DebugSymbols debugSymbols;

        public delegate void Break(ushort offset, CPU cpu, DebugSymbols symbols);

        public event Break OnBreak;
        public Debugger(DebugSymbols debugSymbols)
        {
            this.debugSymbols = debugSymbols;
            if(debugSymbols != null) OnBreak += Null;
        }

        public Break Null = (s, cpu, d) =>
        {
            Console.WriteLine("-=== BREAK ===-");
            Console.WriteLine(JsonConvert.SerializeObject(cpu.State, Formatting.Indented));
            Console.WriteLine($"\n\n{d.symbols.FirstOrDefault(x => x.offset == s).line}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        };

        public void handleBreak(ushort offset, CPU cpu) => OnBreak?.Invoke(offset, cpu, debugSymbols);

        public override string ToString()
        {
            return debugSymbols is null ? 
                $"debugger [not_connected] [0 symbols]" : 
                $"debugger [connected] [{debugSymbols.symbols.Count} symbols]";
        }
    }
}