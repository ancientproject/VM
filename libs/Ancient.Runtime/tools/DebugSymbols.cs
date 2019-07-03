namespace ancient.runtime.tools
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;

    public class DebugSymbols
    {
        public List<(short offset, string line)> symbols = new List<(short offset, string line)>();


        public static DebugSymbols Open(byte[] file)
        {
            var str = Convert.FromBase64String(Encoding.GetEncoding("IBM037").GetString(file));
            return JsonConvert.DeserializeObject<DebugSymbols>(Encoding.UTF8.GetString(str));
        }

        public static byte[] ToBytes(DebugSymbols sym)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sym));
            return Encoding.GetEncoding("IBM037").GetBytes(Convert.ToBase64String(bytes));
        }
    }
}