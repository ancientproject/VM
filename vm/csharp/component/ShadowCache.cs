namespace vm.component
{
    using System;

    public interface ShadowCache<T> where T : Cache, ICloneable
    {
        T L1 { get; set; }
        T L2 { get; }

        void Reflect();
    }
}