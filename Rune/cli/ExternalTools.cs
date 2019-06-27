namespace rune.cli
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class ExternalTools
    {
        private readonly string _cmd;
        private readonly string _args;
        private readonly Process proc;
        public ExternalTools(string cmd, string args, IDictionary<string, string> env = null)
        {
            _cmd = cmd;
            _args = args;
            proc = new Process
            {
                StartInfo = new ProcessStartInfo(cmd, args)
                {
                    
                }
            };
            if (env is null) 
                return;
            foreach (var (key, value) in env) 
                proc.StartInfo.Environment.Add(key, value);
        }

        public ExternalTools WithEnv(string flagName, bool value)
        {
            proc.StartInfo.Environment.Add(flagName, value ? "1" : "0");
            return this;
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