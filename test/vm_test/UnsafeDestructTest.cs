namespace vm_test
{
    using ancient.runtime;
    using ancient.runtime.emit.@unsafe;
    using NUnit.Framework;

    public class UnsafeDestructTest
    {
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("test of destruct of u8 number")]
        public void d8uTest()
        {
            var destructor = new d8u(0xFC);
            Assert.AreEqual(8, destructor.full_size);
            Assert.AreEqual(4, destructor.size);
            Assert.AreEqual(sizeof(byte), destructor.unmanaged_size);

            Assert.AreEqual(4, destructor.shift());
            Assert.AreEqual(0, destructor.shift());
            destructor.resetShifter();
            var (r1, r2) = destructor;
            Assert.AreEqual($"0xF", $"0x{r1:X}");
            Assert.AreEqual($"0xC", $"0x{r2:X}");
        }

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("test of destruct of u16 number")]
        public void d16uTest()
        {
            var destructor = new d16u(0xABCD);
            Assert.AreEqual(12, destructor.size);
            Assert.AreEqual(16, destructor.full_size);
            Assert.AreEqual(sizeof(ushort), destructor.unmanaged_size);

            Assert.AreEqual(12, destructor.shift());
            Assert.AreEqual(8, destructor.shift());
            Assert.AreEqual(4, destructor.shift());
            Assert.AreEqual(0, destructor.shift());
            destructor.resetShifter();
            var (r1, r2, r3, r4) = destructor;
            Assert.AreEqual($"0xA", $"0x{r1:X}");
            Assert.AreEqual($"0xB", $"0x{r2:X}");
            Assert.AreEqual($"0xC", $"0x{r3:X}");
            Assert.AreEqual($"0xD", $"0x{r4:X}");
        }

        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("test of destruct of u32 number")]
        public void d32uTest()
        {
            var destructor = new d32u(0xABCDEF12);
            Assert.AreEqual(28, destructor.size);
            Assert.AreEqual(32, destructor.full_size);
            Assert.AreEqual(sizeof(uint), destructor.unmanaged_size);

            Assert.AreEqual(28, destructor.shift());
            Assert.AreEqual(24, destructor.shift());
            Assert.AreEqual(20, destructor.shift());
            Assert.AreEqual(16, destructor.shift());
            Assert.AreEqual(12, destructor.shift());
            Assert.AreEqual(8, destructor.shift());
            Assert.AreEqual(4, destructor.shift());
            Assert.AreEqual(0, destructor.shift());
            destructor.resetShifter();
            var (r1, r2, r3, r4, r5, r6, r7, r8) = destructor;
            Assert.AreEqual($"0xA", $"0x{r1:X}");
            Assert.AreEqual($"0xB", $"0x{r2:X}");
            Assert.AreEqual($"0xC", $"0x{r3:X}");
            Assert.AreEqual($"0xD", $"0x{r4:X}");
            Assert.AreEqual($"0xE", $"0x{r5:X}");
            Assert.AreEqual($"0xF", $"0x{r6:X}");
            Assert.AreEqual($"0x1", $"0x{r7:X}");
            Assert.AreEqual($"0x2", $"0x{r8:X}");

        }
    }
}