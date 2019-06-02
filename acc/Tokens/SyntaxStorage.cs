namespace flame.compiler.tokens
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using runtime;
    using Sprache;

    public class SyntaxStorage
    {

        private static Dictionary<string, InsID> Keywords =>
            Enum.GetNames(typeof(InsID)).ToDictionary(x => x, Enum.Parse<InsID>);
        static readonly Dictionary<string, OperatorKind> Operators = new Dictionary<string, OperatorKind>
        {
            ["."] = OperatorKind.Dot,
            ["@>>"] = OperatorKind.PipeLeft,
            ["<<@"] = OperatorKind.PipeRight,
            ["&"] = OperatorKind.Ref,
            ["^"] = OperatorKind.AltRef,
            ["("] = OperatorKind.OpenParen,
            [")"] = OperatorKind.CloseParen,
        };

        public static Parser<Instruction> JumpT =>
            from dword in InstructionToken(InsID.jump_t)
            from space1 in Parse.WhiteSpace.Optional()
            from cell1 in RefToken
            select new jump_t(cell1.Cell);

        public static Parser<RefExpression> RefToken =
           from refSym in Parse.Char('&')
           from openParen in Parse.Char('(')
           from cellID in HexNumber
           from closeParen in Parse.Char(')')
           select new RefExpression(short.Parse(cellID, NumberStyles.AllowHexSpecifier));

        public static Parser<string> HexNumber =
            from zero in Parse.Char('0')
            from x in Parse.Chars("x")
            from number in Parse.Chars("0xABCDEF0123456789").Many().Text()
            select number;

        public static Parser<string> PipeLeft =>
            from dword in Parse.Char('@')
            from _1 in Parse.Char('>')
            from _2 in Parse.Char('>')
            select "";


        public static Parser<swap> SwapToken =>
            from dword in InstructionToken(InsID.swap)
            from space1 in Parse.WhiteSpace.Optional()
            from cell1 in RefToken
            from space2 in Parse.WhiteSpace.Optional()
            from cell2 in RefToken
            select new swap(cell1.Cell, cell2.Cell);

        public static Parser<Instruction> PushA =>
            from dword in InstructionToken(InsID.push_a)
            from space1 in Parse.WhiteSpace.Optional()
            from cell1 in RefToken.Token().Named("ref_token")
            from space2 in Parse.WhiteSpace.Optional()
            from cell2 in RefToken.Token().Named("ref_token")
            from space3 in Parse.WhiteSpace.Optional()
            from op2 in PipeLeft.Token().Named("pipe_operator")
            from space4 in Parse.WhiteSpace.Optional()
            from cell3 in RefToken.Token().Named("ref_token")
            select new push_a(cell1.Cell, cell2.Cell, cell3.Cell);

        public static Parser<Instruction> RefT =>
            from dword in InstructionToken(InsID.ref_t)
            from space1 in Parse.WhiteSpace.Optional()
            from cell1 in RefToken
            select new ref_t(cell1.Cell);

        


        public static Parser<IEnumerable<Instruction>> Parser =>
            (from many in
                    SwapToken.Token().Named("swap")
                        .Or(JumpT.Token().Named("jump_t"))
                        .Or(RefT.Token().Named("ref_t"))
                        .Or(PushA.Token().Named("push_a"))
                select many).End().Many();


        public static Parser<string> InstructionToken(InsID instruction) =>
            from dot in Parse.Char('.')
            from ident in Parse.String(instruction.ToString()).Text()
            select ident;
    }

    public class RefExpression
    {
        public readonly short Cell;

        public RefExpression(short cell) => Cell = cell;
    }

    public enum OperatorKind
    {
        Dot,
        PipeLeft,
        PipeRight,
        Ref,
        AltRef,
        OpenParen,
        CloseParen
    }

    public enum FlameSyntaxKind
    {
        Identifier,
        Operator,
        Number,
        String
    }
}