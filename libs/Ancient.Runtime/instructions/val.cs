namespace ancient.runtime
{
    using System;

    public class val : Instruction
    {
        internal object _data;
        public val(float value) : base(IID.val) => _data = value;

        public val(double value) : base(IID.val) => _data = value;

        protected override void OnCompile() { }

        public override ulong Assembly()
        {
            if (_data is float fval)
                return (ulong)BitConverter.ToInt32(BitConverter.GetBytes(fval), 0);
            if (_data is double dval)
                return (ulong)BitConverter.ToInt64(BitConverter.GetBytes(dval), 0);
            throw new InvalidCastException();
        }
    }
}