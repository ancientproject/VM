namespace rune.cmd
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Ancient.ProjectSystem;
    using cli;
    using etc;

    public class VMCommand
    {
        public AncientProject project { get; set; }
        public static int Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune vm",
                FullName = "Ancient project execute in vm",
                Description = "Execute project in Ancient VM"
            };


            app.HelpOption("-h|--help");
            var dotnetBuild = new BuildCommand();
            var vm = new VMCommand();
            app.OnExecute(() =>
            {
                var buildResult = dotnetBuild.Execute(true);
                return buildResult != 0 ? buildResult : vm.Execute();
            });
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

        public int Execute()
        {
            var directory = Directory.GetCurrentDirectory();
            var projectFiles = Directory.GetFiles(directory, "*.rune.json");

            if (projectFiles.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Couldn't find a project to run. Ensure a project exists in {directory}.");
            }

            var p = projectFiles.Single();

            project = AncientProject.Open(new FileInfo(p));

            var ancient_home = Environment.GetEnvironmentVariable("ANCIENT_HOME", EnvironmentVariableTarget.User);

            if (ancient_home is null)
                throw new InvalidOperationException($"env variable 'ANCIENT_HOME' is not set.");
            if (!new DirectoryInfo(ancient_home).Exists)
                throw new InvalidOperationException($"Env variable 'ANCIENT_HOME' is invalid.");

            var vm_home = Path.Combine(ancient_home, "vm");
            var vm_bin = Path.Combine(vm_home, "vm.exe");

            if (!new DirectoryInfo(vm_home).Exists || !new FileInfo(vm_bin).Exists)
                throw new InvalidOperationException($"Ancient VM is not installed.");

            var argBuilder = new List<string>();

            var files = Directory.GetFiles(Path.Combine("obj"), "*.dlx");

            argBuilder.Add($"\"{files.First()}\"");

            var external = new ExternalTools(vm_bin, string.Join(" ", argBuilder));
            return external.Start().Wait().ExitCode();

        }
    }
}