namespace ancient.runtime.compiler.test
{
    using emit.sys;
    using Xunit;

    public class LocalsInitTest
    {
        [Fact]
        public void LocalsSegmentTest()
        {
            var (o1, o2) = new EvaluationSegment(0xAB, "f128").OpCode;
            var eva = EvaluationSegment.Construct((ulong) o1.Assembly(), (ulong) o2.Assembly());
            Assert.Equal("f128", eva.Type);
            Assert.Equal(0xAB, eva.Index);
        }
    }
}