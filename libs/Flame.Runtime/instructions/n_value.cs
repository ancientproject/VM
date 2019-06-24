namespace flame.runtime
{
    using System;

    public class n_value : Instruction
    {
        internal object _data;
        public n_value(float value) : base(InsID.n_value) => _data = value;

        public n_value(double value) : base(InsID.n_value) => _data = value;

        protected override void OnCompile() { }

        public override long Assembly()
        {
            if (_data is float fval)
                return BitConverter.ToInt32(BitConverter.GetBytes(fval), 0);
            if (_data is double dval)
                return BitConverter.ToInt64(BitConverter.GetBytes(dval), 0);
            throw new InvalidCastException();
        }
    }
}