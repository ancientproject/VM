namespace ancient.runtime.exceptions
{
    using System;
    [Serializable]
    public class InvalidCharsException : Exception
    {
        public InvalidCharsException(string msg) : base(msg) { }
    }
}