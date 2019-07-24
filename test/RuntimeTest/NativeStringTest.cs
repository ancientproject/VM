namespace Tests
{
    using System.Text;
    using ancient.runtime.@unsafe;
    using NUnit.Framework;

    public unsafe class NativeStringTest
    {
        [Test]
        public void AllocateTest()
        {
            var str = "foo-bar";
            var p = NativeString.Wrap(str);
            Assert.AreEqual(str.Length, p.GetLen());
            Assert.AreEqual(Encoding.UTF8.GetByteCount(str), p.GetBuffer().Length);
            Assert.AreEqual(Encoding.UTF8, p.GetEncoding());
            Assert.AreEqual(NativeString.GetHashCode(str), p.GetHashCode());
        }
    }
}