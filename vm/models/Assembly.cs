namespace vm.models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    public class Assembly
    {
        private readonly Instruction[] _instructions;
        private readonly Version _version;

        public Assembly(Instruction[] instructions, Version version)
        {
            _instructions = instructions;
            _version = version;
        }

        public void Save(DirectoryInfo info, string name) // todo shitcode
        {
            if(!info.Exists) throw new InvalidOperationException();

            var code_bytes = _instructions.SelectMany(x => BitConverter.GetBytes(x.Assembly())).ToArray();
            var code_map = _instructions.Select(x => $"0x{x.Assembly():X}").ToArray();
            var header = new Head(_version);

            header.metadata.Add("timestamp", DateTime.UtcNow.Ticks.ToString());
            header.metadata.Add("author", "Yuuki Wesp");


            var code_page = new Page(".code", code_bytes);
            var header_page = new Page(".header", header.ToBytes());

            var bytes = new List<byte>();
            bytes.AddRange(new []{ (byte)'o', (byte)'x', (byte)'_', (byte)'e', (byte)'x', (byte)'e', (byte)'\n' });
            bytes.AddRange(new []{ (byte)'h', (byte)' ' });
            bytes.AddRange(BitConverter.GetBytes(header_page.ToBytes().Length));
            bytes.AddRange(new[] { (byte)'\n' });

            bytes.AddRange(new[] { (byte)'c', (byte)' ' });
            bytes.AddRange(BitConverter.GetBytes(code_page.ToBytes().Length));
            bytes.AddRange(new[] { (byte)'\n' });

            bytes.AddRange(header_page.ToBytes());
            bytes.AddRange(code_page.ToBytes());

            File.WriteAllBytes(Path.Combine(info.FullName, $"{name}.ox"), bytes.ToArray());
            File.WriteAllLines(Path.Combine(info.FullName, $"{name}.map"), code_map);
        }


        public class Page
        {
            public Page(string name, byte[] cnt)
            {
                c_name = name;
                content = cnt;
            }
            public string c_name { get; set; }
            public byte[] content { get; set; }


            public byte[] ToBytes()
            {
                using var stream = new MemoryStream();
                var n = Encoding.UTF8.GetBytes(c_name);
                var n_len = BitConverter.GetBytes(n.Length);
                var c_len = BitConverter.GetBytes(content.Length);
                foreach (var b in n_len)
                    stream.WriteByte(b);
                foreach (var b in c_len)
                    stream.WriteByte(b);
                foreach (var b in n)
                    stream.WriteByte(b);
                foreach (var b in content)
                    stream.WriteByte(b);
                return stream.ToArray();
            }
        }

        public class Head
        {
            public Version _v;
            public Dictionary<string, string> metadata { get; set; } = new Dictionary<string, string>();
            public Head() { }
            public Head(Version v) => _v = v;

            public byte[] ToBytes()
            {
                using var stream = new MemoryStream();
                var content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
                var n_len = BitConverter.GetBytes(content.Length);
                foreach (var b in n_len)
                    stream.WriteByte(b);
                foreach (var b in content)
                    stream.WriteByte(b);
                return stream.ToArray();
            }
        }
    }
}