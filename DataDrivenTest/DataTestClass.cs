// Copyright TinyDigit 2015
// -- Pinky --/

namespace TinyDigit.DataTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    // Base abstract class in the even that a consumer
    // wants to override how a Test is actually executed
    // without dealing with generic types
    public abstract class DataTestClass
    {
        public abstract void ExecuteTests();
    }

    // DataTest abstraction that uses generic types to handle 
    // the input (I) and expected value (E) types of a data test
    // The generic types constrain the type of operation performed on the input to produce the output (ExecuteOperation).
    // The generic types also constrain the type of the assert function (ExecuteAssertion).
    // Finally the generic types constrain the actual testcase data that gets 
    // populated in the derived implementation of DataTest. 
    // The constructor gives options for how to compare the expected value in the Testcase
    // against the actual value produced by ExecuteOperation.
    public abstract class DataTestClass<I, E> : DataTestClass
    {
        private static object[] PARAMETERS = new object[0];
        private readonly AssertionHandler asserter;

        public delegate void AssertionHandler(E expected, E actual, string errorMessage);

        // Testcase handles input value of type I
        // and expected output value of type E
        // This is used to constrain the data tests that are processed
        // by the derived type of DataTest
        public class Testcase
        {
            // The name of a test case if specified here, is authoritative
            // Otherwise, if the name is specified in the TestcaseAttribute, then that is used
            // Otherwise we generate a name based on the source.
            public Testcase()
            { }

            public Testcase(I input, E expectedValue)
            {
                this.Input = input;
                this.ExpectedValue = expectedValue;
            }

            public virtual String Name { get; set; }
            public virtual I Input { get; set; }
            public virtual E ExpectedValue { get; set; }

            internal void ConfigureName(string nameInAttribute, string memberName)
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    if (!string.IsNullOrEmpty(nameInAttribute))
                    {
                        this.Name = nameInAttribute;
                    }
                    else
                    {
                        this.Name = memberName;
                    }
                }
            }

            internal void ConfigureName(string nameInAttribute, string memberName, int index)
            {
                ConfigureName(nameInAttribute, memberName);
                this.Name = string.Format("{0}_{1}", this.Name, index);
            }

            internal void ConfigureName(string typeName, int index)
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    this.Name = string.Format("{0}_{1}", typeName, index);
                }
            }
        }

        public DataTestClass(AssertionHandler asserter)
        {
            if (asserter == null)
            {
                throw new ArgumentNullException("asserter");
            }
            this.asserter = asserter;
        }

        protected abstract E ExecuteOperation(I input);

        // performs an assert using the comparer passed in during construction
        protected virtual void ExecuteAssertion(Testcase testcase, E actualValue)
        {
            string errorMessage = String.Format("Test failure '{0}' input: '{1}' expected:'{2}' actual:'{3}", 
                testcase.Name,
                testcase.Input.ToString(),
                testcase.ExpectedValue,
                actualValue);

            this.asserter(testcase.ExpectedValue, actualValue, errorMessage);
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
        public override void ExecuteTests()
        {
            IEnumerable<Testcase> testcases = GetTestcases();
            foreach(Testcase testcase in testcases)
            {
                E actualValue = ExecuteOperation(testcase.Input);
                ExecuteAssertion(testcase, actualValue);
            }
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
            IEnumerable<MemberInfo> members;
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            Type thisType = this.GetType();
            
            // Members (fields, properties, methods)
            members = thisType.GetFields(flags);
            members = Enumerable.Concat(members, thisType.GetProperties(flags));
            members = Enumerable.Concat(members, thisType.GetMethods(flags));
            foreach (Testcase testcase in ProcessMembers(members))
            {
                yield return testcase;
            }
        }

        private IEnumerable<Testcase> ProcessMembers(IEnumerable<MemberInfo> members)
        {
            foreach(MemberInfo member in members)
            {
                TestDataAttribute attribute = (TestDataAttribute) Attribute.GetCustomAttribute(
                    member, 
                    typeof(TestDataAttribute), 
                    true);

                if (attribute != null)
                {
                    // We have a valid test attribute - the input/expectedValue can come either inline from the attribute, or the value of the member produces the values
                    object value = GetMemberValue(member);
                    foreach (Testcase testcase in GetTestcasesFromReflection(value, member, attribute))
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
                return field.GetValue(this);
            }
            else if (property != null)
            {
                return property.GetValue(this);
            }
            else if (method != null)
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
            else
            {
                throw new InvalidOperationException("invalid member info");
            }
        }
        
        private IEnumerable<Testcase> GetTestcasesFromReflection(object value, MemberInfo member, TestDataAttribute attribute)
        {
            Testcase singleTestcase = value as Testcase;
            IEnumerable<Testcase> multipleTestcases = value as IEnumerable<Testcase>;
            Tuple<I, E> shortformTestcase = value as Tuple<I, E>;
            IEnumerable<Tuple<I, E>> multipleShortformTestcase = value as IEnumerable<Tuple<I, E>>;
            if (singleTestcase != null)
            {
                singleTestcase.ConfigureName(attribute.Name, member.Name);
                yield return singleTestcase;
            }
            else if (multipleTestcases != null)
            {
                int index = 1;
                foreach (Testcase testcase in multipleTestcases)
                {
                    testcase.ConfigureName(attribute.Name, member.Name, index++);
                    yield return testcase;
                }
            }
            else if (shortformTestcase != null)
            {
                yield return new Testcase(shortformTestcase.Item1, shortformTestcase.Item2);
            }
            else if (multipleShortformTestcase != null)
            {
                foreach(Tuple<I, E> shortForm in multipleShortformTestcase)
                {
                    yield return new Testcase(shortForm.Item1, shortForm.Item2);
                }
            }
            else
            {
                throw new InvalidOperationException("invalid type returned from TestcaseAttribute");
            }
        }
    }
}
