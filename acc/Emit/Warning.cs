namespace flame.compiler.emit
{
    using System;
    using System.Linq;
    using Sprache;

    public enum Warning
    {
        Undefined = -1,
        InternalError = 1,  
        NoSource = 2,
        CouldNotWrite = 3,
        OutFileNotSpecified = 4,
        SourceFileNotFound = 5,
        UnexpectedToken = 6,
        IgnoredToken
    }
    public static class WarningEx
    {
        public static string Format(this Warning w) => $"FC{(int)w:0000}";

        public static Warning getWarningCode<T>(this IResult<T> s)
        {
            var expect = s.Expectations.First();
            // TODO
            return Warning.UnexpectedToken;
        }
    }
}