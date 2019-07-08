namespace rune.cmd
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using etc;

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
  new-scheme    Initializes empty map file for Ancient VM Devices
  install       Install device package from ancient registry
  clear         Clearing deps in current project
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
            Console.WriteLine(BuildUsageText());
        }


        private static string BuildUsageText()
        {
            var builder = new StringBuilder();


            builder.AppendLine(
                $"Usage: {"rune".Color(Color.BurlyWood)} " +
                $"[{"common-options".Color(Color.Gray)}] " +
                $"[{"command".Color(Color.Gray)}] " +
                $"[{"arguments".Color(Color.Gray)}]");

            builder.AppendLine();
            builder.AppendLine($"Arguments:");
            builder.AppendLine($"  [{"command".Color(Color.Gray)}]    {"The command to execute".Color(Color.DarkKhaki)}");
            builder.AppendLine($"  [{"arguments".Color(Color.Gray)}]  {"Arguments to pass to the command".Color(Color.DarkKhaki)}");
            builder.AppendLine();
            builder.AppendLine($"Common Options {"(passed before the command)".Color(Color.Gray)}:");
            builder.AppendLine($"  {"--version".Color(Color.Gray)}    {"Display Rune CLI Version Number".Color(Color.DarkKhaki)}");
            builder.AppendLine($"  {"--info".Color(Color.Gray)}       {"Display Rune CLI Info".Color(Color.DarkKhaki)}");
            builder.AppendLine();
            builder.AppendLine($"Common Commands:");

            builder.AppendLine($"  {"new".Color(Color.CornflowerBlue)}          {"Initialize a basic Ancient project".Color(Color.DarkKhaki)}");
            builder.AppendLine($"  {"new-scheme".Color(Color.CornflowerBlue)}   {"Initializes empty map file for Ancient VM Devices".Color(Color.DarkKhaki)}");
            builder.AppendLine($"  {"install".Color(Color.CornflowerBlue)}      {"Install device package from ancient registry".Color(Color.DarkKhaki)}");
            builder.AppendLine($"  {"clear".Color(Color.CornflowerBlue)}        {"Clearing deps in current project".Color(Color.DarkKhaki)}");
            builder.AppendLine($"  {"build".Color(Color.CornflowerBlue)}        {"Builds a Ancient project".Color(Color.DarkKhaki)}");
            builder.AppendLine($"  {"restore".Color(Color.CornflowerBlue)}      {"Restore packages from current project".Color(Color.DarkKhaki)}");
            builder.AppendLine($"  {"run".Color(Color.CornflowerBlue)}          {"Immediately executes a script from Ancient project".Color(Color.DarkKhaki)}");
            builder.AppendLine($"  {"vm".Color(Color.CornflowerBlue)}           {"Immediately build and execute project in Ancient VM".Color(Color.DarkKhaki)}");
            builder.AppendLine();
            return builder.ToString();
        }

        public static void PrintVersionHeader()
        {
            Console.WriteLine($"Rune v0.47 x64 [{"Ancient SDK".Color(Color.Chocolate)}]");
        }
    }
}