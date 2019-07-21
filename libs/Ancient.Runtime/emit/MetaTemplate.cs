namespace ancient.runtime.emit.@unsafe
{
    using System.Runtime.InteropServices;
    using System.Security;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MetaTemplate
    {
        public int len { get; set; }
        public TemplateType type { get; set; }

        public static MetaTemplate By(TemplateType type) => new MetaTemplate { type = type };

        public MetaTemplate Len(int l)
        {
            len = l;
            return this;
        }
        [SecurityCritical]
        public byte[] ToBytes() 
        {
            var size = Marshal.SizeOf(this);
            var arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        [SecurityCritical]
        public static MetaTemplate FromBytes(byte[] arr)
        {
            var str = new MetaTemplate();

            var size = Marshal.SizeOf(str);
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (MetaTemplate)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);
            return str;
        }
    }
}