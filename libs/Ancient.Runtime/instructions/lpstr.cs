namespace ancient.runtime
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class lpstr : Instruction
    {
        private readonly string _str;

        public lpstr(string str) : base(IID.lpstr) => _str = str;

        protected override void OnCompile()
        {
        }

        public override bool HasMetadata() => true;

        protected internal override byte[] metadataBytes
        {
            get
            {
                var len = Encoding.UTF8.GetByteCount(_str);
                var bu = new List<byte>();
                bu.AddRange(BitConverter.GetBytes(len));
                bu.AddRange(Encoding.UTF8.GetBytes(_str));
                return bu.ToArray();
            }

        }
    }
}