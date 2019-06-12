namespace flame.runtime
{
    public class label : Instruction
    {
        public label(char c, bool isAuto, bool isEnd, short? cellID) : base(InsID.label)
        {
        }


        protected override void OnCompile()
        {
            throw new System.NotImplementedException();
        }
    }
}