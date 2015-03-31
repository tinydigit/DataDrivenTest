namespace tinydigit.visualstudio.datatest.plugin
{
    ////using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    //using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using System;
    using System.Collections.Generic;

    public class PluginDataTestDiscoverer : MarshalByRefObject//, ITestDiscoverer
    {

        public void DiscoverTests(
            IEnumerable<string> sources
            //IDiscoveryContext discoveryContext,
            //IMessageLogger logger,
            //ITestCaseDiscoverySink discoverySink)
            )
        {
            //logger.SendMessage(TestMessageLevel.Informational, "got here");
        }
    }
}
