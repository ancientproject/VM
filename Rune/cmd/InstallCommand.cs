namespace rune.cmd
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using Ancient.ProjectSystem;
    using cli;
    using etc;
    using Internal;

    public class InstallCommand : WithProject
    {
        public static async Task<int> Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune install",
                FullName = "Install device package.",
                Description = "Install device package from ancient registry"
            };


            app.HelpOption("-h|--help");
            var package = app.Argument("<package>", "package name");
            var registry = app.Option("--registry <url>", "registry url", CommandOptionType.SingleValue);
            var cmd = new InstallCommand();
            var restore = new RestoreCommand();
            app.OnExecute(async () =>
            {
                var result = await cmd.Execute(package.Value, registry);
                if (result != 0)
                    return result;
                return await restore.Execute(registry);
            });

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

        public async Task<int> Execute(string package, CommandOption registryOption)
        {
            var registry = 
                registryOption.HasValue() ? 
                    registryOption.Value() : 
                    Config.Get("core", "registry", "github+https://github.com/ancientproject");

            var dir = Directory.GetCurrentDirectory();

            if (!Validate(dir))
                return 1;

            if (Indexer.FromLocal().UseLock().Exist(package))
            {
                Console.WriteLine($"{":page_with_curl:".Emoji()} '{package}' is already {"found".Nier(0).Color(Color.Red)} in project.");
                return 1;
            }


            if(!await Registry.By(registry).Exist(package))
            {
                Console.WriteLine($"{":page_with_curl:".Emoji()} '{package}' is {"not".Nier(0).Color(Color.Red)} found in '{registry}' registry.");
                return 1;
            }

            try
            {
                var (asm, bytes) = await Registry.By(registry).Fetch(package);

                if (asm is null)
                    return 1;

                Indexer.FromLocal()
                    .UseLock()
                    .SaveDep(asm, bytes, registry);
                AncientProject.FromLocal().AddDep(package, asm.GetName().Version.ToString(), DepVersionKind.Fixed);
                Console.WriteLine($"{":movie_camera:".Emoji()} '{package}' save to deps is {"success".Nier(0).Color(Color.GreenYellow)}.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString().Color(Color.Red));
                return 2;
            }
            return 0;
        }
    }
}