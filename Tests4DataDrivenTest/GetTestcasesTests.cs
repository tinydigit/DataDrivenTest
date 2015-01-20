
namespace TinyDigit.Test.Tests4DataDrivenTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using TinyDigit.Test;

    [TestClass]
    public class GetTestcasesTests
    {
        [TestMethod]
        public void TestcaseAttributeNameTest()
        {
            GetTestcasesBase test = new TestAttributeNamedTestcase();
            IEnumerable<GetTestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("TestcaseAttributeName", testcases);
        }

        [TestMethod]
        public void TestcaseAttributeExplicitNameTest()
        {
            GetTestcasesBase test = new TestAttributeNamedTestcase2();
            IEnumerable<GetTestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("TestcaseAttributeName2", testcases);
        }

        [TestMethod]
        public void NamedTestcaseNameTest()
        {
            GetTestcasesBase test = new ExplicitNameInTestcase();
            IEnumerable<GetTestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("ExplicitName", testcases);

            test = new ExplicitTestCaseNamedOverride();
            testcases = test.GetTestcasesToTest();
            SingleTestAssertion("ExplicitName", testcases);
        }

        private static void SingleTestAssertion(string expectedName, IEnumerable<GetTestcasesBase.Testcase> testcases)
        {
            int testcaseCount = 1;
            foreach (GetTestcasesBase.Testcase testcase in testcases)
            {
                Assert.AreEqual(expectedName, testcase.Name);
                Assert.AreEqual("1", testcase.Input);
                Assert.AreEqual(1, testcase.ExpectedValue);
                testcaseCount--;
            }
            Assert.AreEqual(0, testcaseCount, "more testcases found than expected");
        }

        private void ArrayTestAssertion(string nameBase, IEnumerable<GetTestcasesBase.Testcase> testcases)
        {
            int index = 0;
            foreach (GetTestcasesBase.Testcase testcase in testcases)
            {
                string indexString = (index + 1).ToString();
                Assert.AreEqual(nameBase + indexString, testcase.Name);
                Assert.AreEqual(indexString, testcase.Input);
                Assert.AreEqual(index + 1, testcase.ExpectedValue);
                index++;
            }

        }

        [TestMethod]
        public void GetTestcaseFieldArrayTest()
        {
            GetTestcasesBase tests = new FieldWithArrayTest();
            IEnumerable<GetTestcasesBase.Testcase> testcases = tests.GetTestcasesToTest();
            ArrayTestAssertion("Testcases_", testcases);
        }

        [TestMethod]
        public void GetTestcaseMethodArrayTest()
        {
            GetTestcasesBase tests = new MethodWithArrayTest();
            IEnumerable<GetTestcasesBase.Testcase> testcases = tests.GetTestcasesToTest();
            ArrayTestAssertion("GetTestcases_", testcases);
        }

        [TestMethod]
        public void GetTestcasePropertyTest()
        {
            GetTestcasesBase test = new SinglePropertyTest();
            IEnumerable<GetTestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("PropertyTest", testcases);
        }

        [TestMethod]
        public void GetTestcaseStaticMethodTest()
        {
            GetTestcasesBase test = new StaticMethodTest();
            IEnumerable<GetTestcasesBase.Testcase> testcases = test.GetTestcasesToTest();
            SingleTestAssertion("GetTestcase", testcases);
        }

        private class GetTestcasesBase : DataTest<string, int>
        {
            public GetTestcasesBase() :
                base(new IntComparer())
            {}

            public override int ExecuteOperation(string input)
            {
                return int.Parse(input);
            }

            public IEnumerable<Testcase> GetTestcasesToTest()
            {
                return this.GetTestcases();
            }
        }

        private class TestAttributeNamedTestcase : GetTestcasesBase
        {
            [Testcase("TestcaseAttributeName")]
            private Testcase TestCase = new Testcase { Input = "1", ExpectedValue = 1 };
        }

        private class TestAttributeNamedTestcase2: GetTestcasesBase
        {
            [Testcase(Name = "TestcaseAttributeName2")]
            private Testcase TestCase = new Testcase { Input = "1", ExpectedValue = 1 }; 
        }

        private class ExplicitNameInTestcase : GetTestcasesBase
        {
            [Testcase]
            private Testcase PropertyTest { get { return new Testcase { Input = "1", ExpectedValue = 1, Name = "ExplicitName" }; } }
        }

        private class ExplicitTestCaseNamedOverride : GetTestcasesBase
        {
            [Testcase("NameIsNotUsed")]
            private Testcase PropertyTest { get { return new Testcase { Input = "1", ExpectedValue = 1, Name = "ExplicitName" }; } }
        }

        private class SinglePropertyTest : GetTestcasesBase
        {
            [Testcase]
            private Testcase PropertyTest { get { return new Testcase { Input = "1", ExpectedValue = 1 }; } }
        }

        private class FieldWithArrayTest : GetTestcasesBase
        {
            [Testcase]
            private Testcase[] Testcases = new Testcase[] 
            {
                new Testcase { Input = "1", ExpectedValue = 1 },
                new Testcase { Input = "2", ExpectedValue = 2 },
                new Testcase { Input = "3", ExpectedValue = 3 },
            };
        }

        private class StaticMethodTest : GetTestcasesBase
        {
            [Testcase]
            public static Testcase GetTestcase()
            {
                return new Testcase { Input = "1", ExpectedValue = 1 };
            }
        }

        private class MethodWithArrayTest : GetTestcasesBase
        {
            [Testcase]
            private IEnumerable<Testcase> GetTestcases()
            {
                yield return new Testcase { Input = "1", ExpectedValue = 1 };
                yield return new Testcase { Input = "2", ExpectedValue = 2 };
                yield return new Testcase { Input = "3", ExpectedValue = 3 };
            }
        }
        
        private class IntComparer : IComparer<int>
        {
            public IntComparer() { }

            public int Compare(int actual, int expected)
            {
                int result = 0;
                if (actual < expected) { result = -1; }
                else if (actual > expected) { result = 1; }
                return result;
            }

        }
    }
}
