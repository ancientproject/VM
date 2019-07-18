namespace ancient.runtime
{
    using System.Linq;
    using System.Text;

    public class halt : Instruction
    {
        public halt() : base(IID.halt) { }

        protected override void OnCompile() 
            => Construct(0xE, 0xA, 0xD);

        public override bool HasMetadata() => true;
        protected internal override byte[] metadataBytes => Encoding.ASCII.GetBytes("@shutdown@").ToArray();
    }
}