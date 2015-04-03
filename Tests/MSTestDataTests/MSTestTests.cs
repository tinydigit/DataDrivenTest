// Copyright TinyDigit 2015
// -- Pinky --/

namespace TinyDigit.DataTest.MSTest.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using TinyDigit.DataTest;

    [TestClass]
    public class MSTestTests
    {
        [TestMethod]
        public void TestcaseAttributeNameTest()
        {
            // Test getting the right test cases
            TestcasesBase test = new TestAttributeNamedTestcase();
            IEnumerable<TestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("TestcaseAttributeName", testcases);

            // test the execution
            test.ExecuteTests();
        }

        private class TestAttributeNamedTestcase : TestcasesBase
        {
            [TestData("TestcaseAttributeName")]
            private Testcase TestCase = new Testcase { Input = "1", ExpectedValue = 1 };
        }

        [TestMethod]
        public void TestcaseAttributeExplicitNameTest()
        {
            TestcasesBase test = new TestAttributeNamedTestcase2();
            IEnumerable<TestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("TestcaseAttributeName2", testcases);

            // test the execution
            test.ExecuteTests();
        }

        private class TestAttributeNamedTestcase2 : TestcasesBase
        {
            [TestData(Name = "TestcaseAttributeName2")]
            private Testcase TestCase = new Testcase { Input = "1", ExpectedValue = 1 };
        }

        [TestMethod]
        public void NamedTestcaseNameTest()
        {
            TestcasesBase test = new ExplicitNameInTestcase();
            IEnumerable<TestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("ExplicitName", testcases);

            test = new ExplicitTestCaseNameOverride();
            testcases = test.GetTestcasesToTest();
            SingleTestAssertion("ExplicitName", testcases);

            // test the execution
            test.ExecuteTests();
        }

        private class ExplicitNameInTestcase : TestcasesBase
        {
            [TestData]
            private Testcase PropertyTest { get { return new Testcase { Input = "1", ExpectedValue = 1, Name = "ExplicitName" }; } }
        }

        private class ExplicitTestCaseNameOverride : TestcasesBase
        {
            [TestData("NameIsNotUsed")]
            private Testcase PropertyTest { get { return new Testcase { Input = "1", ExpectedValue = 1, Name = "ExplicitName" }; } }
        }

        [TestMethod]
        public void FieldArrayTest()
        {
            TestcasesBase tests = new FieldWithArrayTest();
            IEnumerable<TestcasesBase.Testcase> testcases = tests.GetTestcasesToTest();
            IEnumerableTestAssertion("Testcases_", testcases);

            // test the execution
            tests.ExecuteTests();
        }

        private class FieldWithArrayTest : TestcasesBase
        {
            [TestData]
            private Testcase[] Testcases = new Testcase[] 
            {
                new Testcase { Input = "1", ExpectedValue = 1 },
                new Testcase { Input = "2", ExpectedValue = 2 },
                new Testcase { Input = "3", ExpectedValue = 3 },
            };
        }

        [TestMethod]
        public void MethodIEnumerableTest()
        {
            TestcasesBase tests = new MethodWithIEnumerableTest();
            IEnumerable<TestcasesBase.Testcase> testcases = tests.GetTestcasesToTest();
            IEnumerableTestAssertion("GetTestcasesMethod_", testcases);

            // test the execution
            tests.ExecuteTests();
        }
        private class MethodWithIEnumerableTest : TestcasesBase
        {
            [TestData]
            private IEnumerable<Testcase> GetTestcasesMethod()
            {
                yield return new Testcase { Input = "1", ExpectedValue = 1 };
                yield return new Testcase { Input = "2", ExpectedValue = 2 };
                yield return new Testcase { Input = "3", ExpectedValue = 3 };
            }
        }

        [TestMethod]
        public void MethodArrayTest()
        {
            TestcasesBase tests = new MethodWithArrayTest();
            IEnumerable<TestcasesBase.Testcase> testcases = tests.GetTestcasesToTest();
            IEnumerableTestAssertion("GetTestcasesMethod_", testcases);

            // test the execution
            tests.ExecuteTests();
        }

        private class MethodWithArrayTest : TestcasesBase
        {
            [TestData]
            private Testcase[] GetTestcasesMethod()
            {
                return new Testcase[] 
                {
                    new Testcase { Input = "1", ExpectedValue = 1 },
                    new Testcase { Input = "2", ExpectedValue = 2 },
                    new Testcase { Input = "3", ExpectedValue = 3 }
                };
            }
        }

        [TestMethod]
        public void PropertyTest()
        {
            TestcasesBase test = new SinglePropertyTest();
            IEnumerable<TestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("PropertyTest", testcases);

            // test the execution
            test.ExecuteTests();
        }

        private class SinglePropertyTest : TestcasesBase
        {
            [TestData]
            private Testcase PropertyTest { get { return new Testcase { Input = "1", ExpectedValue = 1 }; } }
        }

        [TestMethod]
        public void StaticMethodTest()
        {
            TestcasesBase test = new StaticMethodTestClass();
            IEnumerable<TestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("GetTestcase", testcases);

            // test the execution
            test.ExecuteTests();
        }

        private class StaticMethodTestClass : TestcasesBase
        {
            [TestData]
            public static Testcase GetTestcase()
            {
                return new Testcase { Input = "1", ExpectedValue = 1 };
            }
        }

        [TestMethod]
        public void BadTypeTest()
        {
            TestcasesBase test = new BadTestcaseTypeTest();
            try
            {
                IEnumerable<TestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
                foreach (TestcasesBase.Testcase testcase in testcases)
                {
                    Assert.Fail("should have thrown an exception");
                }
                Assert.Fail("should never have gotten here");
            }
            catch(InvalidOperationException)
            {

            }
        }

        private class BadTestcaseTypeTest : TestcasesBase
        {
            [TestData]
#pragma warning disable 0414
            private int Badtestcase = 1;
#pragma warning restore 0414
        }

        [TestMethod]
        public void SingleShortFormTest()
        {
            TestcasesBase test = new SingleShortFormTestClass();
            IEnumerable<TestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("singleTest", testcases);
            test.ExecuteTests();
        }

        private class SingleShortFormTestClass : TestcasesBase
        {
            [TestData]
            Tuple<string, int> singleTest = new Tuple<string, int>("1", 1 );
        }

        [TestMethod]
        public void MultipleShortFormTest()
        {
            TestcasesBase test = new MultipeShortFormTestClass();
            IEnumerable<TestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            IEnumerableTestAssertion("multipleTests_", testcases);
            test.ExecuteTests();
        }

        private class MultipeShortFormTestClass : TestcasesBase
        {
            [TestData]
            Tuple<string, int>[] multipleTests = new Tuple<string, int>[]
            {
                new Tuple<string, int>("1", 1),
                new Tuple<string, int>("2", 2),
                new Tuple<string, int>("3", 3),
            };
        }

        private static void SingleTestAssertion(string expectedName, IEnumerable<TestcasesBase.Testcase> testcases)
        {
            int testcaseCount = 0;
            foreach (TestcasesBase.Testcase testcase in testcases)
            {
                Assert.AreEqual(expectedName, testcase.Name);
                Assert.AreEqual("1", testcase.Input);
                Assert.AreEqual(1, testcase.ExpectedValue);
                testcaseCount++;
            }
            Assert.AreEqual(1, testcaseCount, "expected a single testcase");
        }

        private void IEnumerableTestAssertion(string nameBase, IEnumerable<TestcasesBase.Testcase> testcases)
        {
            int index = 0;
            foreach (TestcasesBase.Testcase testcase in testcases)
            {
                string indexString = (index + 1).ToString();
                Assert.AreEqual(nameBase + indexString, testcase.Name);
                Assert.AreEqual(indexString, testcase.Input);
                Assert.AreEqual(index + 1, testcase.ExpectedValue);
                index++;
            }
        }

        internal class TestcasesBase : MSTestDataTest<string, int>
        {
            public TestcasesBase() : base()
            { }

            protected override int ExecuteOperation(string input)
            {
                return int.Parse(input);
            }

            // make public so that the test methods above can get the testcases to test
            public IEnumerable<Testcase> GetTestcasesToTest()
            {
                return this.GetTestcases();
            }
        }
    }
}
