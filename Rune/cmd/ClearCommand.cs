namespace rune.cmd
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using Ancient.ProjectSystem;
    using cli;
    using etc;

    public class ClearCommand
    {
        public static async Task<int> Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune clear",
                FullName = "Clear all deps.",
                Description = "Clearing deps in current project."
            };


            app.HelpOption("-h|--help");
            var cmd = new ClearCommand();
            app.OnExecute(() => cmd.Execute());

            try
            {
                return await app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString().Color(Color.Red));
                return 1;
            }
        }

        public int Execute()
        {
            try
            {
                Indexer.FromLocal().UseLock().DropDeps();
                Console.WriteLine($"{":leaves:".Emoji()} clearing deps {"success".Nier(0).Color(Color.GreenYellow)}!");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"{":fallen_leaf:".Emoji()} clearing deps {"fail".Nier(1).Color(Color.Red)}!");
                return 1;
            }
            
        }
    }
}