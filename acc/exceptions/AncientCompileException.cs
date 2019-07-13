namespace ancient.compiler.exceptions
{
    using System;
    [Serializable]
    public class AncientCompileException : Exception
    {
        public AncientCompileException(string msg) : base(msg){}
    }
}