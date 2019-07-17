namespace ancient.runtime.tools
{
    using System;
    using System.Text;
    using static System.Environment;

    
    public static class ByteArrayUtils
    {
        private static readonly char[] HexdumpTable = new char[256 * 4];
        private static readonly string[] Byte2Hex = new string[256];
        private static readonly string[] HexPadding = new string[16];
        private static readonly string[] Byte2HexPad = new string[256];
        private static readonly string[] BytePadding = new string[16];
        private static readonly string[] Byte2HexNopad = new string[256];
        private static readonly char[] Byte2Char = new char[256];
        private static readonly string[] HexDumpRowPrefixes = new string[(int)((uint)65536 >> 4)];

        static ByteArrayUtils()
        {
            var digits = "0123456789abcdef".ToCharArray();
            for (var i = 0; i < 256; i++)
            {
                HexdumpTable[i << 1] = digits[(int)((uint)i >> 4 & 0x0F)];
                HexdumpTable[(i << 1) + 1] = digits[i & 0x0F];
            }
            // Generate the lookup table for hex dump paddings
            for (var i = 0; i < HexPadding.Length; i++)
            {
                var padding = HexPadding.Length - i;
                var buf = new StringBuilder(padding * 3);
                for (var j = 0; j < padding; j++) buf.Append("   ");
                HexPadding[i] = buf.ToString();
            }
            for (var i = 0; i < BytePadding.Length; i++)
            {
                var padding = BytePadding.Length - i;
                var buf = new StringBuilder(padding);
                for (var j = 0; j < padding; j++) buf.Append(' ');
                BytePadding[i] = buf.ToString();
            }
            {
                int i;
                for (i = 0; i < 10; i++)
                {
                    var buf = new StringBuilder(2);
                    buf.Append('0');
                    buf.Append(i);
                    Byte2HexPad[i] = buf.ToString();
                    Byte2HexNopad[i] = (i).ToString();
                }
                for (; i < 16; i++)
                {
                    var buf = new StringBuilder(2);
                    var c = (char)('A' + i - 10);
                    buf.Append('0');
                    buf.Append(c);
                    Byte2HexPad[i] = buf.ToString();
                    Byte2HexNopad[i] = c.ToString(); /* String.valueOf(c);*/
                }
                for (; i < Byte2HexPad.Length; i++)
                {
                    var buf = new StringBuilder(2);
                    buf.Append(i.ToString("X") /*Integer.toHexString(i)*/);
                    var str = buf.ToString();
                    Byte2HexPad[i] = str;
                    Byte2HexNopad[i] = str;
                }
            }
            string ByteToHexStringPadded(int value) => Byte2HexPad[value & 0xff];

            for (var i = 0; i < Byte2Hex.Length; i++) Byte2Hex[i] = $"{' '}{ByteToHexStringPadded(i)}";

            // Generate the lookup table for byte-to-char conversion
            for (var i = 0; i < Byte2Char.Length; i++)
            {
                if (i <= 0x1f || i >= 0x7f)
                    Byte2Char[i] = '.';
                else
                    Byte2Char[i] = (char) i;
            }

            // Generate the lookup table for the start-offset header in each row (up to 64KiB).
            for (var i = 0; i < HexDumpRowPrefixes.Length; i++)
            {
                var buf = new StringBuilder(12);
                buf.Append(NewLine);
                buf.Append((i << 4 & 0xFFFFFFFFL | 0x100000000L).ToString("X2"));
                buf.Insert(buf.Length - 9, '|');
                buf.Append('|');
                HexDumpRowPrefixes[i] = buf.ToString();
            }
        }

        public static string HexDump(byte[] buffer) 
            => HexDump(buffer, 0, buffer.Length);
        public static string HexDump(byte[] buffer, int fromIndex, int length)
        {
            if (length == 0) return "";
            var endIndex = fromIndex + length;
            var buf = new char[length << 1];
            var srcIdx = fromIndex;
            var dstIdx = 0;
            for (; srcIdx < endIndex; srcIdx++, dstIdx += 2)
                Array.Copy(HexdumpTable, buffer[srcIdx] << 1, buf, dstIdx, 2);
            return new string(buf);
        }

        public static string PrettyHexDump(byte[] buffer)
            => PrettyHexDump(buffer, 0, buffer.Length);
        public static string PrettyHexDump(byte[] buffer, int offset, int length)
        {
            if (length == 0)
                return string.Empty;
            var rows = length / 16 + (length % 15 == 0 ? 0 : 1) + 4;
            var buf = new StringBuilder(rows * 80);
            AppendPrettyHexDump(buf, buffer, offset, length);
            return buf.ToString();
        }

        public static void AppendPrettyHexDump(StringBuilder dump, byte[] buf, int offset, int length)
        {
            if (length == 0)
                return;

            dump.Append(
                "          +-------------------------------------------------+" +
                NewLine + "          |  0  1  2  3  4  5  6  7  8  9  a  b  c  d  e  f |" +
                NewLine + "+---------+-------------------------------------------------+----------------+");

            var startIndex = offset;
            var fullRows = (int)((uint)length >> 4);
            var remainder = length & 0xF;

            for (var row = 0; row < fullRows; row++)
            {
                var rowStartIndex = (row << 4) + startIndex;

                // Per-row prefix.
                AppendHexDumpRowPrefix(dump, row, rowStartIndex);

                // Hex dump
                var rowEndIndex = rowStartIndex + 16;
                for (var j = rowStartIndex; j < rowEndIndex; j++)
                    dump.Append(Byte2Hex[buf[j]]);
                dump.Append(" |");
                // ASCII dump
                for (var j = rowStartIndex; j < rowEndIndex; j++)
                    dump.Append(Byte2Char[buf[j]]);
                dump.Append('|');
            }

            if (remainder != 0)
            {
                var rowStartIndex = (fullRows << 4) + startIndex;
                AppendHexDumpRowPrefix(dump, fullRows, rowStartIndex);

                // Hex dump
                var rowEndIndex = rowStartIndex + remainder;
                for (var j = rowStartIndex; j < rowEndIndex; j++) dump.Append(Byte2Hex[buf[j]]);
                dump.Append(HexPadding[remainder]);
                dump.Append(" |");

                // Ascii dump
                for (var j = rowStartIndex; j < rowEndIndex; j++) dump.Append(Byte2Char[buf[j]]);
                dump.Append(BytePadding[remainder]);
                dump.Append('|');
            }

            dump.Append(NewLine + "+---------+-------------------------------------------------+----------------+");
        }

        private static void AppendHexDumpRowPrefix(StringBuilder dump, int row, int rowStartIndex)
        {
            if (row < HexDumpRowPrefixes.Length)
                dump.Append(HexDumpRowPrefixes[row]);
            else
            {
                dump.Append(NewLine);
                dump.Append((rowStartIndex & 0xFFFFFFFFL | 0x100000000L).ToString("X2"));
                dump.Insert(dump.Length - 9, '|');
                dump.Append('|');
            }
        }
    }
}