namespace vm.dev.Internal
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using exceptions;

    public static class DevMemoryManager
    {
        public static void WriteMemory(this IDevice dev, short address, int memory)
        {
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

            var methods = dev.GetType().GetMethods()
                .Where(x => !(x.GetCustomAttribute<ActionAddressAttribute>() is null))
                .Select(x => new {meth = x, addr = x.GetCustomAttribute<ActionAddressAttribute>()})
                .ToArray();


            foreach (var method in methods)
            {
                if (method.addr.Address != address) continue;

                if (!method.meth.GetParameters().Any())
                {
                    method.meth.Invoke(dev, null);
                    return;
                }
                if(method.meth.GetParameters().Length > 1)
                    throw new CorruptedMemoryException(
                        $"Method '{method.meth.Name}' is not valid signature at calling '0x{method.addr.Address:X}' in '{dev.Name}' device. [too many arg]");
                var typeDesc = TypeDescriptor.GetConverter(method.meth.GetParameters().First().ParameterType);
                if (!typeDesc.CanConvertFrom(typeof(int)))
                    throw new CorruptedMemoryException(
                        $"Method '{method.meth.Name}' is not valid signature at calling '0x{method.addr.Address:X}' in '{dev.Name}' device. [invalid first arg]");
                method.meth.Invoke(dev, new[] {typeDesc.ConvertFrom(memory)});
                return;
            }
            throw new CorruptedMemoryException($"Invalid memory address signature '0x{address:X}' in '{dev.Name}' device.");
        }
    }
}