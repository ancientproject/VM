namespace ancient.compiler.tokens
{
    using System.Globalization;
    using System.Linq;
    using runtime;
    using runtime.emit.sys;
    using Sprache;

    public class TransformerSyntax : AssemblerSyntax
    {
        public virtual Parser<IEvolveToken> PushJ =>
            (from dword in InstructionToken(IID.mvj)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeRight
                from cell3 in CastStringToken
                select new PushJEvolve(cell3, cell1.Cell, cell2.Cell))
            .Token()
            .WithPosition()
            .Named("mvj transform expression");

        public override Parser<string> HexToken =>
            (from zero in Parse.Char('0')
                from x in Parse.Chars("x")
                from number in Parse.Chars("0xABCDEF123456789").Many().Text()
                select number)
            .Token()
            .Named("hex number").Or(RefLabel);
        public virtual Parser<string> RefLabel =>
            (from sym in Parse.String("![~")
                from name in Parse.LetterOrDigit.Or(Parse.Char('_')).Many().Text()
                from end in  Parse.String("]")
                select name).Token().Named("ref_label token");
        public Parser<IEvolveToken> Evolver =>
            PushJ
                .Or(Locals)
                .Or(Group(Label).Select(x => new DefineLabels(x)))
                .Or(Parser.Return(new EmptyEvolve()));


        public Parser<IEvolveToken[]> ManyEvolver => 
            (from many in Evolver select many)
            .ContinueMany()
            .Select(x => x.ToArray());
    }
}