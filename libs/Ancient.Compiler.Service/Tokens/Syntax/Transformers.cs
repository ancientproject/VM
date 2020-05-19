namespace ancient.compiler.tokens
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using runtime;
    using runtime.emit.sys;
    using Sprache;

    public partial class AssemblerSyntax
    {
        public virtual Parser<IInputToken> Signature =>
            (from dword in InstructionToken(IID.sig)
                from name in (from c in Parse.Char('@')
                    from name in Parse.AnyChar.Except(Parse.Char('(')).Many().Text()
                    select name)
                from args in (
                    from open in Parse.Char('(')
                    from list in TypeListToken
                    from close in Parse.Char(')')
                    select list
                ).Token().Named("argument list")
                from lambda in Parse.String("->").Token().Named("lambda")
                from type in TypeToken
                select new SignatureEvolve(args.ToList(), name, type))
            .Token()
            .WithPosition()
            .Named($"sig expression");

        private Parser<IEnumerable<string>> TypeListToken =>
        (
            from type in TypeToken
            select type
        ).DelimitedBy(Parse.Char(','));

        public virtual Parser<IEvolveToken> Locals =>
            (from dword in InstructionToken(IID.locals)
             from init in Parse.String("init").Token()
             from s in Parse.String("#(").Text()
             from g in (
                 from hex in (from c1 in Parse.Char('[')
                              from hex in HexToken
                              from c2 in Parse.Char(']')
                              select hex).Token().Named("hex tail token")
                 from type in
                     Parse.String("u8").Text().Or(
                         Parse.String("u16").Text()).Or(
                         Parse.String("u32").Text()).Or(
                         Parse.String("u64").Text()).Or(
                         Parse.String("f64").Text()).Or(
                         Parse.String("str").Text()).Or(
                         Parse.String("u2").Text())
                 from dd in Parse.Char(',').Optional()
                 select new EvaluationSegment(byte.Parse(hex, NumberStyles.AllowHexSpecifier), type)
             ).Token().Named("segment evaluation stack token").AtLeastOnce()
             from end in Parse.Char(')')
             select new LocalsInitEvolver(g.ToArray()))
            .Token()
            .WithPosition()
            .Named("locals transform expression");


        public virtual Parser<IEvolveToken[]> Group(Parser<IEvolveToken> @group) =>
            from s in Parse.String("#{").Text()
            from g in @group.AtLeastOnce()
            from end in Parse.Char('}')
            select g.ToArray();

        public virtual Parser<IEvolveToken> Label =>
            (from dword in ProcToken("label")
             from name in QuoteIdentifierToken
             from hex in HexToken
             from auto in Keyword("auto").Optional()
             select new DefineLabel(name, hex))
            .Token()
            .Named("label token");
    }
}