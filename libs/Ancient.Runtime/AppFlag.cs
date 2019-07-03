namespace ancient.runtime
{
    using System;

    public static class AppFlag
    {
        public static bool GetVariable(string code, bool? @default = null)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            var result = Environment.GetEnvironmentVariable(code.ToUpperInvariant())?.ToLowerInvariant();

            if (result is null && @default is null)
                return false;
            if (result is null)
                return @default.Value;
            return    result == "1" 
                   || result == "true" 
                   || result == "on" 
                   || result == "enable" 
                   || result == "yes";
        }
    }
}