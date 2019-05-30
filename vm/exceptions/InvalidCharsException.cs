namespace vm.exceptions
{
    using System;

    public class InvalidCharsException : Exception
    {
        public InvalidCharsException(string msg) : base(msg) { }
    }
}