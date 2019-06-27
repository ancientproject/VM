namespace rune.cmd
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Ancient.ProjectSystem;
    using cli;
    using etc;

    public class BuildCommand
    {
        public AncientProject project { get; set; }

        public static int Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune build",
                FullName = "Ancient project build",
                Description = "Build all files from project"
            };


            app.HelpOption("-h|--help");
            var type = app.Option("-t|--type <TYPE>", "script name", CommandOptionType.SingleValue);
            var dotnetNew = new BuildCommand();
            app.OnExecute(() => dotnetNew.Execute());

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

            return 0;

        }
    }
}