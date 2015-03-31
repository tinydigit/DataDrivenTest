// Copyright TinyDigit
// -- Pinky --/

namespace tinydigit.visualstudio.datatest
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Security.Policy;

    public class DataTestDiscoverer : ITestDiscoverer
    {
        private const string PluginAssemblyName = "tinydigit.visualstudio.datatest.plugin.dll";
        private const string CopiedPluginAssemblyName = "tinydigit.visualstudio.datatest.plugin.copy.dll";
        private const string PluginTypeName = "TinyDigit.VisualStudio.DataTest.Plugin.PluginDataTestDiscoverer";
        private DateTime latestVersion = DateTime.MinValue;
        private AppDomain pluginDomain;
        private readonly string pluginPath; // cache the path so you don't have to recreate
        private ITestDiscoverer pluginDiscoverer;

        public DataTestDiscoverer()
        {
            string currentPath = this.GetType().Assembly.Location;
            this.pluginPath = Path.Combine(Path.GetDirectoryName(currentPath), PluginAssemblyName);
        }

        public void DiscoverTests(
            IEnumerable<string> sources, 
            IDiscoveryContext discoveryContext, 
            IMessageLogger logger, 
            ITestCaseDiscoverySink discoverySink)
        {
            // First check to see if our plugin assembly has been refreshed
            if (IsPluginRefreshAvailable())
            {
                RefreshPlugin(logger);
            }


        }

        private bool IsPluginRefreshAvailable()
        {
            DateTime lastWriteTime = File.GetLastWriteTimeUtc(this.pluginPath);
            return this.latestVersion < lastWriteTime;
        }

        private void RefreshPlugin(IMessageLogger logger)
        {
            // Unload the current plugin
            if (this.pluginDomain != null)
            {
                try
                {
                    AppDomain.Unload(this.pluginDomain);
                }
                catch(CannotUnloadAppDomainException exception)
                {
                    string message = String.Format("Unable to unload {0}. Error is {1}", CopiedPluginAssemblyName, exception.Message);
                    logger.SendMessage(TestMessageLevel.Error, message);
                }
            }
            // Copy the new file to the copy version
            string copyPath = Path.Combine(Path.GetDirectoryName(this.pluginPath), CopiedPluginAssemblyName);
            File.Copy(this.pluginPath, copyPath, true);
            AppDomainSetup setup = new AppDomainSetup()
            {
                PrivateBinPath = Path.GetDirectoryName(this.pluginPath)
            };
            this.pluginDomain = AppDomain.CreateDomain("tinyfinger.visualstudio.datatest.plugin", null, setup);
            this.pluginDomain.Load(copyPath);
            //this.pluginDiscoverer = (ITestDiscoverer)this.pluginDomain.CreateInstance(copyPath, PluginTypeName);
        }
    }
}