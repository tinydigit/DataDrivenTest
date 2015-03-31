// Copyright TinyDigit 2015
// -- Pinky --/

namespace TinyDigit.DataTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    /// <summary>
    /// Shim for MSTest data test
    /// </summary>
    [TestClass]
    public abstract class MSTestDataTest<I, E> : DataTestClass<I, E>
    {
        public MSTestDataTest() : base(Assert.AreEqual)
        { }

        [TestMethod]
        public override void ExecuteTests()
        {
            base.ExecuteTests();
        }
    }
}
