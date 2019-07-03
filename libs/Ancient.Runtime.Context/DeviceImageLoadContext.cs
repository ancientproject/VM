namespace ancient.runtime.context
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;

    public class DeviceImageLoadContext : AssemblyLoadContext
    {
        public DeviceImageLoadContext() : base("dev-loader", true) { }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var devName = $"{assemblyName.Name}.image";
            var file = new[]
            {
                new FileInfo($"./dev/{devName}"),
                new FileInfo($"./../dev/{devName}"),
                new FileInfo($"./../../dev/{devName}"),
                new FileInfo($"{Environment.GetEnvironmentVariable("VM_DEV_HOME")}/{devName}")
            }.FirstOrDefault(x => x.Exists);

            if (file is null)
                throw new Exception($"can't load '{assemblyName.Name}|0.0.0-managed.image' - [not found].");
            return Assembly.LoadFile(file.FullName);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) 
            => throw new Exception($"can't load '{unmanagedDllName}|0.0.0-unmanaged.image' - loader is not supported.");
    }
}
