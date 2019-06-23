namespace vm.component
{
    using System;

    public class Cache : ICloneable
    {
        /// <summary>
        /// base register cell
        /// </summary>
        public virtual ushort r1 { get; set; }
        public virtual ushort r2 { get; set; }
        public virtual ushort r3 { get; set; }
        /// <summary>
        /// value register cell
        /// </summary>
        public virtual ushort u1 { get; set; }
        public virtual ushort u2 { get; set; }
        /// <summary>
        /// magic cell
        /// </summary>
        public virtual ushort x1 { get; set; }
        public virtual ushort x2 { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public virtual ushort IID { get; set; }

        public virtual ulong PC { get; set; }

        public object Clone() => new Cache 
            { r1 = r1, r2 = r2, r3 = r3, u1 = u1, u2 = u2, x1 = x1, x2 = x2, IID = IID, PC = PC };
    }
}