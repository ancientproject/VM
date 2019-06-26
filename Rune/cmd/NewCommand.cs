namespace Rune.cmd
{
    using System;
    using System.Drawing;
    using System.Linq;
    using cli;
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
                Console.WriteLine(ex.ToString().Pastel(Color.Red));
                return 1;
            }
        }

        private int CreateEmptyProject()
        {
            return 0;
        }
    }
}