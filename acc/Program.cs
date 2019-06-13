namespace flame.compiler
{
    using System;
    using emit;
    using Fclp;
    using runtime;
    using Sprache;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using runtime.emit;
    using tokens;
    using static _term;
    using static TrueColorConsole.VTConsole;
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

            var ver = FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).ProductVersion;
            WriteLine($"Flame Assembler Compiler version {ver} (default)", Color.Gray);
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
            
            var map = new StringBuilder();
            var offset = 0;
            var asm = new DynamicAssembly(args.OutFile, ("timestamp", $"{DateTime.UtcNow.Ticks}"));
            var gen = asm.GetGenerator();

            foreach (var expression in @try)
            {
                void CompileToken(Instruction token)
                {
                    offset++;
                    gen.Emit(token);
                    var value = (uint)token.Assembly();
                    var str =
                        $"0x{value:X8} // Offset: 0x{offset:X8}, ID: {token.ID}, OpCode: 0x{token.OPCode:X4}";
                    map.AppendLine(str);
                    Trace($"Compile {str}");
                }

                switch (expression)
                {
                    case InstructionExpression iExp:
                        CompileToken(iExp.Instruction);
                        break;
                    case TransformationContext ctx:
                    {
                        foreach (var ins in ctx.Instructions)
                            CompileToken(ins);
                        break;
                    }
                    case ErrorToken error:
                        Error(error.ErrorResult.getWarningCode(), error.ErrorResult.ToString());
                        return;
                    case CommentToken comment:
                        // ignore
                        break;
                    default:
                        Warn(Warning.IgnoredToken, $"Ignored {expression} at {expression.InputPosition}");
                        break;
                }
            }
            
            File.WriteAllBytes($"{args.OutFile}.dlx", asm.GetBytes());
            File.WriteAllText($"{args.OutFile}.map", map.ToString());
        }


        internal class Args
        {
            public List<string> sourceFiles { get; set; }
            public string OutFile { get; set; }
        }
    }
}
