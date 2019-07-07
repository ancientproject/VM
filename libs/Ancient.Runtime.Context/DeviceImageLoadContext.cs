namespace ancient.runtime.context
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;

    public class DeviceImageLoadContext : AssemblyLoadContext
    {
        private readonly Action<string> _trace;
        public DeviceImageLoadContext(Action<string> trace = null) : base("dev-loader", true) => _trace = trace ?? (x => {});

        public DirectoryInfo CacheDir
        {
            get
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ".ancient/cache/dev");
                return Directory.CreateDirectory(dir);
            }
        }


        protected override Assembly Load(AssemblyName assemblyName)
        {
            var devName = $"{assemblyName.Name}.image";
            var file = new[] 
            {
                new FileInfo($"./dev/{devName}"),
                new FileInfo($"./../dev/{devName}"),
                new FileInfo($"./../../dev/{devName}"),
                new FileInfo($"{Environment.GetEnvironmentVariable("VM_DEV_HOME")}/{devName}"),
                new FileInfo(Path.Combine(CacheDir.FullName, $"{devName}"))
            }.FirstOrDefault(x => x.Exists);
            
            if (file is null)
                throw new Exception($"can't load '{assemblyName.Name}|0.0.0-managed.image' - [not found].");
            (_trace + (q => Trace.WriteLine(q)))($"'{devName}' can find in '{file}' and load...");
            return Assembly.LoadFile(file.FullName);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) 
            => throw new Exception($"can't load '{unmanagedDllName}|0.0.0-unmanaged.image' - loader is not supported.");
    }
}
