namespace ancient.runtime.context
{
    using MoreLinq;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;

    public class DeviceImageLoadContext : AssemblyLoadContext
    {
        private readonly Action<string> _trace;
        public DeviceImageLoadContext(Action<string> trace = null) 
            : base("dev-loader", true) 
            => _trace = trace ?? (x => {});

        public DirectoryInfo CacheDir
        {
            get
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ancient");
                return Directory.CreateDirectory(dir).CreateSubdirectory("packages");
            }
        }

        private void log(string s) => _trace($"[image-load-context] {s}");

        public FileSystemInfo FindImage(string devName, Version version)
        {
            var corePath = $"deps/{version.Major}.{version.Minor}.{version.Build}/any/{devName}.image";
            return new FileSystemInfo[]
            {
                new FileInfo($"./{corePath}"),
                new FileInfo($"./../{corePath}"),
                new FileInfo($"./../../{corePath}"),
                new FileInfo($"{Environment.GetEnvironmentVariable("VM_DEV_HOME") ?? "vm:/home"}/{corePath}"),
                new VMFileInfo(Path.Combine(CacheDir.FullName, corePath).Replace("\\", "/"))
            }.Pipe(x => log($"try find '{devName}' in '{x}'")).FirstOrDefault(x => x.Exists);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var imageName = assemblyName.Name;
            var fullImageName = $"{assemblyName.Name}.image";
            var imageVersion = assemblyName.Version;
            var file = FindImage(imageName, imageVersion);
            if (file is null)
                throw new Exception($"can't load '{fullImageName}' - [not found].");
            log($"'{fullImageName}' was found in '{file}' and success loaded");
            return Assembly.LoadFile(file.FullName);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) 
            => throw new Exception($"can't load '{unmanagedDllName}|0.0.0-unmanaged.image' - loader is not supported.");
    }
}
