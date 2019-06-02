namespace flame.compiler.tokens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sprache;

    public static class ParserExtensions
    {
        public static Parser<OperatorKind> NamedOperator(this Parser<OperatorKind> parser)
        {
            return i =>
            {
                var value = parser(i);
                return parser.Named(
                    $"{value.Value} operator ({SyntaxStorage.Operators.First(x => x.Value == value.Value).Key})")(i);
            };
        }

        public static Parser<T> WithPosition<T>(this Parser<T> parser) where T : class, IInputToken
        {
            return i =>
            {
                var r = parser(i);
                if (r.WasSuccessful)
                    r.Value.InputPosition = new Position(i.Position, i.Line, i.Column);

                return r;
            };
        }
        public static Parser<IEnumerable<IInputToken>> ContinueMany(this Parser<IInputToken> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var remainder = i;
                var result = new List<IInputToken>();
                var r = parser(i);

                while (true)
                {
                    if (remainder.Equals(r.Remainder)) break;
                    result.Add(r.WasSuccessful ? r.Value : new ErrorToken(r));
                    remainder = r.Remainder;
                    r = parser(remainder);
                }
                return Result.Success<IEnumerable<IInputToken>>(result, remainder);
            };
        }
    }
}