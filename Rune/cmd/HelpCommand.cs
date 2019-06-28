namespace rune.cmd
{
    using System;
    using System.Linq;

    public class HelpCommand
    {
        private const string UsageText = @"Usage: rune [common-options] [command] [arguments]

Arguments:
  [command]     The command to execute
  [arguments]   Arguments to pass to the command

Common Options (passed before the command):
  --version     Display Rune CLI Version Number
  --info        Display Rune CLI Info

Common Commands:
  new           Initialize a basic Ancient project
  build         Builds a Ancient project
  run           Immediately executes a script from Ancient project
  vm            Immediately build and execute project in Ancient VM";

        public static int Run(string[] args)
        {
            if (args.Any())
                return Host.Main(new[] { args[0], "--help" });
            PrintHelp();
            return 0;
        }

        public static void PrintHelp()
        {
            PrintVersionHeader();
            Console.WriteLine(UsageText);
        }

        public static void PrintVersionHeader()
        {
            Console.WriteLine("Rune v0.31 x64 [Ancient SDK]");
        }
    }
}