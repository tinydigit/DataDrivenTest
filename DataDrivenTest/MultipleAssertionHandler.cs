// Copyright TinyDigit 2015
// -- Pinky --/

namespace TinyDigit.DataTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Base class for dealing with a multiple test Assertions for all of the testcases in a test class
    /// </summary>
    internal class MultipleAssertionHandler<T> : AssertionHandler<T>
    {
        private List<Exception> exceptions;
        private readonly AssertionHandler<T> innerAssertion;

        public MultipleAssertionHandler(AssertionHandler<T> innerAssertion)
        {
            if (innerAssertion == null)
            {
                throw new ArgumentNullException("innerAssertion");
            }
            this.innerAssertion = innerAssertion;
        }

        private IList<Exception> ExceptionList
        {
            get
            {
                if (this.exceptions == null)
                {
                    this.exceptions = new List<Exception>();
                }
                return this.exceptions;
            }
        }

        public override void TestcaseAssert(T expected, T actual, string errorMessage)
        {
            try
            {
                this.innerAssertion.TestcaseAssert(expected, actual, errorMessage);
            }
            catch(Exception e)
            {
                ExceptionList.Add(e);
            }
        }

        public override void TestCompleteAssert(string errorMessage)
        {
            // null if never initialized because no exceptions were ever created
            if (this.exceptions != null && this.exceptions.Count > 0)
            {
                StringBuilder errorBuilder = new StringBuilder();
                for (int i = 0; i < this.exceptions.Count; i++)
                {
                    Exception e = this.exceptions[i];
                    errorBuilder.AppendLine(e.Message);
                }
                this.innerAssertion.TestCompleteAssert(errorBuilder.ToString());
            }
        }
    }
}
