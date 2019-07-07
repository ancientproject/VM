namespace Ancient.ProjectSystem
{
    using System;

    public class AncientLockFile
    {
        public string id { get; set; }
        public string registry { get; set; }
        public Version version { get; set; }
        public string platform { get; set; } = "any";
    }
}