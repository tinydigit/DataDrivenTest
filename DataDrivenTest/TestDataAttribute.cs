// Copyright TinyDigit 2015
// -- Pinky --/

namespace TinyDigit.DataTest
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class TestDataAttribute : Attribute
    {
        public TestDataAttribute() { }

        public TestDataAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}