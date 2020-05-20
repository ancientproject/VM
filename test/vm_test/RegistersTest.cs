namespace vm_test
{
    using ancient.runtime.@base;
    using NUnit.Framework;

    [TestFixture]
    public class RegistersTest
    {
        [Test]
        public void ControlRegisterTest()
        {
            var state = new FixtureState
            {
                pc = 0x77
            };

            var cr = new ControlRegister(state);

            Assert.AreEqual(0x77, cr.Recoil());
            cr.Branch(0x95);
            Assert.AreEqual(0x95, cr.Recoil());
            Assert.AreEqual(0x77, cr.Recoil());
            cr.Branch(0x95);
            cr.Branch(0x145);
            cr.Branch(0x175);
            Assert.AreEqual(0x77, cr.Recoil(3));
            cr.Branch(0x95);
            cr.Branch(0x145);
            cr.Branch(0x175);
            Assert.AreEqual(0x95, cr.Recoil(2));

        }




        
    }
}