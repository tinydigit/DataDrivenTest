// Copyright TinyDigit
// -- Pinky --/

namespace TinyDigit.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    // Base abstract class in the even that a consumer
    // wants to override how a Test is actually executed
    // without dealing with generic types
    public abstract class DataTest
    {
        [TestMethod]
        public abstract void ExecuteTest();
    }

    // DataTest abstraction that uses generic types to handle 
    // the input (I) and expected value (E) types of a data test
    // The generic types constrain the type of operation performed on the input to produce the output (ExecuteOperation).
    // The generic types also constrain the type of the assert function (ExecuteAssertion).
    // Finally the generic types constrain the actual testcase data that gets 
    // populated in the derived implementation of DataTest. 
    // The constructor gives options for how to compare the expected value in the Testcase
    // against the actual value produced by ExecuteOperation.
    public abstract class DataTest<I, E> : DataTest
    {
        private static object[] PARAMETERS = new object[0];
        private readonly Func<E, E, bool> comparer;

        public DataTest()
        {
            this.comparer = null;
        }

        public DataTest(IComparer<E> comparer)
        {
            this.comparer = (E actual, E expected) => comparer.Compare(actual, expected) == 0;
        }

        public DataTest(Func<E, E, bool> comparer)
        {
            this.comparer = comparer;
        }

        public abstract E ExecuteOperation(I input);

        // performs an assert using the comparer passed in during construction
        public virtual void ExecuteAssertion(E actualValue, E expectedValue)
        {
            if (this.comparer != null)
            {
                Assert.IsTrue(this.comparer(actualValue, expectedValue));
            }
            else
            {
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        // Testcase handles input value of type I
        // and expected output value of type E
        // This is used to constrain the data tests that are processed
        // by the derived type of DataTest
        public class Testcase
        {
            // The name of a test case if specified here, is authoritative
            // Otherwise, if the name is specified in the TestcaseAttribute, then that is used
            // Otherwise we generate a name based on the source.
            public virtual String Name { get; set; }
            public virtual I Input { get; set; }
            public virtual E ExpectedValue { get; set; }
        }

        /* Basic algorithm description
         * 1. Get the list of testcases using TestcaseAttribute
         * 2. Make sure all of the Testcases are named, otherwise name them
         * 3. Construct a results object
         * 4. For each Testcase
         *      1. Within a try/catch
         *      2. Perform the operation
         *      3. Perform the assertion
         *      4. Capture the results in the result object, including exception
         * 5. Report the results to MSTest
         */
        public override void ExecuteTest()
        {

        }

        // This is protected only to allow unit testing of the Gettestcase functionality that uses attributes
        // to capture testcase data
        // Basic algorithm
        // 1. Get all of the members with TestcaseAttribute
        // 2. Foreach member, get the value
        //      2.1 If the value is a single Testcase, then fix the name and return it
        //      2.2 else the value must be an IEnumerable<Testcase>, so we fix each name and return it
        protected IEnumerable<Testcase> GetTestcases()
        {
            IEnumerable<Testcase> results;
            IEnumerable<MemberInfo> members;
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            Type thisType = this.GetType();

            // Properties
            members = thisType.GetFields(flags);
            members = Enumerable.Concat(members, thisType.GetProperties(flags));
            members = Enumerable.Concat(members, thisType.GetMethods(flags));
            results = ProcessMembers(members);
            return results;

        }

        private IEnumerable<Testcase> ProcessMembers(IEnumerable<MemberInfo> members)
        {
            foreach(MemberInfo member in members)
            {
                TestcaseAttribute attribute = (TestcaseAttribute) Attribute.GetCustomAttribute(
                    member, 
                    typeof(TestcaseAttribute), 
                    true);

                if (attribute != null)
                {
                    object value = GetMemberValue(member);
                    foreach(Testcase testcase in GetTestcasesFromReflection(value, member, attribute))
                    {
                        yield return testcase;
                    }
                }
            }
        }

        private object GetMemberValue(MemberInfo member)
        {
            FieldInfo field = member as FieldInfo;
            PropertyInfo property = member as PropertyInfo;
            MethodInfo method = member as MethodInfo;

            if (field != null)
            {
                return GetMemberValue(field);
            }
            else if (property != null)
            {
                return GetMemberValue(property);
            }
            else if (method != null)
            {
                return GetMemberValue(method);
            }
            else
            {
                throw new InvalidOperationException("invalid member info");
            }
        }

        private object GetMemberValue(FieldInfo field)
        {
            return field.GetValue(this);
        }

        private object GetMemberValue(PropertyInfo property)
        {
            return property.GetValue(this);
        }

        private object GetMemberValue(MethodInfo method)
        {

            if (method.IsStatic)
            {
                return method.Invoke(null, PARAMETERS);
            }
            else
            {
                return method.Invoke(this, PARAMETERS);
            }
        }
        
        private IEnumerable<Testcase> GetTestcasesFromReflection(object value, MemberInfo member, TestcaseAttribute attribute)
        {
            Testcase singleTestcase = value as Testcase;
            IEnumerable<Testcase> multipleTestcases = value as IEnumerable<Testcase>;
            if (singleTestcase != null)
            {
                if (string.IsNullOrEmpty(singleTestcase.Name))
                {
                    singleTestcase.Name = CreateTestcaseName(member, attribute);
                }
                yield return singleTestcase;
            }
            else if (multipleTestcases != null)
            {
                int index = 1;
                foreach (Testcase testcase in multipleTestcases)
                {
                    if (string.IsNullOrEmpty(testcase.Name))
                    {
                        testcase.Name = CreateTestcaseName(member, attribute) + "_" + index.ToString();
                        index++;
                    }
                    yield return testcase;
                }
            }
            else
            {
                throw new InvalidOperationException("invalid type returned from TestcaseAttribute");
            }
        }

        private string CreateTestcaseName(MemberInfo member, TestcaseAttribute attribute)
        {
            if (!string.IsNullOrEmpty(attribute.Name))
            {
                return attribute.Name;
            }
            return member.Name;
        }
    }
}
