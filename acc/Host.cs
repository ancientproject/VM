namespace ancient.compiler
{
    using System;
    using emit;
    using Fclp;
    using runtime;
    using Sprache;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Ancient.Runtime.tools;
    using exceptions;
    using Pastel;
    using runtime.emit;
    using tokens;
    using static System.Console;
    using static _term;
    using Color = System.Drawing.Color;

    internal class Host
    {
        public static int Main(string[] c_args)
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => { ConsoleExtensions.Disable(); };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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

            var ver = FileVersionInfo.GetVersionInfo(typeof(Host).Assembly.Location).ProductVersion;
            
            WriteLine($"Ancient assembler compiler version {ver} (default)".Pastel(Color.Gray));
            WriteLine($"Copyright (C) Yuuki Wesp.\n\n".Pastel(Color.Gray));
            
            if (!args.sourceFiles.Any())
            {
                Warn(Warning.NoSource, "No source files specified.");
                return 1;
            }
            if (string.IsNullOrEmpty(args.OutFile))
            {
                Error(Warning.OutFileNotSpecified, "Outputs without source must have the --out option specified.");
                return 1;
            }

            if (!args.sourceFiles.Select(x => new FileInfo(x).Exists).All(x => x))
            {
                Error(Warning.SourceFileNotFound, "One source file not found.");
                return 1;
            }



            var source = File.ReadAllText(args.sourceFiles.First()).Replace("\r", "");
            
            try
            {
                var e = Evolve(source);
                var c = Compile(e, args);

                File.WriteAllBytes($"{args.OutFile}.dlx", c.data);
                File.WriteAllBytes($"{args.OutFile}.pdb", c.map);
                return 0;
            }
            catch (AncientCompileException)
            { }
            catch (AncientEvolveException)
            { }
            catch (Exception e)
            {
                WriteLine(e);
            }

            return 1;
        }

        public static string Evolve(string code)
        {
            var result = code;
            var block = code.Replace("\r", "").Split('\n');
            var parsed = new FlameTransformerSyntax().ManyEvolver.Parse(code);
            foreach (var token in parsed)
            {
                switch (token)
                {
                    case EmptyEvolve _: break;
                    default: Trace($"evolving :: {token}"); break;
                }
                switch (token)
                {
                    case ClassicEvolve e:
                        result = result.Replace(
                            block[token.InputPosition.Line-1], 
                            string.Join("\n", e.Result));
                        break;
                    case EmptyEvolve _:
                        break;
                    case DefineLabels labels:
                        var reg = new Regex(@"\!\[~(?<alias>\w+)\]");

                        var usedAliases = reg.Matches(code).Select(x => x.Groups["alias"].Value).ToArray();
                        var compiledAliases = labels.Labels.Select(x => x.Name).ToList();

                        var failedFind = usedAliases.Where(x => !compiledAliases.Contains(x)).ToArray();

                        if (failedFind.Any())
                        {
                            var msg = $"[{string.Join(",", failedFind)}] symbols not compiled.";
                            Error(Warning.PrecompiledSymbolNotFound, msg);
                            throw new AncientEvolveException(msg);
                        }

                        result = labels.Labels.Aggregate(result, (current, aliase) => 
                                current.Replace($"![~{aliase.Name}]", $"0x{aliase.Hex}"));
                        result = new Regex(@"^#\{.+\}$", 
                            RegexOptions.Compiled | 
                            RegexOptions.Multiline | 
                            RegexOptions.Singleline)
                            .Replace(result, "");
                        break;
                    case ErrorEvolveToken error:
                        Error(error, code);
                        throw new AncientEvolveException(error.ErrorResult.ToString());
                }
            }
            return result;
        }
        public static (byte[] data, byte[] map) Compile(string source, Args args)
        {
            var @try = new FlameAssemblerSyntax().ManyParser.Parse(source);
            var map = new DebugSymbols();
            var offset = 0;
            var asm = new DynamicAssembly(args.OutFile, ("timestamp", $"{DateTime.UtcNow.Ticks}"));
            var gen = asm.GetGenerator();

            foreach (var expression in @try)
            {
                void CompileToken(Instruction token)
                {
                    map.symbols.Add(((short)offset, source.Split('\n')[offset]));
                    offset++;
                    gen.Emit(token);
                    var value = (uint)token.Assembly();
                    var str = $"0x{value:X16}, offset: 0x{offset:X3}, op-code: 0x{token.OPCode:X2}, id: {token.ID}";
                    Trace($"compile :: {str}");
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
                        Error(error, source);
                        throw new AncientCompileException(error.ErrorResult.ToString());
                    case CommentToken _:
                        break;
                    default:
                        Warn(Warning.IgnoredToken, $"Ignored {expression} at {expression.InputPosition}");
                        break;
                }
            }

            return (asm.GetBytes(), DebugSymbols.ToBytes(map));
        }


        internal class Args
        {
            public List<string> sourceFiles { get; set; }
            public string OutFile { get; set; }
        }
    }
}
