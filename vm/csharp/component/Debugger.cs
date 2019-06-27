namespace vm.component
{
    using System;
    using System.Linq;
    using Ancient.Runtime.tools;

    public class Debugger
    {
        private readonly DebugSymbols debugSymbols;

        public delegate void Break(ushort offset, CPU cpu, DebugSymbols symbols);

        public event Break OnBreak;
        public Debugger(DebugSymbols debugSymbols)
        {
            this.debugSymbols = debugSymbols;
            OnBreak += Null;
        }

        public Break Null = (s, cpu, d) =>
        {
            Console.WriteLine("-=== BREAK ===-");
            Console.WriteLine(ObjectDumper.Dump(cpu.State));
            Console.WriteLine($"\n\n{d.symbols.FirstOrDefault(x => x.offset == s).line}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        };

        public void handleBreak(ushort offset, CPU cpu) => OnBreak?.Invoke(offset, cpu, debugSymbols);
    }
}