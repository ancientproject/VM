namespace ancient.runtime.tools
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;

    public class DebugSymbols
    {
        public List<(short offset, string line)> symbols = new List<(short offset, string line)>();
        public static string SymbolEncoding => Environment.GetEnvironmentVariable("VM_SYM_ENCODING") ?? "IBM037";

        public static DebugSymbols Open(byte[] file)
        {
            var str = Convert.FromBase64String(Encoding.GetEncoding(SymbolEncoding).GetString(file));
            return JsonConvert.DeserializeObject<DebugSymbols>(Encoding.UTF8.GetString(str));
        }

        public static byte[] ToBytes(DebugSymbols sym)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sym));
            return Encoding.GetEncoding(SymbolEncoding).GetBytes(Convert.ToBase64String(bytes));
        }
    }
}