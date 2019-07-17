namespace ancient.runtime.emit.sys
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ExternSignature
    {
        public string Signature { get; set; }
        public ushort MethodIndex { get; set; }
        public IList<ExternType> Arguments { get; set; }
        public MethodInfo method { get; set; }


        public bool IsVoid() => method.ReturnType == typeof(void);

        public bool IsArgs() => method.GetParameters().Any();
    }
}