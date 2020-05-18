namespace ancient.runtime.compiler.test
{
    using emit;
    using Xunit;

    public class AssemblyTagTest
    {
        [Fact]
        public void IsTag() 
            => Assert.True(AssemblyTag.IsTag("EF0109JG00"));

        [Fact]
        public void EF0119K() => 
            Assert.Contains("EF0119K", new AssemblyTag(AssemblyTag.SignType.Signed, AssemblyTag.ArchType.Any, 1).ToString());

        [Fact]
        public void EF0500K() 
            => Assert.Contains("EF0500K", new AssemblyTag(AssemblyTag.SignType.UnSecurity, AssemblyTag.ArchType.x64, 5).ToString());
    }
}