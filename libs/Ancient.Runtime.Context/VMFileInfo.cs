namespace ancient.runtime.context
{
    using System.IO;

    public class VMFileInfo : FileSystemInfo
    {
        public VMFileInfo(string fileName)
        {
            Name = fileName;
            try
            {
                raw = new FileInfo(fileName);
                Exists = raw.Exists;
            }
            catch { }
        }

        private FileInfo raw { get; }

        public override void Delete() => raw?.Delete();

        public override bool Exists { get; }
        public override string Name { get; }
        public override string ToString() => Name;
    }
}