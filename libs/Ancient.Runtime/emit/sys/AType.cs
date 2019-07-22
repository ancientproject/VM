namespace ancient.runtime.emit.sys
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using @unsafe;

    public abstract class ExternType
    {
        public static ExternType Find(params d8u[] bytes) 
            => Find(Encoding.ASCII.GetString(bytes.Select(u8 => u8.Value).ToArray()));

        public static ExternType Find(string code)
        {
            var type = Type.GetType($"ancient.runtime.emit.sys.{code}_Type");
            if(type is null)
                return new Unknown_Type();
            return Activator.CreateInstance(type) as ExternType;
        }
    }
    public sealed class Unknown_Type : ExternType { }
    public sealed class Str_Type : ExternType { }
    public sealed class f64_Type : ExternType { }
    public sealed class u64_Type : ExternType { }
    public sealed class u32_Type : ExternType { }
    public sealed class u16_Type : ExternType { }
    public sealed class u8_Type  : ExternType { }
    public sealed class u2_Type  : ExternType { }

    public sealed class i64_Type : ExternType { }
    public sealed class i32_Type : ExternType { }
    public sealed class i16_Type : ExternType { }
    public sealed class i8_Type  : ExternType { }
    public sealed class i2_Type  : ExternType { }

    public sealed class Module
    {
        public static ExternType[] DefinedTypes { get; } =
        {
            new u8_Type(), new u16_Type(), new u32_Type(), new u64_Type(), new f64_Type(), new u2_Type(),
            new i8_Type(), new i16_Type(), new i32_Type(), new i64_Type(), new i2_Type()
        };
        public static Context Global { get; } = new Context();

        public static ExternSignature Composite(string sign, ushort index)
        {
            var s = new ExternSignature
            {
                Signature = sign, MethodIndex = index, Arguments = new List<ExternType>()
            };
            foreach (Match match in new Regex(@"(?<type>i32|i64|i16|i8|i2|u32|u64|u16|u8|u2|f64)+?\,?\s?").Matches(sign))
                s.Arguments.Add(DefinedTypes.First(x => x.GetType().Name == $"{match.Groups["type"]}_Type"));
            return s;
        }

        public static ushort CompositeIndex(string sign) 
            => ushort.Parse($"{sign.GetHashCode():X}".Remove(0, 4), NumberStyles.AllowHexSpecifier);


        public static void Boot()
        {
            Global.Add("sys->readChar()", typeof(Console).GetMethod("Read"));
        }

        public class Context
        {
            public IDictionary<ushort, ExternSignature> methods = new Dictionary<ushort, ExternSignature>();

            public void Add(string sign, MethodInfo method)
            {
                var index = CompositeIndex(sign);
                if(methods.ContainsKey(index))
                    return;
                var @extern = Composite(sign, index);
                @extern.method = method;
                methods.Add(index, @extern);
            }

            public ExternStatus Find(ushort sign, out ExternSignature signature)
            {
                signature = null;
                if (!methods.ContainsKey(sign))
                    return ExternStatus.MethodNotFound;
                signature = methods[sign];
                if (signature?.method is null)
                    return ExternStatus.SigFault;
                if (!signature.method.IsStatic)
                    return ExternStatus.MethodNotStatic;
                if (!signature.method.IsSecurityCritical)
                    return ExternStatus.SecurityFault;


                return ExternStatus.Found;
            }
        }
    }

    public enum ExternStatus
    {
        MethodNotFound = 2,
        SigFault = 3,
        MethodNotStatic = 4,
        SecurityFault = 5,
        Found = 10
    }
}