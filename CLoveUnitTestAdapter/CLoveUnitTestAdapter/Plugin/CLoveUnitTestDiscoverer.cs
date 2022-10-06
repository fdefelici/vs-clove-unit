using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using CLoveUnitTestAdapter.Core;

namespace CLoveUnitTestAdapter.Extension
{

    [FileExtension(CloveConfig.TestFileExtension)]
    [DefaultExecutorUri(CloveConfig.TestExecutorUri)]
    public class CLoveUnitTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger msgLogger, ITestCaseDiscoverySink discoverySink)
        { 
            XLogger logger = XLogger.From(msgLogger);
            CloveRunSettings settings = CloveRunSettings.FromDiscovery(discoveryContext, logger);
            logger.DebugEnabled = settings.DebugEnabled;

            logger.Debug(discoveryContext.RunSettings.SettingsXml);

            if (!settings.AdapterEnabled)
            {
                //If not enabled, stop silenlty (no logging)
                return;
            }

            CloveCacheMgr cacheMgr = CloveCacheMgr.Setup(settings.CachePath);
            XTestProcessMgr procMgrNoDebug = XTestProcessMgr.NoDebug();

            ListTestsHandler handler = ListTestsHandler.ForUI(cacheMgr, settings, procMgrNoDebug, discoverySink, logger);
            handler.Execute(sources);
        }
    }
}

