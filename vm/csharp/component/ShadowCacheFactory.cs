namespace vm.component
{
    public class ShadowCacheFactory : ShadowCache<Cache>
    {
        public Cache L1 { get; set; } = new Cache();
        public Cache L2 { get; private set; } = new Cache();

        public void Reflect() => L2 = L1.Clone() as Cache;

        public static ShadowCache<Cache> Create() => new ShadowCacheFactory();
    }
}