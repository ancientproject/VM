namespace rune.etc
{
    using System.IO;

    public static class Extensions
    {
        public static string ReadToEnd(this FileInfo info)
        {
            if(!info.Exists)
                throw new FileNotFoundException($"'{info.FullName}' not exist.");
            return info.OpenText().ReadToEnd();
        }
    }
}