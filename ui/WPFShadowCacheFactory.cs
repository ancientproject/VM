namespace CPU_Host
{
    using vm.component;

    public class WPFShadowCacheFactory : ShadowCache<Cache>
    {
        public Cache L1 { get; set; } = new WPFCache();

        public Cache L2 { get; set; } = new Cache();

        public void Reflect()
        {
            L2 = (Cache)L1.Clone();
        }
    }
}