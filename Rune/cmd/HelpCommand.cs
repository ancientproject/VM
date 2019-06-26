namespace Rune.cmd
{
    using System;

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
              run           Compiles and immediately executes a Ancient project";

        public static int Run(string[] args)
        {
            if (args.Length != 0) 
                return Host.Main(new[] {args[0], "--help"});
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
            Console.WriteLine("Rune v0.17 x64");
        }
    }
}