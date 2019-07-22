namespace ancient.runtime.@unsafe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;

    /*
     * |                                      Method |      Mean |     Error |    StdDev | Rank |
     * |-------------------------------------------- |----------:|----------:|----------:|-----:|
     * |        'intern string [managed non-insert]' |  4.604 ns | 0.0782 ns | 0.0732 ns |    1 |
     * |         'intern string [native non-insert]' |  4.838 ns | 0.0551 ns | 0.0515 ns |    2 |
     * |  'intern string [native insert, auto free]' | 55.333 ns | 0.3792 ns | 0.3361 ns |    3 |
     * | 'intern string [managed insert, auto free]' | 64.734 ns | 0.3740 ns | 0.3316 ns |    4 |
     */
    public static unsafe class StringLiteralMap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityCritical]
        public static string GetInternedString(string str, bool AddIfNotFound)
        {
            var @ref = NativeString.Wrap(str);
            NativeString.Unwrap(GetInternedString(&@ref, AddIfNotFound), out var result, true, true);
            return result;
        }
        [SecurityCritical]
        public static NativeString* GetInternedString(NativeString* pStr, bool AddIfNotFound)
        {
            if (IsInternedString(pStr))
            {
                Marshal.AddRef((IntPtr)pStr->@ref);
                return pStr;
            }
            if (AddIfNotFound)
            {
                literalStorage.Add(*pStr);
                GC.KeepAlive(*pStr);
                return pStr;
            }
            return null;
        }
        public static bool IsInternedString(in NativeString* a) => literalStorage.Contains(*a);


        public static void Clear() => literalStorage.Clear();

        public static bool Has(int index)
        {
            return literalStorage.Any(x => x.GetHashCode() == index);
        }

        #region private

        private static readonly HashSet<NativeString> literalStorage = new HashSet<NativeString>();

        static StringLiteralMap()
        {
            GC.KeepAlive(literalStorage);
        }

        #endregion
    }
}