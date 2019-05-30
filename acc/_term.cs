namespace flame.compiler
{
    using System;
    using System.Drawing;
    using static TrueColorConsole.VTConsole;
    internal class _term
    {
        private static readonly object Guarder = new object();

        public static void Warn(string message) => Warn("", message);

        public static void Warn(string keyCode, string message)
        {
            lock (Guarder)
            {
                if (!IsSupported || !IsEnabled)
                {
                    Console.WriteLine($"warning {keyCode}: {message}");
                    return;
                }
                Write("warning ", Color.Orange);
                Write(keyCode, Color.Orange);
                Write(": ", Color.White);
                Write(message, Color.White);
            }
        }
        public static void Error(string keyCode, string message)
        {
            lock (Guarder)
            {
                if (!IsSupported || !IsEnabled)
                {
                    Console.WriteLine($"error {keyCode}: {message}");
                    return;
                }
                Write("warning ", Color.Red);
                Write(keyCode, Color.Red);
                Write(": ", Color.White);
                Write(message, Color.White);
            }
        }
    }
}