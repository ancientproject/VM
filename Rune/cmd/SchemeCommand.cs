namespace rune.cmd
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using Ancient.ProjectSystem;
    using cli;
    using etc;
    using Newtonsoft.Json;

    public class SchemeCommand
    {
        public static async Task<int> Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "rune new-scheme",
                FullName = "Ancient device-mapper initializer",
                Description = "Initializes empty map file for Ancient VM Devices"
            };


            app.HelpOption("-h|--help");
            var cmd = new SchemeCommand();
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
            var dir = Directory.GetCurrentDirectory();
            var scheme = Path.Combine(dir, "device.scheme");

            if(new FileInfo(scheme).Exists)
            {
                Console.WriteLine($"{":dizzy:".Emoji()} '{"device.scheme".Color(Color.Gray)}' {"already".Nier(0).Color(Color.Red)} exist.");
                return 1;
            }

            var sc = new DeviceScheme();

            sc.scheme.Add("memory", "0x0");
            sc.scheme.Add("bios", "0x45");
            
            File.WriteAllText(scheme, JsonConvert.SerializeObject(sc));
            Console.WriteLine($"{":dizzy:".Emoji()} {"Success".Nier().Color(Color.GreenYellow)} write device scheme to '{"./device.scheme".Color(Color.Gray)}'");
            return 0;
        }
    }
}