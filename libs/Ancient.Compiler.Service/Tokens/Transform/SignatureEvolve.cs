namespace ancient.compiler.tokens
{
    using System.Collections.Generic;
    using System.Linq;
    using runtime;
    using runtime.emit.sys;

    public class SignatureEvolve : ClassicEvolve
    {
        internal readonly List<string> _argumentTypes;
        internal readonly string _signatureName;
        private readonly string _returnType;

        public SignatureEvolve(List<string> argumentTypes, string signatureName, string returnType)
        {
            _argumentTypes = argumentTypes;
            _signatureName = signatureName;
            _returnType = returnType;
        }


        protected override void OnBuild(List<Instruction> jar)
        {
            jar.AddRange(new Instruction[]
            {
                new sig(_signatureName, _argumentTypes.Count, _returnType),
                new lpstr(_signatureName)
            });
            jar.AddRange(_argumentTypes.Select(x => new raw(ExternType.FindAndConstruct(x))));
        }
    }
}