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
    using exceptions;
    using runtime.emit;
    using tokens;
    using TrueColorConsole;
    using static _term;
    using static TrueColorConsole.VTConsole;
    internal class Host
    {
        public static void Main(string[] c_args)
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => { VTConsole.Disable(); };
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

            var ver = FileVersionInfo.GetVersionInfo(typeof(Host).Assembly.Location).ProductVersion;
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


            try
            {
                var e = Evolve(source);
                var c = Compile(e, args);

                File.WriteAllBytes($"{args.OutFile}.dlx", c.data);
                File.WriteAllText($"{args.OutFile}.map", c.map);
            }
            catch (FlameCompileException)
            { }
            catch (FlameEvolveException)
            { }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static string Evolve(string code)
        {
            var result = code;
            var block = code.Replace("\r", "").Split('\n');
            var parsed = FlameTransformerSyntax.ManyParser.Parse(code);
            foreach (var token in parsed)
            {
                switch (token)
                {
                    case ClassicEvolve e:
                        result = result.Replace(
                            block[token.InputPosition.Line-1], 
                            string.Join("\n", e.Result));
                        break;
                    case EmptyEvolve _:
                        break;
                    case ErrorEvolveToken error:
                        Error(error.ErrorResult.getWarningCode(), error.ErrorResult.ToString());
                        throw new FlameEvolveException(error.ErrorResult.ToString());
                }
            }
            return result;
        }

        public static (byte[] data, string map) Compile(string source, Args args)
        {
            var @try = FlameAssemblerSyntax.ManyParser.Parse(source);
            
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
                        $"0x{value:X16} // Offset: 0x{offset:X8}, ID: {token.ID}, OpCode: 0x{token.OPCode:X2}";
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
                    case ErrorCompileToken error:
                        Error(error.ErrorResult.getWarningCode(), error.ErrorResult.ToString());
                        throw new FlameCompileException(error.ErrorResult.ToString());
                    case CommentToken comment:
                        // ignore
                        break;
                    default:
                        Warn(Warning.IgnoredToken, $"Ignored {expression} at {expression.InputPosition}");
                        break;
                }
            }

            return (asm.GetBytes(), map.ToString());
        }


        internal class Args
        {
            public List<string> sourceFiles { get; set; }
            public string OutFile { get; set; }
        }
    }
}
