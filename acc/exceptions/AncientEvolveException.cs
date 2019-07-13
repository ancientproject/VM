namespace ancient.compiler.exceptions
{
    using System;
    [Serializable]
    public class AncientEvolveException : Exception
    {
        public AncientEvolveException(string msg) : base(msg){}
    }
}