# DataDrivenTest
Build data-driven tests in .Net using objects.

# Introduction
I was in the process of writing a little parser. I like to test my code, but I wanted to write a bunch of examples of the output (tokenization and semantic graph) given various test input. The problem is that using MSTest, or even other test frameworks with Visual Studio, did not let me do this easily.

I spent a little bit of time playing with some of the data-driven test frameworks already built by *cool people*. The built in capability in MSTest required me to serialize my data into an XML or SQL database. OK, but what if my serialization code has a bug. I just wanted to new up some objects. I also looked at some existing libraries on the web. Those provided ways to inline data in CLR attributes. Cool, but CLR attributes limit the kinds of values I can use. I just wanted to new up some objects. I decided I wanted to just new up some objects. Thus the birth of this data-driven test library.

## Simple Example
Simply put, I wanted to new up some objects as my input data and/or my expected values. Here's a simple example of a data-driven test using this library:


```c#
private class TokenizationTests : MSTestDataTest<string, IEnumerable<Token>>
{
    [TestData]
    private Testcase[] singleTokenTests = new Testcase[]
    {
        new Testcase 
        { 
            Input = "1", 
            ExpectedValue = new Token[]
            {
                new ConstantToken { Value = 1, ValueKind = ValueKind.Number, StartIndex = 0, Length = 1 }
            }
        },
        new Testcase 
        { 
            Input = "     ", 
            ExpectedValue = new Token[]
            {
                new WhitespaceToken { Value = "     ", StartIndex = 0, Length = 5 }
            }
        },
    };

    [TestData]
    private Testcase[] constantExpressionTest = new Testcase[]
    {
        new Testcase 
        { 
            Input = "1+1", 
            ExpectedValue = new Token[]
            {
                new ConstantToken { Value = 1, ValueKind = ValueKind.Number, StartIndex = 0, Length = 1 },
                new BinaryOperatorToken { Operator = BinaryOperator.Plus, StartIndex = 1, Length = 1 },
                new ConstantToken { Value = 1, ValueKind = ValueKind.Number, StartIndex = 2, Length = 1 }
            }
        },
    };

    protected override IEnumerable<Token> ExecuteOperation(string input)
    {
        ExpressionParser parser = new ExpressionParser();
        return parser.Tokenize(input);
    }
}
```

# Implementation Steps
Follow these general steps to implement a data-driven test.

    1. Pull down the nuget package and add it to your test project
    2. For each test class that you want data-driven tests, inherit from the appropriate data-driven base class depending on your test runner (see below).
       * The class has 2 generic type parameters. The first is the Input type. The second is the ExpectedValue type.
    3. Override the method, ExecuteOpration. ExecuteOperation performs whatever steps needed to translate the input value to the expected result.
    4. The default assertion is to do an equals comparison between expected and actual (actual being the result produced in #4, the ExecuteOperation). If you want to customize the assertion, override
    5. Create instances of Testcase<I, E>, where I is the input type and E is the expected value type.
       * Instances can be created in properties, fields, or as a result of static or instance methods.
       * Whatever member you use, the type of the field or property, or the return type of the method can be either a Testcase values can be single Testcase instance or an IEnumerable<Testcase> values
    6. Go!

*Note - in some instances, the Attributes used by the test runner framework to identify a test class and test methods are not always being discovered correctly from the base class by the test runner. Therefore, you may need to add them to your test class that inherits from the data-driven base class. To add the test method attribute, override the ExecuteTests method. For example:

```c#
    [TestClass]
    public class MyTest : MSTestDataTest<string, int>
    {
        [TestMethod]
        protected override void ExecuteTests()
        {
            base.ExecuteTests();
        }
    }
```

# Test Driver Support
The intention of this library is to support the different test drivers used by .Net developers. The following test runners are supported:

* MSTest

## MSTest

To use MSTest, inherit your test class from MSTestDataTest. 

## XUnit
coming soon...

## NUnit
coming soon...

# Future Investments

* Deeper Visual Studio integration, namely, showing each Testcase as a separate line item in the Test explorer.
* Catch assertion exceptions and continue running, capturing all of the results in 1 "report"

# Links
* [Nuget package](http://www.nuget.org/packages/TinyDigit.DataTest)
* [me](http://www.tinyfinger.com)

## About
I am a Microsoft employee, a very conservative and hesitant web participant, and my nickname is pinky (thus TinyDigit, TinyFinger, etc.). 