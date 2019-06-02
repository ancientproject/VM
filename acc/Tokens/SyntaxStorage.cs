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

        internal static Dictionary<string, InsID> Keywords =>
            Enum.GetNames(typeof(InsID)).ToDictionary(x => x, Enum.Parse<InsID>);
        internal static readonly Dictionary<string, OperatorKind> Operators = new Dictionary<string, OperatorKind>
        {
            ["."] = OperatorKind.Dot,
            ["@>>"] = OperatorKind.PipeLeft,
            ["<<@"] = OperatorKind.PipeRight,
            ["&"] = OperatorKind.Ref,
            ["^"] = OperatorKind.AltRef,
            ["("] = OperatorKind.OpenParen,
            [")"] = OperatorKind.CloseParen,
        };

        public static Parser<InstructionExpression> JumpT =>
            (from dword in InstructionToken(InsID.jump_t)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                select new InstructionExpression(new jump_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_t expression");
        public static Parser<RefExpression> RefToken =
           (from refSym in Parse.Char('&')
               from openParen in Parse.Char('(')
               from cellID in HexNumber
               from closeParen in Parse.Char(')')
               select new RefExpression(short.Parse(cellID, NumberStyles.AllowHexSpecifier)))
           .Token()
           .WithPosition()
           .Named("ref_token");
        public static Parser<string> HexNumber =
            (from zero in Parse.Char('0')
                from x in Parse.Chars("x")
                from number in Parse.Chars("0xABCDEF123456789").Many().Text()
                select number)
            .Token()
            .Named("hex number");
        public static Parser<OperatorKind> PipeLeft =>
            (from _ in Parse.String("@>>")
             select OperatorKind.PipeLeft)
            .Token()
            .NamedOperator();
        public static Parser<OperatorKind> PipeRight =>
            (from _ in Parse.String("<<@")
             select OperatorKind.PipeRight)
            .Token()
            .NamedOperator();
        public static Parser<InstructionExpression> SwapToken =>
            (from dword in InstructionToken(InsID.swap)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from space2 in Parse.WhiteSpace.Optional()
                from cell2 in RefToken
                select new InstructionExpression(new swap(cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("swap expression");
        public static Parser<InstructionExpression> PushA =>
            (from dword in InstructionToken(InsID.push_a)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new push_a(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("push_a expression");
        public static Parser<InstructionExpression> RefT => (
                from dword in InstructionToken(InsID.ref_t)
                from cell1 in RefToken
                select new InstructionExpression(new ref_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("ref_t expression");
        public static Parser<IInputToken[]> InstructionParser => (
            from many in SwapToken.Token().Named("swap")
                .Or(JumpT.Token().Named("jump_t"))
                .Or(RefT.Token().Named("ref_t"))
                .Or(PushA.Token().Named("push_a"))
            select many)
            .ContinueMany()
            .Select(x => x.ToArray());
        public static Parser<string> InstructionToken(InsID instruction) =>
            from dot in Parse.Char('.')
            from ident in Parse.String(instruction.ToString()).Text()
            select ident;
    }
}