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
    using Newtonsoft.Json;

    public class DynamicAssembly : AncientAssembly
    {
        [JsonIgnore]
        private ILGen generator { get; }
        

        public DynamicAssembly(string name, params (string key, string value)[] metadata)
        {
            Name = name;
            Metadata = metadata.ToList();
            generator = new ILGen(this);
        }

        public void EnableSign() 
            => Tag.Sign = AssemblyTag.SignType.Signed;
        public ILGen GetGenerator() 
            => generator;
        public override byte[] GetILCode() 
            => generator.Load().SelectMany(x => x.GetBodyILBytes()).ToArray();

        /// <summary>
        /// Get bytes of assembly
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Invalid state for current assembly.
        /// </exception>
        public byte[] GetBytes()
        {
            if(!generator.Any())
                throw new InvalidOperationException("Invalid state for current assembly.");
            var header = Encoding.UTF32.GetBytes(JsonConvert.SerializeObject(new { Name, Metadata }));
            var list = new List<byte>();
            var headerLen = 0L;
            using var outMemory = new MemoryStream();
            if (Tag.Sign == AssemblyTag.SignType.Signed)
            {
                using var sig = new AssemblySigner(SymmetricAlgorithm.Create("Rijndael"), HashAlgorithm.Create("MD5"));
                using var inMemory = new MemoryStream(header);
                
                var pass = new SecureString();
                Array.ForEach("flame-asm".ToArray(), pass.AppendChar);
                pass.MakeReadOnly();
                sig.SetPassword(pass);
                headerLen = sig.EncryptStream(inMemory, outMemory, default);
            }
            else
                headerLen = header.LongLength;

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

            // EF0119JG00
            // 01 - version
            // 0 - not sign, 1 - sign
            // 9 - any arch cpu
            // A - 2010 year, J - 2019
            // A - junary, G - june

            // wS 10 bytes - file type
            push(Tag.ToString());
            push("\n"); // push 0x0A to next section
            push("\n"); // push 0x0A to next section
            // wL 8 bytes - header len
            push(headerLen);
            // wM ? bytes - header body 
            if (Tag.Sign == AssemblyTag.SignType.Signed)
                push(outMemory);
            else 
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