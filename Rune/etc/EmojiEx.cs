namespace Rune.cli
{
    using System;
    using System.Drawing;
    using Pastel;

    public static class StringEx
    {
        public static string Emoji(this string str)
        {
            if (Environment.GetEnvironmentVariable("RUNE_EMOJI_USE") == "0")
                return "";
            return EmojiOne.EmojiOne.ShortnameToUnicode(str);
        }

        public static string Color(this string str, Color color)
        {
            if (Environment.GetEnvironmentVariable("RUNE_COLOR_USE") == "0")
                return str;
            return str.Pastel(color);
        }
    }
}