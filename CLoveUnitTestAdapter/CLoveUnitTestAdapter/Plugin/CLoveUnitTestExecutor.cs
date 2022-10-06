using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using CLoveUnitTestAdapter.Core;
using System;

namespace CLoveUnitTestAdapter.Extension
{
    [ExtensionUri(CloveConfig.TestExecutorUri)]
    public class CLoveUnitTestExecutor : ITestExecutor
    {
        public CLoveUnitTestExecutor()
        {
            _lock = new object();
            _canceled = false;

            _logger = null;
            _runTestsHandler = null;
        }

        public void Cancel()
        {
            if (_canceled) return; //pre-avoiding lock
            lock (_lock)
            {
                if (_canceled) return;
                _canceled = true;
                _runTestsHandler.Cancel();
            }
        }

        //RunTests from TestExplorer UI
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {   
            _logger = XLogger.From(frameworkHandle);
            CloveRunSettings settings = CloveRunSettings.FromExecution(runContext, _logger);
            
            _logger.DebugEnabled = settings.DebugEnabled;   
            _logger.Debug("RunTests from tests...");

            CloveCacheMgr cacheMgr = CloveCacheMgr.Setup(settings.CachePath);
            XTestProcessMgr procMgr = XTestProcessMgr.FromContext(runContext, frameworkHandle);

            _runTestsHandler = new RunTestsHandler(cacheMgr, procMgr, frameworkHandle, _logger);
            _runTestsHandler.Execute(tests);
        }

        //Discovery + RunTests
        //NOTE: Seems not to be invoked when in Editor wrote TestExplorer UI
        //      For sure called when using vstest.console.exe from command-line 
        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            _logger = XLogger.From(frameworkHandle);

            CloveRunSettings settings = CloveRunSettings.FromExecution(runContext, _logger);
            _logger.DebugEnabled = settings.DebugEnabled;

            _logger.Debug("RunTests from sources...");
            _logger.Debug($"Adapter enabled: {settings.AdapterEnabled}");
            _logger.Debug($"Cache path: {settings.CachePath}");
            
            if (!settings.AdapterEnabled)
            {
                _logger.Warn("Test run stopped because adapter is disabled!");
                return;
            }

            CloveCacheMgr cacheMgr = CloveCacheMgr.Setup(settings.CachePath);
            XTestProcessMgr procMgrNoDebug = XTestProcessMgr.NoDebug();
            XTestProcessMgr procMgrAsIs = XTestProcessMgr.FromContext(runContext, frameworkHandle);

            //Discovery (no UI)
            ListTestsHandler listTestsHandler = ListTestsHandler.ForTask(cacheMgr, settings, procMgrNoDebug, out List<TestCase> tests, _logger);
            listTestsHandler.Execute(sources);

            //Run Tests
            _runTestsHandler = new RunTestsHandler(cacheMgr, procMgrAsIs, frameworkHandle, _logger);
            _runTestsHandler.Execute(tests);            
        }

        private XLogger _logger;
        private RunTestsHandler _runTestsHandler;
        private readonly object _lock;
        private bool _canceled;
    }
}
