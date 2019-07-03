namespace ancient.runtime.context
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;

    public class DeviceLoadContext : AssemblyLoadContext
    {
        public DeviceLoadContext() : base("dev-loader", true) { }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var devName = $"{assemblyName.FullName}|-managed.dll";
            var file = new[]
            {
                new FileInfo($"./dev/{devName}"),
                new FileInfo($"./../dev/{devName}"),
                new FileInfo($"./../../dev/{devName}"),
                new FileInfo($"{Environment.GetEnvironmentVariable("VM_DEV_HOME")}/{devName}")
            }.FirstOrDefault(x => x.Exists);

            if (file is null)
                return null;
            return Assembly.LoadFile(file.FullName);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) 
            => throw new Exception($"can't load '{unmanagedDllName}|0.0.0-unmanaged.dll' - loader is not supported.");
    }
}
