namespace Ancient.ProjectSystem
{
    using System;
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;

    public class AncientLockFile
    {
        [JsonProperty(Order = 0)]
        public string IS_AUTO_GENERATED { get; set; } = "DON'T TOUCH THIS FILE";

        public AncientLockFile() {}
        public AncientLockFile(Assembly asm, string Registry)
        {
            id = Path.GetFileNameWithoutExtension(asm.GetName().Name);
            registry = Registry;
            version = asm.GetName().Version;
        }

        public string id { get; set; }
        public string registry { get; set; }
        public Version version { get; set; }
        public string platform { get; set; } = "any";
    }
}