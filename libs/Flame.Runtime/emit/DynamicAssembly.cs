namespace flame.runtime.emit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class DynamicAssembly : FlameAssembly
    {
        [JsonIgnore]
        private ILGen generator { get; }

        public DynamicAssembly(string name, IEnumerable<(string key, string value)> metadata)
        {
            Name = name;
            Metadata = metadata.ToList();
            generator = new ILGen(this);
        }

        public ILGen GetGenerator() => generator;


        public byte[] GetBytes()
        {
            //if(!generator.Any())
            //    throw new InvalidOperationException("Invalid state for current assembly.");
            var header = Encoding.UTF32.GetBytes(JsonConvert.SerializeObject(new { Name, Metadata }));
            var list = new List<byte>();

            //using var sig = new AssemblySigner(SymmetricAlgorithm.Create("DES"), HashAlgorithm.Create("MD5"));
            //using var inMemory = new MemoryStream(header);
            //using var outMemory = new MemoryStream();

            //var pass = new SecureString();
            //Array.ForEach("flame-asm".ToArray(), pass.AppendChar);
            //pass.MakeReadOnly();
            //sig.SetPassword(pass);

            var headerLen = header.LongLength;//sig.EncryptStream(inMemory, outMemory, default);
            var body = generator.Load().SelectMany(x => x.GetBodyILBytes()).ToArray(out var bodyLen);

            
            void push<T>(T value)
            {
                switch (value)
                {
                    case string str:
                        list.AddRange(Encoding.ASCII.GetBytes(str));
                        return;
                    case byte[] arr:
                        list.AddRange(arr);
                        return;
                    case MemoryStream mem:
                        list.AddRange(mem.ToArray());
                        return;
                    default:
                        var size = Marshal.SizeOf(typeof(T));
                        var result = new byte[size];
                        var gcHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
                        Marshal.Copy(gcHandle.AddrOfPinnedObject(), result, 0, size);
                        gcHandle.Free();
                        list.AddRange(result);
                        return;
                }
            }

            // wS 8 bytes - file type
            push("EFV_1\0\0\0");
            push("\n"); // push 0x0A to next section
            // wL 8 bytes - header len
            push(headerLen);
            // wM ? bytes - header body 
            push(header);
            push("\n"); // push 0x0A to next section
            // wL 8 bytes - body code len
            push(bodyLen);
            // wL ? bytes - body code
            push(body);
            push("\n"); // push 0x0A to next section
            return list.ToArray();
        }
    }
}