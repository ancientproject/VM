namespace rune.etc
{
    using System;
    using System.IO;

    public static class Dirs
    {
        public static DirectoryInfo RootFolder =>
            new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".rune"));
        public static FileInfo ConfigFile =>
            new FileInfo(Path.Combine(RootFolder.FullName, "@.ini"));


        public static void Ensure()
        {
            if(!RootFolder.Exists)
                RootFolder.Create();
        }
    }
}