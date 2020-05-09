namespace ancient.compiler.tokens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sprache;
    using static Sprache.Result;

    public class None<T> : IOption<T>
    {
        public T GetOrDefault() => default;
        public T Get() => throw new NotImplementedException();
        public bool IsEmpty => true;
        public bool IsDefined => false;
    }
    public class Some<T> : IOption<T>
    {
        private readonly T _t;

        public Some(T t) => _t = t;
        public T GetOrDefault() => _t;
        public T Get() => _t;
        public bool IsEmpty => false;
        public bool IsDefined => true;
    }

    public static class ParserExtensions
    {

        public static T? Unwrap<T>(this IOption<T> t) where T : struct
        {
            if (t.IsDefined)
                return t.Get();
            return null;
        }
        public static Parser<object> Not<T>(this Parser<T> parser, string expectations)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            return i =>
            {
                var result = parser(i);
                if (!result.WasSuccessful)
                    return Success((object)null, i);
                var message = "`" + string.Join<string>(", ", result.Expectations) + "' was not expected";
                return Failure<object>(i, message, new []{ expectations });
            };
        }
        public static Parser<IOption<T>> OptionalWhenNotStart<T>(this Parser<T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            return (i =>
            {
                var result = parser(i);
                
                if (result.WasSuccessful)
                    return Success(new Some<T>(result.Value), result.Remainder) as IResult<IOption<T>>;
                if (result.Expectations.Any() && result.Remainder.Source.Length != result.Remainder.Position)
                    return Failure<None<T>>(result.Remainder, "", result.Expectations) as IResult<IOption<T>>;
                return Success(new None<T>(), i) as IResult<IOption<T>>;
            });
        }

        public static Parser<OperatorKind> NamedOperator(this Parser<OperatorKind> parser, OperatorKind kind)
        {
            return parser.Named(
                $"{kind} operator ({AssemblerSyntax.Operators.First(x => x.Value == kind).Key})");
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

        public static Parser<IEnumerable<IEvolveToken>> ContinueMany(this Parser<IEvolveToken> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var remainder = i;
                var result = new List<IEvolveToken>();
                var r = parser(i);

                while (true)
                {
                    if (remainder.Equals(r.Remainder)) break;
                    result.Add(r.WasSuccessful ? r.Value : new ErrorEvolveToken(r));
                    remainder = r.Remainder;
                    r = parser(remainder);
                }
                return Success<IEnumerable<IEvolveToken>>(result, remainder);
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
                    result.Add(r.WasSuccessful ? r.Value : new ErrorCompileToken(r));
                    remainder = r.Remainder;
                    r = parser(remainder);
                }
                return Success<IEnumerable<IInputToken>>(result, remainder);
            };
        }
    }
}