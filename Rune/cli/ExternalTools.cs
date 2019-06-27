namespace rune.cli
{
    using System.Diagnostics;

    public class ExternalTools
    {
        private readonly string _cmd;
        private readonly string _args;
        private readonly Process proc;
        public ExternalTools(string cmd, string args)
        {
            _cmd = cmd;
            _args = args;
            proc = new Process
            {
                StartInfo = new ProcessStartInfo(cmd, args)
                {
                }
            };
        }

        public ExternalTools Start()
        {
            proc.Start();
            return this;
        }

        public ExternalTools Wait()
        {
            proc.WaitForExit();
            return this;
        }

        public int ExitCode() => proc.ExitCode;
    }
}