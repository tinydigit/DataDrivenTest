// Copyright TinyDigit 2015
// -- Pinky --/

namespace TinyDigit.DataTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;


    /// <summary>
    /// Shim for MSTest data test
    /// </summary>
    [TestClass]
    public abstract class MSTestDataTest<I, E> : DataTestClass<I, E>
    {
        
        /// <summary>
        /// Uses the MultipleAssertionHandler to collect all assertion failures across all testcases in a class
        /// Wraps the MSTest assertion class implemented below, which calls MSTest.Assert class
        /// </summary>
        public MSTestDataTest() : base(
            new MultipleAssertionHandler<E>(
                new MSTestAssertionHandler()))
        {
            this.DataTestClassCompleted += MSTestDataTest_TestingCompleted;
        }

        void MSTestDataTest_TestingCompleted(object sender, EventArgs e)
        {
            this.assertionHandler.TestCompleteAssert(null);
        }

        [TestMethod]
        public override void ExecuteTests()
        {
            base.ExecuteTests();
        }

        /// <summary>
        /// Wrapper around the Assert.* methods in MSTest
        /// </summary>
        private class MSTestAssertionHandler : AssertionHandler<E>
        {
            public override void TestcaseAssert(E expected, E actual, string errorMessage)
            {
                Assert.AreEqual(expected, actual, errorMessage);
            }

            public override void TestCompleteAssert(string errorMessage)
            {
                Assert.Fail(errorMessage);
            }
        }
    }
}
