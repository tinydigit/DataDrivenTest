// Copyright TinyDigit
// -- Pinky --/

namespace TestVisualStudioDataTestPlugin
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using datatest = tinydigit.visualstudio.datatest;
    using plugin = tinydigit.visualstudio.datatest.plugin;

    [TestClass]
    public class PluginLoadTests
    {
        [TestMethod]
        public void TestLoad()
        {
            var dataTest = new datatest.DataTestDiscoverer();
            var logger = new TestLogger();
            var context = new TestDiscoveryContext();
            var sink = new TestCaseDiscoverySink();
            string[] sources = new string[0];

            dataTest.DiscoverTests(sources, context, logger, sink);
            Assert.AreEqual(TestMessageLevel.Informational, logger.LastMessageLevel);
            Assert.AreEqual("got here", logger.LastMessage);
        }

        private class TestLogger : IMessageLogger
        {
            public TestMessageLevel LastMessageLevel { get; set; }
            public string LastMessage { get; set; }

            public void SendMessage(TestMessageLevel testMessageLevel, string message)
            {
                this.LastMessageLevel = testMessageLevel;
                this.LastMessage = message;
            }
        }

        private class TestCaseDiscoverySink : ITestCaseDiscoverySink
        {
            public TestCase LastTestCase { get; set; }
            public void SendTestCase(TestCase discoveredTest)
            {
                this.LastTestCase = discoveredTest;
            }
        }

        private class TestDiscoveryContext : IDiscoveryContext
        {
            public IRunSettings RunSettings
            {
                get { return new TestRunSettings(); }
            }

            private class TestRunSettings : IRunSettings
            {

                public ISettingsProvider GetSettings(string settingsName)
                {
                    return null;
                }

                public string SettingsXml
                {
                    get { return string.Empty; }
                }
            }
        }
    }
}
