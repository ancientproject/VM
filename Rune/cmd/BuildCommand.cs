namespace rune.cmd
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Ancient.ProjectSystem;
    using cli;
    using etc;
    using Internal;

    public class BuildCommand : WithProject
    {
        public static int Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune build",
                FullName = "Ancient project build",
                Description = "Build all files from project"
            };

            app.HelpOption("-h|--help");
            var type = app.Option("-t|--temp <bool>", "Is temp", CommandOptionType.BoolValue);
            var dotnetNew = new BuildCommand();
            app.OnExecute(() => dotnetNew.Execute(type.BoolValue.HasValue));

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

        public int Execute(bool isTemp)
        {
            var directory = Directory.GetCurrentDirectory();
            if (!Validate(directory))
                return 1;

            var ancient_home = Environment.GetEnvironmentVariable("ANCIENT_HOME", EnvironmentVariableTarget.User);

            if (ancient_home is null)
                throw new InvalidOperationException($"Env variable 'ANCIENT_HOME' is not set.");
            if (!new DirectoryInfo(ancient_home).Exists)
                throw new InvalidOperationException($"Env variable 'ANCIENT_HOME' is invalid.");

            var acc_home = Path.Combine(ancient_home, "compiler");
            var acc_bin = Path.Combine(acc_home, "acc.exe");

            if (!new DirectoryInfo(acc_home).Exists || !new FileInfo(acc_bin).Exists)
                throw new InvalidOperationException($"Ancient compiler is not installed.");

            var argBuilder = new List<string>();

            var files = Directory.GetFiles(directory, "*.asm");

            var outputDir = "bin";

            if (isTemp)
                outputDir = "obj";
            var Project = AncientProject.FromLocal();
            argBuilder.Add($"-o ./{outputDir}/{Project.Name}");
            if (Project.Extension != null)
                argBuilder.Add($"-e {Project.Extension}");
            argBuilder.Add($"-s \"{files.First()}\"");

            var external = new ExternalTools(acc_bin, string.Join(" ", argBuilder));
            Directory.CreateDirectory(Path.Combine(directory, outputDir));
            return external.Start().Wait().ExitCode();
        }
    }
}