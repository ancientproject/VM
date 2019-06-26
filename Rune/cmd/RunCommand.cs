namespace Rune.cmd
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Ancient.ProjectSystem;
    using cli;

    public class RunCommand
    {
        public static int Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune run",
                FullName = "Ancient script runner",
                Description = "Run script from project"
            };


            app.HelpOption("-h|--help");

            var type = app.Option("<SCRIPT>", "script name", CommandOptionType.SingleValue);
            var dotnetNew = new RunCommand();
            app.OnExecute(() => dotnetNew.Execute(type.Value()));

            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString().Color(Color.Red));
                return 1;
            }
        }

        public int Execute(string value)
        {
            var directory = Directory.GetCurrentDirectory();
            var projectFiles = Directory.GetFiles(directory, "*.rune");

            if (projectFiles.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Couldn't find a project to run. Ensure a project exists in {directory}.");
            }

            var project = projectFiles.Single();

            var rune = AncientProject.Open(new FileInfo(project));

            var script = rune.scripts.FirstOrDefault(x => x.Key.Equals(value, StringComparison.InvariantCultureIgnoreCase)).Value;

            if(script is null)
                throw new InvalidOperationException($"Command '{value}' not found.");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo("cmd.exe", $"/c '{script}'")
                {
                    RedirectStandardError = true, 
                    RedirectStandardOutput = true
                }
            };

            proc.Start();
            proc.WaitForExit();

            var err = proc.StandardError.ReadToEnd();
            var @out = proc.StandardOutput.ReadToEnd();
            if(!string.IsNullOrEmpty(err))
                Console.WriteLine($"{err}".Color(Color.Red));
            if(!string.IsNullOrEmpty(@out))
                Console.WriteLine($"{@out}".Color(Color.DarkGray));

            return proc.ExitCode;
        }
    }
}