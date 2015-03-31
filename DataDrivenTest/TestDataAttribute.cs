// Copyright TinyDigit 2015
// -- Pinky --/

namespace TinyDigit.DataTest
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class TestDataAttribute : Attribute
    {
        private readonly object input;
        private readonly object expectedValue;
        private readonly bool dataSpecified = false;

        public TestDataAttribute() { }

        public TestDataAttribute(object input, object expectedValue)
        {
            this.input = input;
            this.expectedValue = expectedValue;
            this.dataSpecified = true;
        }

        public TestDataAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        internal bool InlineDataSpecified { get { return this.dataSpecified; } }

        public virtual object Input { get { return this.input; } }

        public virtual object ExpectedValue { get { return this.expectedValue; } }
    }
}