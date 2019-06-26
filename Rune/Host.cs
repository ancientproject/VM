namespace Rune
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using cmd;
    using Microsoft.DotNet.PlatformAbstractions;
    using Pastel;

    internal class Host
    {

        private static Dictionary<string, Func<string[], int>> s_builtIns = new Dictionary<string, Func<string[], int>>
        {
            ["new"] = NewCommand.Run,
            ["help"] = HelpCommand.Run
        };
        public static int Main(string[] args)
        {
            InitializeProcess();

            try
            {
                return ProcessArgs(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.Pastel(Color.OrangeRed));
                return 1;
            }
        }
        private static void InitializeProcess()
        {
            // by default, .NET Core doesn't have all code pages needed for Console apps.
            // see the .NET Core Notes in https://msdn.microsoft.com/en-us/library/system.diagnostics.process(v=vs.110).aspx
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        internal static int ProcessArgs(string[] args)
        {
            bool? verbose = null;
            var success = true;
            var command = string.Empty;
            var lastArg = 0;
            for (; lastArg < args.Length; lastArg++)
            {
                if (IsArg(args[lastArg], "d", "diagnostics"))
                    verbose = true;
                else if (IsArg(args[lastArg], "version"))
                {
                    PrintVersion();
                    return 0;
                }
                else if (IsArg(args[lastArg], "info"))
                {
                    PrintInfo();
                    return 0;
                }
                else if (IsArg(args[lastArg], "h", "help") || 
                         args[lastArg] == "-?" ||
                         args[lastArg] == "/?")
                {
                    HelpCommand.PrintHelp();
                    return 0;
                }
                else if (args[lastArg].StartsWith("-"))
                {
                    Console.WriteLine($"Unknown option: {args[lastArg]}");
                    success = false;
                }
                else
                {
                    // It's the command, and we're done!
                    command = args[lastArg];
                    break;
                }
            }
            if (!success)
            {
                HelpCommand.PrintHelp();
                return 1;
            }

            var appArgs = (lastArg + 1) >= args.Length ? Enumerable.Empty<string>() : args.Skip(lastArg + 1).ToArray();


            if (string.IsNullOrEmpty(command))
                command = "help";


            int exitCode;
            if (s_builtIns.TryGetValue(command, out var builtIn))
                exitCode = builtIn(appArgs.ToArray());
            else
                exitCode = -1;

            return exitCode;
        }
        private static void PrintVersion()
        {
            Console.WriteLine("v1.0");
        }

        private static void PrintInfo()
        {
            Console.WriteLine($" OS Name:     {RuntimeEnvironment.OperatingSystem}");
            Console.WriteLine($" OS Version:  {RuntimeEnvironment.OperatingSystemVersion}");
            Console.WriteLine($" OS Platform: {RuntimeEnvironment.OperatingSystemPlatform}");
            Console.WriteLine($" Base Path:   {ApplicationEnvironment.ApplicationBasePath}");
        }

        private static bool IsArg(string candidate, string longName)
        {
            return IsArg(candidate, shortName: null, longName: longName);
        }

        private static bool IsArg(string candidate, string shortName, string longName)
        {
            return (shortName != null && candidate.Equals("-" + shortName, StringComparison.OrdinalIgnoreCase)) ||
                   (longName != null && candidate.Equals("--" + longName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
