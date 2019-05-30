namespace flame.compiler
{
    using System.Drawing;
    using System.Linq;
    using static TrueColorConsole.VTConsole;
    using static _term;

    internal class Program
    {
        public static void Main(string[] args)
        {
            Enable();
            CursorSetVisibility(false);
            CursorSetBlinking(false);

            WriteLine($"Flame Assembler Compiler version 0.0.0.0 (default)", Color.Gray);
            WriteLine($"Copyright (C) Yuuki Wesp.\n\n", Color.Gray);

            if (!args.Any())
            {
                Fatal($"FC0004", "No source files specified.");
                return;
            }
        }
    }
}
