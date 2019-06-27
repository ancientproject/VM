namespace rune.cmd
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Ancient.ProjectSystem;
    using cli;
    using DustInTheWind.ConsoleTools.InputControls;
    using DustInTheWind.ConsoleTools.Musical;
    using DustInTheWind.ConsoleTools.Spinners;
    using etc;
    using Newtonsoft.Json;
    using Pastel;

    public class NewCommand
    {
        public static int Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune new",
                FullName = "Ancient project initializer",
                Description = "Initializes empty project for Ancient VM"
            };


            app.HelpOption("-h|--help");

            var type = app.Option("-t|--type <TYPE>", "Type of project", CommandOptionType.SingleValue);
            var dotnetNew = new NewCommand();
            app.OnExecute(() => dotnetNew.CreateEmptyProject());

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

        private int CreateEmptyProject()
        {
            var projectName = new ValueView<string>($"[1/4] {":drum:".Emoji()} Project Name:").WithDefault(Directory.GetCurrentDirectory().Split('/').Last()).Read();
            var version     = new ValueView<string>($"[2/4] {":boom:".Emoji()} Project Version:").WithDefault("0.0.0").Read();
            var desc        = new ValueView<string>($"[3/4] {":balloon:".Emoji()} Project Description:").WithDefault("").Read();
            var author      = new ValueView<string>($"[4/4] {":skull:".Emoji()} Project Author:").WithDefault("").Read();
            var dir = Directory.GetCurrentDirectory();

            var proj = new AncientProject
            {
                name = projectName,
                version = version,
                author = author
            };

            proj.scripts.Add($"start", "echo 1");


            File.WriteAllText($"{Path.Combine(dir, $"{projectName}.rune")}", JsonConvert.SerializeObject(proj));

            return 0;
        }
    }
}