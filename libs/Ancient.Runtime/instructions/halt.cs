namespace ancient.runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using emit.@unsafe;

    public class halt : Instruction
    {
        public halt() : base(IID.halt) { }

        protected override void OnCompile() 
            => Construct(0xE, 0xA, 0xD);

        public override bool HasMetadata() => true;
        protected internal override byte[] metadataBytes
        {
            get
            {
                var str = Encoding.ASCII.GetBytes("@shutdown@");
                var bytes = new List<byte>();
                bytes.AddRange(MetaTemplate.By(TemplateType.RND).Len(str.Length).ToBytes());
                bytes.AddRange(str);
                return bytes.ToArray();
            }
        }
    }
}