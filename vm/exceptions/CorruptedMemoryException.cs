namespace vm.exceptions
{
    using System;

    public class CorruptedMemoryException : Exception
    {
        public CorruptedMemoryException() { }
        public CorruptedMemoryException(string msg) : base(msg) { }
    }
}