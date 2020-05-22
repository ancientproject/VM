namespace Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ancient.runtime;
    using ancient.runtime.@unsafe;
    using Xunit;

    public unsafe class NativeStringTest
    {
        [Fact]
        public void AllocateTest()
        {
            var str = "foo-bar";
            var p = NativeString.Wrap(str);
            Assert.Equal(str.Length, p.GetLen());
            Assert.Equal(Encoding.UTF8.GetByteCount(str), p.GetBuffer().Length);
            Assert.Equal(Encoding.UTF8, p.GetEncoding());
            Assert.Equal(NativeString.GetHashCode(str), p.GetHashCode());
        }
        public class StringGenerator : IEnumerable<string[]>, IEnumerable<object[]>
        {
            public string[][] Data() =>
                Enumerable.Range(0, 1)
                    .Select(i => Enumerable.Range(0, (int) Math.Pow(10, i))
                        .Select(v => Guid.NewGuid().ToString())
                        .ToArray()).ToArray();


            public IEnumerator<string[]> GetEnumerator() 
                => Data().ToList().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() 
                => GetEnumerator();
            IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() 
                => Data().Select(x => new object[]{x}).GetEnumerator();
        }

        //[Theory(Timeout = 1000 * 120)]
        //[ClassData(typeof(StringGenerator))]
        public void LoadTest(string[] data)
        {
            var hashMap = new HashSet<int>();

            foreach (var s in data) 
                Assert.True(hashMap.Add(NativeString.GetHashCode(s)));

        }
    }
}