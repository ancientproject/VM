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

        public static void Success(string message)
        {
            lock (Guarder)
            {
                if (!IsSupported || !IsEnabled)
                {
                    Console.WriteLine($"[SUCCESS]: {message}");
                    return;
                }
                Write("[", Color.White);
                Write($"SUCCESS", Color.YellowGreen);
                Write("]: ", Color.White);
                WriteLine($" {message}", Color.White);
            }
        }
        public static void Warn(Warning keyCode, string message)
        {
            lock (Guarder)
            {
                if (!IsSupported || !IsEnabled)
                {
                    Console.WriteLine($"[WARN]: {keyCode} {message}");
                    return;
                }
                Write("[", Color.White);
                Write($"WARN", Color.Orange);
                Write("]: ", Color.White);
                Write($"{keyCode.Format()}", Color.Orange);
                WriteLine($" {message}", Color.White);
            }
        }
        public static void Error(Warning keyCode, string message)
        {
            lock (Guarder)
            {
                if (!IsSupported || !IsEnabled)
                {
                    Console.WriteLine($"[ERROR]: {keyCode} {message}");
                    return;
                }
                Write("[", Color.White);
                Write($"ERROR", Color.Red);
                Write("]: ", Color.White);
                Write($"{keyCode.Format()}", Color.Red);
                WriteLine($" {message}", Color.White);
            }
        }
    }
}