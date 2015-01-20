// Copyright TinyDigit
// -- Pinky --/

namespace TinyDigit.Test
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, Inherited = true)]
    public class TestcaseAttribute : Attribute
    {
        public TestcaseAttribute() { }

        public TestcaseAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}