namespace rune.cmd
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using cli;
    using etc;
    using Internal;


    public class ConfigCommand : RuneCommand<ConfigCommand>
    {
        internal override CommandLineApplication Setup()
        {
            var app = new CommandLineApplication
            {
                Name = "rune config",
                FullName = "Ancient project build",
                Description = "Manage config"

            };

            app.Command("set", SetConfig);
            app.Command("get", GetConfig);
            app.Command("list", ListConfig);

            app.HelpOption("-h|--help");

            return app;
        }

        private static void ListConfig(CommandLineApplication app)
        {
            app.Description = $"Print current config";
            app.OnExecute(() =>
            {
                var configRaw = Config.GetRaw();

                if (string.IsNullOrEmpty(configRaw))
                {
                    Console.WriteLine($"Config has empty".Color(Color.Orange));
                    return 0;
                }

                Console.WriteLine($"\n{Config.GetRaw()}");
                return 0;
            });
        }

        private static void SetConfig(CommandLineApplication app)
        {
            app.Description = $"[section:key] [value] Set value by key into config";
            var key = app.Argument("key", "");
            var value = app.Argument("value", "");
            app.HelpOption("-h|--help");

            app.OnExecute(new SetCommand(key, value).Execute);
        }
        private static void GetConfig(CommandLineApplication app)
        {
            app.Description = $"[section:key] Get value by key from config";
        }

        private class SetCommand
        {
            private readonly CommandArgument _key;
            private readonly CommandArgument _value;

            public SetCommand(CommandArgument key, CommandArgument value)
            {
                _key = key;
                _value = value;
            }

            public async Task<int> Execute()
            {
                if (_key.IsEmpty())
                    return Error($"key argument expects.");
                if (_value.IsEmpty())
                    return Error($"value argument expects.");

                if (!Regex.IsMatch(_key.Value, @"\w+\:\w+"))
                    return Error($"'{_key.Value}' is not valid format. [section:key](\\w+\\:\\w+)");
                var section = _key.Value.Split(':').First();
                var key = _key.Value.Split(':').Last();

                Config.Set(section, key, _value.Value);

                Console.WriteLine($"{":heavy_check_mark:".Emoji()} {"Success".Nier().Color(Color.GreenYellow)} set '{_value.Value}' to '{section}:{key}'.");

                return 0;
            }
        }


        private static int Error(string text)
        {
            Console.WriteLine(text.Color(Color.Red));
            return 1;
        }

    }
}