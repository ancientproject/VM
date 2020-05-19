namespace ancient.runtime.compiler.test
{
    using System.Linq;
    using ancient.compiler.tokens;
    using Sprache;
    using Xunit;

    public class AdditionalTokenParseTest
    {
        [Theory]
        [InlineData(".sig @test(u2, u32) -> void")]
        public void CallStaticInternalFunctions(string code)
        {
            var result = new AssemblerSyntax().Signature.End().Parse(code);

            Assert.IsType<SignatureEvolve>(result);
            if (result is SignatureEvolve ev)
            {
                Assert.Equal(2, ev._argumentTypes.Count);
                Assert.Equal("u2", ev._argumentTypes.First());
                Assert.Equal("u32", ev._argumentTypes.Last());
                Assert.Equal("test", ev._signatureName);

                Assert.IsAssignableFrom<IEvolveEvent>(result);

                (ev as IEvolveEvent).OnBuild();

                var instructions = ev.GetInstructions();


            }
        }
    }
}