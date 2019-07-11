namespace ancient.compiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using emit;
    using Pastel;
    using Pixie;
    using Pixie.Code;
    using Pixie.Markup;
    using Pixie.Terminal;
    using Pixie.Terminal.Devices;
    using Pixie.Terminal.Render;
    using runtime;
    using tokens;
    using static System.Console;
    using Color = System.Drawing.Color;

    internal class _term
    {
        private static readonly object Guarder = new object();

        public static void Trace(string message)
        {
            lock (Guarder)
            {
                WriteLine($"trace: {message}".Pastel( Color.Gray));
            }
        }

        public static void Success(string message)
        {
            lock (Guarder)
            {
                Write("[");
                Write($"SUCCESS".Pastel(Color.YellowGreen));
                Write("]: ");
                WriteLine($" {message}");
            }
        }
        public static void Warn(Warning keyCode, string message)
        {
            lock (Guarder)
            {
                Write("[");
                Write($"WARN".Pastel(Color.Orange));
                Write("]: ");
                Write($"{keyCode.Format()}".Pastel(Color.Orange));
                WriteLine($" {message}");
            }
        }
        public static string getFromMiddle(string value, int indexMiddle, int count, bool isStopWhenEmptySpace = false)
        {
            try
            {
                indexMiddle -= 1;
                var str = new List<char>();
                for (var i = indexMiddle; i != indexMiddle + count + 1; i++)
                {
                    if(value.Length <= i)
                        break;
                    if(value[i] == ' ' && isStopWhenEmptySpace)
                        break;
                    str.Add(value[i]);
                }
                for (var i = indexMiddle-1; i != indexMiddle - count - 1; i--)
                {
                    if(value[i] == ' ' && isStopWhenEmptySpace)
                        break;
                    str.Insert(0, value[i]);
                }
                return string.Join("", str.ToArray());
            }
            catch
            {
                return value;
            }
        }

        #region shit-code

        public static void Error<T>(ErrorToken<T> token, string source)
        {
            lock (Guarder)
            {
                _error(token, source);
            }
        }

        private static void _error<T>(ErrorToken<T> token, string source)
        {
            var col = token.ErrorResult.Remainder.Column;
            var lin = token.ErrorResult.Remainder.Line;
            var exp = token.ErrorResult.Expectations.First();
            var rem = token.ErrorResult.Remainder.Current;

            var nestedLine = source.Split('\n')[lin-1];
            var fuck = getFromMiddle(nestedLine, col, nestedLine.Length - col, true);
            var startOffset = source.IndexOf(nestedLine, StringComparison.InvariantCultureIgnoreCase);
            var nameOffset = (startOffset + col - 1);

            var doc2 = new StringDocument("", source);
            var highlightRegion = new SourceRegion(new SourceSpan(doc2, startOffset, nestedLine.Length));

            var focusRegion = new SourceRegion(
                new SourceSpan(doc2, nameOffset, fuck.Length));
            var title = $"{token.ErrorResult.getWarningCode().To<string>().Pastel(Color.Orange)}";
            var message = $"character '{exp}' expected".Pastel(Color.Orange);

            string Render(MarkupNode node, params NodeRenderer[] extraRenderers)
            {
                var writer = new StringWriter();
                var terminal = new TextWriterTerminal(writer, 160, Encoding.ASCII);
                var log = new TerminalLog(terminal).WithRenderers(extraRenderers);
                log.Log(node);
                return writer.ToString();
            }

            var result = Render(
                new HighlightedSource(highlightRegion, focusRegion), 
                new HighlightedSourceRenderer(3, Colors.Red));
            WriteLine($" :: {title} - {message}");
            var ses = result.Split('\n');
            var flag1 = false;
            foreach (var (value, index) in ses.Select((value, index) => (value, index)))
            {
                var next = ses.Select((value, index) => new {value, index}).FirstOrDefault(x => x.index == index + 1);
                if (next != null && (next.value.Contains('~') && next.value.Contains('^')) && !flag1)
                    WriteLine(value.Replace(fuck, fuck.Pastel(Color.Red)));
                else if (value.Contains('~') && value.Contains('^'))
                {
                    if (flag1) 
                        continue;
                    WriteLine(value.Pastel(Color.Red));
                    flag1 = true;
                }
                else
                    WriteLine($"{value} ");
            }
        }
        #endregion
        public static void Error(Warning keyCode, string message)
        {
            lock (Guarder)
            {
                Write("[");
                Write($"ERROR".Pastel(Color.Red));
                Write("]: ");
                Write($"{keyCode.Format()}".Pastel(Color.Red));
                WriteLine($" {message}");
            }
        }
    }
}