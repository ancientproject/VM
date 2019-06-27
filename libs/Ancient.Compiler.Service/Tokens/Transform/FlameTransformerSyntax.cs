namespace ancient.compiler.tokens
{
    using System.Linq;
    using runtime;
    using Sprache;

    public class FlameTransformerSyntax : FlameAssemblerSyntax
    {
        public virtual Parser<IEvolveToken> PushJ =>
            (from dword in InstructionToken(InsID.push_j)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeRight
                from cell3 in CastStringToken
                select new PushJEvolve(cell3, cell1.Cell, cell2.Cell))
            .Token()
            .WithPosition()
            .Named("push_j transform expression");

        public virtual Parser<IEvolveToken[]> Group(Parser<IEvolveToken> @group) => 
            from s in Parse.String("#{").Text()
            from g in @group.AtLeastOnce()
            from end in Parse.Char('}')
            select g.ToArray();

        public virtual Parser<IEvolveToken> Label =>
            (from dword in ProcToken("label")
                from name in QuoteIdentifierToken
                from hex in HexNumber
                from auto in Keyword("auto").Optional()
                select new DefineLabel(name, hex))
            .Token()
            .Named("label token");

        public override Parser<string> HexNumber =>
            (from zero in Parse.Char('0')
                from x in Parse.Chars("x")
                from number in Parse.Chars("0xABCDEF123456789").Many().Text()
                select number)
            .Token()
            .Named("hex number").Or(RefLabel);
        public virtual Parser<string> RefLabel =>
            (from sym in Parse.String("![~")
                from name in Parse.LetterOrDigit.Many().Text()
                from end in  Parse.String("]")
                select name).Token().Named("ref_label token");
        public Parser<IEvolveToken> Evolver =>
            Parser.Return(new EmptyEvolve())
                .Or(PushJ)
                .Or(Group(Label).Select(x => new DefineLabels(x)));

        public Parser<IEvolveToken[]> ManyEvolver => 
            (from many in Evolver select many)
            .ContinueMany()
            .Select(x => x.ToArray());
    }
}