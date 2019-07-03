namespace ancient.runtime
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class OpCodeAttribute : Attribute
    {
        private readonly bool _isIgnore;
        public readonly short OpCode;
        public OpCodeAttribute(short op) => OpCode = op;
        public OpCodeAttribute(bool isIgnore)
        {
            _isIgnore = isIgnore;
            OpCode = short.MinValue;
        }
    }
}