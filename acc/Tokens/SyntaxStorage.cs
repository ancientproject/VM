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

        public static Parser<IInputToken[]> InstructionParser => (
                from many in
                    SwapToken
                        .Or(JumpT)
                        .Or(RefT)
                        .Or(PushA)
                        .Or(LoadI)
                        .Or(ByIIDToken(InsID.halt))
                        .Or(ByIIDToken(InsID.warm))
                select many)
            .ContinueMany()
            .Select(x => x.ToArray());

        public static Parser<char> CharToken =
            (from _1 in Parse.Char('\'')
                from @char in Parse.AnyChar
                from _2 in Parse.Char('\'')
             select @char)
            .Token()
            .Named("char token");
        public static Parser<string> HexNumber =
            (from zero in Parse.Char('0')
                from x in Parse.Chars("x")
                from number in Parse.Chars("0xABCDEF123456789").Many().Text()
                select number)
            .Token()
            .Named("hex number");

        #region Operator tokens
        public static Parser<OperatorKind> PipeLeft =>
            (from _ in Parse.String("@>>")
                select OperatorKind.PipeLeft)
            .Token()
            .NamedOperator(OperatorKind.PipeLeft);
        public static Parser<OperatorKind> PipeRight =>
            (from _ in Parse.String("<<@")
                select OperatorKind.PipeRight)
            .Token()
            .NamedOperator(OperatorKind.PipeRight);
        public static Parser<RefExpression> RefToken =
            (from refSym in Parse.Char('&')
                from openParen in Parse.Char('(')
                from cellID in HexNumber
                from closeParen in Parse.Char(')')
                select new RefExpression(short.Parse(cellID, NumberStyles.AllowHexSpecifier)))
            .Token()
            .WithPosition()
            .Named("ref_token");
        public static Parser<RefExpression> ValueToken =
            (from refSym in Parse.Char('$')
                from openParen in Parse.Char('(')
                from cellID in HexNumber
                from closeParen in Parse.Char(')')
                select new RefExpression(short.Parse(cellID, NumberStyles.AllowHexSpecifier)))
            .Token()
            .WithPosition()
            .Named("value_token");
        public static Parser<short> CastCharToken =
            (from refSym in Parse.String("@char_t")
             from openParen in Parse.Char('(')
                from @char in CharToken
             from closeParen in Parse.Char(')')
                select (short)@char)
            .Token()
            .Named("char_t expression");
        #endregion
        #region Instructuions token
        public static Parser<InstructionExpression> LoadI =>
            (from dword in InstructionToken(InsID.loadi)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from cell2 in ValueToken
                select new InstructionExpression(new loadi(cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("loadi expression");
        public static Parser<InstructionExpression> JumpT =>
            (from dword in InstructionToken(InsID.jump_t)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                select new InstructionExpression(new jump_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_t expression");
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
                from cell3 in ValueToken.Or(CastCharToken.Select(x => new RefExpression(x)))
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
        #endregion
        #region etc tokens
        public static Parser<string> InstructionToken(InsID instruction) =>
            from dot in Parse.Char('.')
            from ident in Parse.String(instruction.ToString()).Text()
            select ident;
        public static Parser<InstructionExpression> ByIIDToken(InsID id) =>
            (from dword in InstructionToken(id)
                select new InstructionExpression(Instruction.Summon(id)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        #endregion

    }
}