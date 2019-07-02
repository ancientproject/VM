namespace vm.dev.Internal
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using ancient.runtime.exceptions;

    public static class MemoryManagement
    {
        internal static Dictionary<string, (IDevice dev, MethodAddress address)> storage = new Dictionary<string, (IDevice dev, MethodAddress address)>();
        
        public static bool FastWrite = false;

        internal struct MethodAddress
        {
            public MethodInfo Method { get; set; }
            public short Address { get; set; }
            public bool IsArgs { get; set; }
            public TypeConverter ArgConverter { get; set; }
        }

        internal static void WriteMemory(this IDevice dev, long address, long memory)
        {
            var key = $"{dev.Name}-{dev.StartAddress:X}-{address:X}";
            if (FastWrite)
            {
                if (storage.ContainsKey(key))
                {
                    var addr = storage[key].address;
                    addr.Method.Invoke(dev, addr.IsArgs ? new[] {addr.ArgConverter.ConvertFrom(memory)} : null);
                    return;
                }
            }

            var methods = dev.GetType().GetMethods()
                .Where(x => !(x.GetCustomAttribute<ActionAddressAttribute>() is null))
                .Select(x => new MethodAddress {Method = x, Address = x.GetCustomAttribute<ActionAddressAttribute>().Address})
                .ToArray();

            foreach (var method in methods)
            {
                if (method.Address != address) continue;

                if (!method.Method.GetParameters().Any())
                {
                    method.Method.Invoke(dev, null);
                    
                    if (FastWrite && !storage.ContainsKey(key))
                        storage.Add(key, (dev, method));
                    return;
                }
                if(method.Method.GetParameters().Length > 1)
                    throw new CorruptedMemoryException(
                        $"Method '{method.Method.Name}' is not valid signature at calling '0x{method.Address:X}' in '{dev.Name}' device. [too many arg]");
                var typeDesc = TypeDescriptor.GetConverter(method.Method.GetParameters().First().ParameterType);
                if (!typeDesc.CanConvertFrom(typeof(int)))
                    throw new CorruptedMemoryException(
                        $"Method '{method.Method.Name}' is not valid signature at calling '0x{method.Address:X}' in '{dev.Name}' device. [invalid first arg]");
                method.Method.Invoke(dev, new[] {typeDesc.ConvertFrom(memory)});
                var addr = method;
                addr.IsArgs = true;
                addr.ArgConverter = typeDesc;
                if (FastWrite && !storage.ContainsKey(key))
                    storage.Add(key, (dev, addr));
                return;
            }

            var props = dev.GetType().GetProperties()
                .Where(x => x.CanWrite)
                .Where(x => !(x.GetCustomAttribute<PropAddressAttribute>() is null))
                .Select(x => new {prop = x, addr = x.GetCustomAttribute<PropAddressAttribute>()}).ToArray();

            foreach (var prop in props)  
            {
                if (prop.addr.Address != address)
                    continue;
                prop.prop.SetValue(dev, TypeDescriptor.GetConverter(prop.prop.PropertyType).ConvertFrom(memory));
                return;
            }
            throw new CorruptedMemoryException($"Invalid memory address signature '0x{address:X}' in '{dev.Name}' device.");
        }
    }
}