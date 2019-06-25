namespace ancient.runtime.compiler.test
{
    using emit;
    using Xunit;

    public class AssemblyTagTest
    {
        [Fact]
        public void Create()
        {
            Assert.True(AssemblyTag.IsTag("EF0109JG00"));
            Assert.Equal("EF0119JG00", new AssemblyTag(AssemblyTag.SignType.Signed, AssemblyTag.ArchType.Any, 1).ToString());
            Assert.Equal("EF0500JG00", new AssemblyTag(AssemblyTag.SignType.UnSecurity, AssemblyTag.ArchType.x64, 5).ToString());
        }
    }
}