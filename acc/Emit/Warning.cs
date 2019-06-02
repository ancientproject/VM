namespace flame.compiler.emit
{
    using System;

    public enum Warning
    {
        Undefined = -1,
        InternalError = 1,  
        NoSource = 2,
        CouldNotWrite = 3,
        OutFileNotSpecified = 4,
        SourceFileNotFound = 5,
    }
    public static class WarningEx
    {
        public static string Format(this Warning w) => $"FC{(int)w:####}";
    }
}