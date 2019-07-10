namespace rune
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using cmd;
    using etc;
    using Microsoft.DotNet.PlatformAbstractions;
    using Newtonsoft.Json;
    using static System.Console;
    using static System.Environment;
    using RuntimeEnvironment = Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment;

    internal class Host
    {
        private static readonly Dictionary<string, Func<string[], int>> s_builtIns 
            = new Dictionary<string, Func<string[], int>>
        {
            ["new"]         = NewCommand.Run,
            ["help"]        = HelpCommand.Run,
            ["run"]         = RunCommand.Run,
            ["build"]       = BuildCommand.Run,
            ["vm"]          = VMCommand.Run,
            ["new-scheme"]  = SchemeCommand.Run,
            ["install"]     = InstallCommand.Run,
            ["clear"]       = ClearCommand.Run,
            ["remove"]      = RemoveCommand.Run,
            ["restore"]     = RestoreCommand.Run
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
                WriteLine(e.Message.Color(Color.OrangeRed));
                return 1;
            }
        }
        private static void InitializeProcess()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                WriteLine("Platform is not supported.");
                Exit(-0xFFFFFFF);
                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                OutputEncoding = Encoding.Unicode;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore, 
                Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, 
                Culture = CultureInfo.InvariantCulture
            };
            if (GetEnvironmentVariable("WT_SESSION") == null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SetEnvironmentVariable($"RUNE_EMOJI_USE", "0");
                SetEnvironmentVariable($"RUNE_COLOR_USE", "0");
                SetEnvironmentVariable($"RUNE_NIER_USE", "0");
                SetEnvironmentVariable($"NO_COLOR", "true");
                ForegroundColor = ConsoleColor.Gray;
                WriteLine($"no windows-terminal: coloring, emoji and nier has disabled.");
                ForegroundColor = ConsoleColor.White;
            }
        }

        internal static bool Verbose { get; set; }

        internal static int ProcessArgs(string[] args)
        {
            var success = true;
            var command = string.Empty;
            var lastArg = 0;
            for (; lastArg < args.Length; lastArg++)
            {
                if (IsArg(args[lastArg], "d", "diagnostics"))
                    Verbose = true;
                else if (IsArg(args[lastArg], "version"))
                {
                    HelpCommand.PrintVersion();
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
                    WriteLine($"Unknown option: {args[lastArg]}");
                    success = false;
                }
                else
                {
                    command = args[lastArg];
                    break;
                }
            }
            if (!success)
            {
                HelpCommand.PrintHelp();
                return 1;
            }

            var appArgs = (lastArg + 1) >= 
                          args.Length ? 
                Enumerable.Empty<string>() : 
                args.Skip(lastArg + 1).ToArray();


            if (string.IsNullOrEmpty(command))
                command = "help";

            var watch = Stopwatch.StartNew();

            int exitCode;
            if (s_builtIns.TryGetValue(command, out var builtIn))
                exitCode = builtIn(appArgs.ToArray());
            else
            {
                WriteLine("Could not execute because the specified command or file was not found.".Color(Color.Red));
                exitCode = -1;
            }
                
            watch.Stop();

            WriteLine($"{":sparkles:".Emoji()} Done in {watch.Elapsed.TotalSeconds:00.000}s.");

            return exitCode;
        }
        private static void PrintInfo()
        {
            WriteLine($" OS Name:     {RuntimeEnvironment.OperatingSystem}");
            WriteLine($" OS Version:  {RuntimeEnvironment.OperatingSystemVersion}");
            WriteLine($" OS Platform: {RuntimeEnvironment.OperatingSystemPlatform}");
            WriteLine($" Base Path:   {ApplicationEnvironment.ApplicationBasePath}");
        }
        private static bool IsArg(string candidate, string longName) 
            => IsArg(candidate, null, longName);
        private static bool IsArg(string candidate, string shortName, string longName) =>
            (shortName != null && candidate.Equals($"-{shortName}", StringComparison.OrdinalIgnoreCase)) ||
            (longName != null && candidate.Equals($"--{longName}", StringComparison.OrdinalIgnoreCase));
    }
}
