﻿namespace flame.runtime.compiler.test
{
    using System;
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
            d.GetGenerator().Emit(new loadi(0xF, 0xC));
            Assert.Equal(4, d.GetILCode().Length);
            Assert.Equal($"{0x1F000C00:X}", $"{BitConverter.ToUInt32(d.GetILCode()):X}");
        }
    }
}