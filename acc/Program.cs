namespace flame.compiler
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using emit;
    using Fclp;
    using runtime;
    using Sprache;
    using tokens;
    using static TrueColorConsole.VTConsole;
    using static _term;

    internal class Program
    {
        public static void Main(string[] c_args)
        {
            var raw = new FluentCommandLineParser<Args>();
            raw.Setup(x => x.sourceFiles)
                .As('s', "source")
                .WithDescription("Source files.")
                .SetDefault(new List<string>());
            raw.Setup(x => x.OutFile)
                .As('o', "out")
                .WithDescription("Out file.");
            raw.Parse(c_args);
            var args = raw.Object;
            

            Enable();
            CursorSetVisibility(false);
            CursorSetBlinking(false);

            WriteLine($"Flame Assembler Compiler version 0.0.0.0 (default)", Color.Gray);
            WriteLine($"Copyright (C) Yuuki Wesp.\n\n", Color.Gray);

            if (!args.sourceFiles.Any())
            {
                Warn(Warning.NoSource, "No source files specified.");
                return;
            }
            if (string.IsNullOrEmpty(args.OutFile))
            {
                Error(Warning.OutFileNotSpecified, "Outputs without source must have the --out option specified.");
                return;
            }

            if (!args.sourceFiles.Select(x => new FileInfo(x).Exists).All(x => x))
            {
                Error(Warning.SourceFileNotFound, "One source file not found.");
                return;
            }

            var source = File.ReadAllText(args.sourceFiles.First()).Replace("\r", "");

            var @try = SyntaxStorage.InstructionParser.Parse(source);

            using var mem = new MemoryStream();
            var map = new StringBuilder();
            var offset = 0;
            foreach (var expression in @try)
            {
                if (expression is InstructionExpression iExp)
                {
                    var token = iExp.Instruction;
                    offset++;
                    var value = (ushort)token.Assembly();
                    var bytes = BitConverter.GetBytes(value);
                    mem.Write(bytes);
                    var str =
                        $"0x{value:X7} // Offset: 0x{offset:X8}, ID: {token.ID}, OpCode: 0x{token.OPCode:X8}";
                    map.AppendLine(str);
                    Trace(str);
                }
                if (expression is ErrorToken error)
                {
                    Error(error.ErrorResult.getWarningCode(), error.ErrorResult.ToString());
                    return;
                }
            }
            File.WriteAllBytes($"{args.OutFile}.bin", mem.ToArray());
            File.WriteAllText($"{args.OutFile}.map", map.ToString());
        }


        internal class Args
        {
            public List<string> sourceFiles { get; set; }
            public string OutFile { get; set; }
        }
    }
}
