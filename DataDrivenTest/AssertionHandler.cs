// Copyright TinyDigit 2015
// -- Pinky --/

namespace TinyDigit.DataTest
{
    /// <summary>
    /// Base class for dealing with a test Assertion for each testcase
    /// </summary>
    public abstract class AssertionHandler<T>
    {
        /// <summary>
        /// This method is called everytime a testcase is validated
        /// </summary>
        /// <param name="expected">Expected value - comes from the testcase data</param>
        /// <param name="actual">Actual value - comes from calling ExecuteOperation(input)</param>
        /// <param name="errorMessage">Error message to be used if there is a failure. Message is created by the DataTestClass framework</param>
        public abstract void TestcaseAssert(T expected, T actual, string errorMessage);

        /// <summary>
        /// This method is called at the end of executing all testcases in a DataTestClass (e.g., end of ExecuteTests).
        /// The implementor of DataTestClass must be sure to 1st wire up to the DataTestClassCompleted event, and then must explicitly call this method.
        /// </summary>
        /// <param name="errorMessage">The error message for all failed testcases in the DataTestClass</param>
        public abstract void TestCompleteAssert(string errorMessage);
    }
}
