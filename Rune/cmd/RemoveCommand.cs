namespace rune.cmd
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using Ancient.ProjectSystem;
    using cli;
    using etc;
    using static System.Console;

    public class RemoveCommand
    {
        public static async Task<int> Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune remove",
                FullName = "Remove device package.",
                Description = "Remove device package from ancient project"
            };


            app.HelpOption("-h|--help");
            var package = app.Argument("<package>", "package name");
            var cmd = new RemoveCommand();
            app.OnExecute(() => cmd.Execute(package.Value));

            try
            {
                return await app.Execute(args);
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString().Color(Color.Red));
                return 1;
            }
        }

        public int Execute(string id)
        {
            if (!Indexer.FromLocal().UseLock().Exist(id))
            {
                WriteLine($"{":loudspeaker:".Emoji()} '{$"{id}".Color(Color.Gray)}' {"not".Nier(0).Color(Color.Red)} found.");
                return 1;
            }
            Indexer.FromLocal().UseLock().GetVersion(id, out var version).RevDep(id);
            WriteLine($"{":loudspeaker:".Emoji()} remove '{$"{id}^{version.ToString(2)}".Color(Color.Gray)}' {"success".Nier(0).Color(Color.GreenYellow)}.");
            return 0;
        }
    }
}