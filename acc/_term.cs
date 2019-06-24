namespace flame.compiler
{
    using System;
    using System.Drawing;
    using emit;
    using Pastel;
    using static System.Console;
    internal class _term
    {
        private static readonly object Guarder = new object();

        public static void Trace(string message)
        {
            lock (Guarder)
            {
                WriteLine($"trace: {message}".Pastel( Color.Gray));
            }
        }

        public static void Success(string message)
        {
            lock (Guarder)
            {
                Write("[");
                Write($"SUCCESS".Pastel(Color.YellowGreen));
                Write("]: ");
                WriteLine($" {message}");
            }
        }
        public static void Warn(Warning keyCode, string message)
        {
            lock (Guarder)
            {
                Write("[");
                Write($"WARN".Pastel(Color.Orange));
                Write("]: ");
                Write($"{keyCode.Format()}".Pastel(Color.Orange));
                WriteLine($" {message}");
            }
        }
        public static void Error(Warning keyCode, string message)
        {
            lock (Guarder)
            {
                Write("[");
                Write($"ERROR".Pastel(Color.Red));
                Write("]: ");
                Write($"{keyCode.Format()}".Pastel(Color.Red));
                WriteLine($" {message}");
            }
        }
    }
}