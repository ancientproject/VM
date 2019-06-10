namespace flame.runtime.emit
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using Newtonsoft.Json;

    public class FlameAssembly
    {
        public string Name { get; protected set; }
        public List<(string key, string value)> Metadata { get; protected set; }
        protected byte[] ILCode { get; set; }

        public static FlameAssembly Load(byte[] bytes)
        {
            using var mem = new MemoryStream(bytes);

            var type = mem.ReadBytes(sizeof(long));
            if (type[0] != 'E' && type[1] != 'F' && type[2] != 'V')
                throw new BadImageFormatException();
            mem.ReadBytes(1); // read '\n'
            var headerLen = BitConverter.ToInt64(mem.ReadBytes(sizeof(long)), 0);

            //using var sig = new AssemblySigner(SymmetricAlgorithm.Create("DES"), HashAlgorithm.Create("MD5"));
            //using var inMemory = new MemoryStream(mem.ReadBytes((int) headerLen));
            //using var outMemory = new MemoryStream();

            //var pass = new SecureString();
            //Array.ForEach("flame-asm".ToArray(), pass.AppendChar);
            //pass.MakeReadOnly();
            //sig.SetPassword(pass);

            //sig.DecryptStream(inMemory, outMemory, default);
            (string key, string value)[] Metadata = new []{("", "")};
            dynamic header = JsonConvert.DeserializeObject(Encoding.UTF32.GetString(mem.ReadBytes((int) headerLen)), new{Name = "", Metadata}.GetType());
           
            mem.ReadBytes(1); // read '\n'

            var bodyLen = BitConverter.ToInt64(mem.ReadBytes(sizeof(long)), 0);
            var body = mem.ReadBytes((int) bodyLen);

            var asm = new FlameAssembly
            {
                ILCode = body,
                Metadata = header.Metadata, 
                Name = header.Name
            };


            return asm;
        }

        public static FlameAssembly LoadFrom(string filename) // todo
            => Load(File.ReadAllBytes(filename));

        public virtual byte[] GetILCode() => ILCode;
    }
}