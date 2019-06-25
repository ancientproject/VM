namespace ancient.runtime
{
    using emit;

    public abstract class OpCode : IILGenerable
    {
        public abstract byte[] GetBodyILBytes();
    }
}