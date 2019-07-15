namespace ancient.runtime
{
    using emit;

    public abstract class OpCode : IILGenerable
    {
        public abstract byte[] GetBodyILBytes();
        public abstract byte[] GetMetaDataILBytes();
        public virtual bool HasMetadata() => false;
    }
}