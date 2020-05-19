namespace ancient.compiler.tokens
{
    using System.Linq;
    using Sprache;

    public partial class AssemblerSyntax
    {
        public virtual Parser<string> TypeToken =>
            Parse.String("u8").Text().Or(
                Parse.String("u16").Text()).Or(
                Parse.String("u32").Text()).Or(
                Parse.String("u64").Text()).Or(
                Parse.String("f64").Text()).Or(
                Parse.String("void").Text()).Or(
                Parse.String("str").Text()).Or(
                Parse.String("u2").Text()).Token();

        public virtual Parser<IInputToken[]> ManyParser => (
                from many in
                    Parser
                select many)
            .ContinueMany()
            .Select(x => x.ToArray());
        /// <summary>
        /// Comment token
        /// </summary>
        /// <example>
        /// CharToken.Parse("; this is single-line comment");
        /// </example>
        public virtual Parser<CommentToken> CommentToken =>
            (from comment in new CommentParser(";", "{|", "|}", "\n").SingleLineComment
             select new CommentToken(comment))
            .Token()
            .Named("comment token");
        /// <summary>
        /// Single char wrapped in quote character
        /// </summary>
        /// <example>
        /// CharToken.Parse("'1'");
        /// </example>
        public virtual Parser<char> CharToken =>
            (from @char in Wrap(Parse.AnyChar, Parse.Char('\''))
             select @char)
            .Token()
            .Named("char token");
        /// <summary>
        /// Single quote wrapped identifier token
        /// </summary>
        /// <example>
        /// QuoteIdentifierToken.Parse("'test identifier token'");
        /// </example>
        public virtual Parser<string> QuoteIdentifierToken => (
                from @string in Wrap(Parse.AnyChar.Except(Parse.Char('\'')).Many().Text(), Parse.Char('\''))
                select @string)
            .Token()
            .Named("quote string token");
        /// <summary>
        /// string wrapped in double quote chars
        /// </summary>
        /// <example>
        /// StringToken.Parse("\"str\"") -> "str"
        /// </example>
        public virtual Parser<string> StringToken => (
            from @string in Wrap(Parse.AnyChar.Except(Parse.Char('"')).Many().Text(), Parse.Char('"'))
            select @string)
            .Token().Named("string token");

        /// <summary>
        /// Keyword token
        /// </summary>
        /// <param name="keyword">
        /// keyword string
        /// </param>
        /// <example>
        /// Keyword("var").Parse("var")
        /// </example>
        public virtual Parser<string> Keyword(string keyword) =>
            (from word in Parse.String(keyword).Text() select word)
                .Token().Named($"keyword {keyword} token");
        /// <summary>
        /// Float number token
        /// </summary>
        /// <example>
        /// FloatToken.Parse("12.45") -> "12.45"
        /// FloatToken.Parse("-12.45") -> "-12.45"
        /// </example>
        public virtual Parser<string> FloatToken => (
                from minus in Parse.Char('-').Optional()
                from @string in Parse.DecimalInvariant
                select $"{minus.GetOrElse('+')}{@string}")
            .Token().Named("string token");

        /// <summary>
        /// hex number token
        /// </summary>
        /// <example>
        /// HexToken.Parse("0xDA") -> DA
        /// </example>
        public virtual Parser<string> HexToken =>
            (from zero in Parse.Char('0')
             from x in Parse.Chars("x")
             from number in Parse.Chars("ABCDEF1234567890").Many().Text()
             select number)
            .Token()
            .Named("hex number");
    }
}