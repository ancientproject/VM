namespace System.Linq
{
    using Collections.Generic;
    using IO;

    public static class Etc
    {
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source, out long Length)
        {
            var array = source.ToArray();
            Length = array.LongLength;
            return array;
        }

        public static byte[] ReadBytes(this MemoryStream stream, int size)
        {
            var bytes = new List<byte>(size);
            for (var i = 0; i < size; i++)
                bytes.Add((byte)stream.ReadByte());
            return bytes.ToArray();
        }
    }
}