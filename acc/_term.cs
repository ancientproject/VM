namespace flame.compiler
{
    using System;
    using System.Drawing;
    using emit;
    using static TrueColorConsole.VTConsole;
    internal class _term
    {
        private static readonly object Guarder = new object();

        public static void Trace(string message)
        {
            lock (Guarder)
            {
                if (!IsSupported || !IsEnabled)
                {
                    Console.WriteLine($"trace: {message}");
                    return;
                }
                WriteLine($"trace: {message}", Color.Gray);
            }
        }
        public static void Warn(Warning keyCode, string message)
        {
            lock (Guarder)
            {
                if (!IsSupported || !IsEnabled)
                {
                    Console.WriteLine($"warning {keyCode}: {message}");
                    return;
                }
                Write($"warning {keyCode.Format()}", Color.Orange);
                WriteLine($": {message}", Color.White);
            }
        }
        public static void Error(Warning keyCode, string message)
        {
            lock (Guarder)
            {
                if (!IsSupported || !IsEnabled)
                {
                    Console.WriteLine($"error {keyCode}: {message}");
                    return;
                }
                Write($"error {keyCode.Format()}", Color.Red);
                WriteLine($": {message}", Color.White);
            }
        }
    }
}