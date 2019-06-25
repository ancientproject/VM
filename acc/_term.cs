namespace flame.compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using emit;
    using Pastel;
    using Pixie;
    using Pixie.Code;
    using Pixie.Markup;
    using Pixie.Terminal;
    using Pixie.Terminal.Render;
    using Pixie.Transforms;
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
            var str = new List<char>();
            for (var i = indexMiddle; i != indexMiddle + count + 1; i++)
            {
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
        public static void Error(ErrorCompileToken error, string source)
        {
            static LogEntry MakeDiagnostic(LogEntry entry) => 
                DiagnosticExtractor.Transform(entry, new Text("program"));

            ILog log = TerminalLog.Acquire().WithRenderers(new HighlightedSourceRenderer(5));
            log = new Pixie.TransformLog(
                log,
                new Func<LogEntry, LogEntry>[]
                {
                    MakeDiagnostic
                });
            foreach (Match match in new Regex(@"\;.*").Matches(source))
                source = source.Replace(match.Value, match.Value.Pastel(Color.DarkOliveGreen));
                        
            var col = error.ErrorResult.Remainder.Column;
            var lin = error.ErrorResult.Remainder.Line;
            var exp = error.ErrorResult.Expectations.First();
            var rem = error.ErrorResult.Remainder.Current;

            var nestedLine = source.Split('\n')[lin-1];
            var fuck = getFromMiddle(nestedLine, col, 2, true);
            var ctorStartOffset = source.IndexOf(nestedLine, StringComparison.InvariantCultureIgnoreCase);
            var ctorNameOffset = source.IndexOf(fuck, StringComparison.InvariantCultureIgnoreCase);

            var doc2 = new StringDocument("", source);
            var highlightRegion = new SourceRegion(new SourceSpan(doc2, ctorStartOffset, nestedLine.Length))
                .ExcludeCharacters(char.IsWhiteSpace);

            var focusRegion = new SourceRegion(
                new SourceSpan(doc2, ctorNameOffset, fuck.Length));
            var title = $"{error.ErrorResult.getWarningCode().To<string>().Pastel(Color.Orange)}";
            var message = $"character '{exp}' expected".Pastel(Color.Orange);
            log.Log(
                new LogEntry(
                    Severity.Error,
                    "", 
                    new Text($"{title} - {message}"),
                    new HighlightedSource(highlightRegion, focusRegion)));
        }
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