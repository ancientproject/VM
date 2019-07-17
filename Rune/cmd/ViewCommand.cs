namespace rune.cmd
{
    using System;
    using System.Drawing;
    using System.IO;
    using ancient.runtime.tools;
    using cli;
    using etc;

    public class ViewCommand
    {
        public static int Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune view",
                FullName = "View file.",
                Description = "View file as hex table."
            };


            app.HelpOption("-h|--help");
            var cmd = new ViewCommand();
            var file = app.Argument("<file>", "file name");
            app.OnExecute(() => cmd.Execute(file.Value));

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

        public int Execute(string file)
        {
            var dir = Directory.GetCurrentDirectory();
            var info = new FileInfo(file);

            if (!info.Exists)
            {
                Console.WriteLine($"{":page_with_curl:".Emoji()} '{info}' not {"found".Nier(0).Color(Color.Red)}.");
                return 1;
            }

            Console.WriteLine(ByteArrayUtils.PrettyHexDump(File.ReadAllBytes(file)));
            return 0;
        }
    }
}