namespace ancient.runtime.exceptions
{
    using System;
    [Serializable]
    public class CorruptedMemoryException : Exception
    {
        public CorruptedMemoryException() { }
        public CorruptedMemoryException(string msg) : base(msg) { }
        public CorruptedMemoryException(string msg, Exception e) : base(msg, e) { }
    }
}