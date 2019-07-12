namespace ancient.runtime
{
    using JetBrains.Annotations;
    using emit.@unsafe;
    
    /// <summary>
    /// Duplicate value from first cell to second cell.
    /// </summary>
    [PublicAPI]
    public class dup : Instruction
    {
        public dup(byte cell1, byte cell2) 
            : base(IID.dup) => _ = (v1 = cell1, v2 = cell2);

        /// <summary>
        /// first cell ref
        /// </summary>
        public byte v1 { get; set; }
        /// <summary>
        /// second cell ref
        /// </summary>
        public byte v2 { get; set; }

        protected override void OnCompile()
        {
            var (r1, r2) = new d8u(v1);
            var (u1, u2) = new d8u(v2);
            base.Construct(r1, r2, 0x0, u1, u2);
        }
    }
}