namespace ancient.runtime.compiler.test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using emit;
    using System.Linq;
    using Xunit;

    public class DynamicAssemblyTest
    {
        [Fact]
        public void Create()
        {
            var d = new DynamicAssembly("test", ("foo", "bar"));
            Assert.Equal("test", d.Name);
            Assert.Equal(("foo", "bar"), d.Metadata.First());
            Assert.Throws<InvalidOperationException>(() => { d.GetBytes(); });
            Assert.NotNull(d.GetGenerator());
            Assert.IsType<ILGen>(d.GetGenerator());
            d.GetGenerator().Emit(new ldi(0xF, 0xC));
            Assert.Equal(sizeof(ulong), d.GetILCode().Length);
            Assert.Equal($"{0x1F00C000:X}", $"{BitConverter.ToUInt32(d.GetILCode()):X}");
        }
    }

    public class SummonData : IEnumerable, IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[] {(IID.abs, new[] {(byte)0x1})},
            new object[] {(IID.flr, new[] {(byte)0x1})},
            new object[] {(IID.atan, new[] {(byte)0x1})},
            new object[] {(IID.atan2, new[] {(byte)0x1, (byte)0x2})},
            new object[] {(IID.max, new[] {(byte)0x1, (byte)0x2})},
        };
        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}