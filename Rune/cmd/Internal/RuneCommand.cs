namespace rune.cmd.Internal
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using cli;
    using etc;

    public abstract class RuneCommand<T> where T : RuneCommand<T>, new()
    {
        public static async Task<int> Run(string[] args)
        {
            var cmd = new T();

            var app = cmd.Setup();


            try
            {
                return await app.Execute(args);
            }
            catch (CommandParsingException parsingException)
            {
                Console.WriteLine(parsingException.Message.Color(Color.Red));
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString().Color(Color.Red));
                return 1;
            }
        }

        internal abstract CommandLineApplication Setup();
    }
}