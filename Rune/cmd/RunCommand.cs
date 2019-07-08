namespace rune.cmd
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Ancient.ProjectSystem;
    using cli;
    using etc;
    using Internal;

    public class RunCommand : WithProject
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
            var type =  app.Argument("<script>", "script name");
            var dotnetNew = new RunCommand();
            app.OnExecute(() => dotnetNew.Execute(type.Value));

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
            if (!Validate(directory))
                return 1;
            var script = AncientProject.FromLocal().scripts.FirstOrDefault(x => x.Key.Equals(value, StringComparison.InvariantCultureIgnoreCase)).Value;

            if(script is null)
                throw new InvalidOperationException($"Command '{value}' not found.");
            Console.WriteLine($"trace :: call :> cmd /c '{script}'".Color(Color.DimGray));
            var proc = default(Process);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                proc = new Process
                {
                    StartInfo = new ProcessStartInfo("cmd.exe", $"/c \"{script}\"")
                    {
                        RedirectStandardError = true, 
                        RedirectStandardOutput = true
                    }
                };
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                proc = new Process
                {
                    StartInfo = new ProcessStartInfo("bash", $"-c \"{script}\"")
                    {
                        RedirectStandardError = true, 
                        RedirectStandardOutput = true
                    }
                };
            }

            proc.Start();
            proc.WaitForExit();

            var err = proc.StandardError.ReadToEnd();
            var @out = proc.StandardOutput.ReadToEnd();
            if(!string.IsNullOrEmpty(err )) Console.WriteLine($"{err}".Color(Color.Red));
            if(!string.IsNullOrEmpty(@out)) Console.WriteLine($"{@out}".Color(Color.DarkGray));

            return proc.ExitCode;
        }
    }
}