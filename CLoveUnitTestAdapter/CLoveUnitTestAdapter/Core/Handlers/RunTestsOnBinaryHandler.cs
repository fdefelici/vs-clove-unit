using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using System.Linq;
using CLoveUnitTestAdapter.Core.Commands;

namespace CLoveUnitTestAdapter.Core
{
    public class RunTestsOnBinaryHandler
    {
        private readonly CloveCacheMgr _cacheMgr;
        private readonly XTestProcessMgr _procMgr;
        private readonly ITestExecutionRecorder _recorder;
        private readonly XLogger _logger;

        public RunTestsOnBinaryHandler(CloveCacheMgr cacheMgr, XTestProcessMgr procMgr, IFrameworkHandle recorder, XLogger logger)
        {
            _cacheMgr = cacheMgr;
            _procMgr = procMgr;
            _recorder = recorder;
            _logger = logger;
        }

        public void Execute(XBinaryFile binary, IEnumerable<TestCase> tests)
        {
            //Case 1: Missing cache for binary
            //        Should never happen, because tests execution is only on binary from the "discovery" phase
            string cacheId = binary.FileNameNoExt;
            string binaryPath = binary.FilePath;

            if (!_cacheMgr.HasCache(cacheId))
            {
                _logger.Debug($"Cache file not found for binary: {binaryPath}");
                _logger.Debug($"Binary is presumed to not be a clove-unit executable! Execution stopped!");
                return;
            }

            //Case 2: Not a clove binary
            //        Should never happen, because tests execution is only on binary from the "discovery" phase
            CloveCache cache = _cacheMgr.LoadCache(cacheId);
            bool isCloveUnitBin = cache.ReadProp(CloveConfig.CacheProp_IsCloveBinary, false);
            if (!isCloveUnitBin)
            {
                _logger.Debug($"Skipping non clove-unit binary: {binaryPath}");
                return;
            }

            //Case 3: Normal Flow
            int launchedTestCount = tests.Count();
            int discoveredTestCount = cache.ReadProp(CloveConfig.CacheProp_DiscoveredTests, launchedTestCount);
            bool isRunSelective = launchedTestCount != discoveredTestCount;
            _logger.Debug($"Discovered Tests: {discoveredTestCount} - Launched Tests: {launchedTestCount} - SelectiveMode: {isRunSelective}");

            _logger.Info($"Running {launchedTestCount}/{discoveredTestCount} test(s) from {binaryPath}");
            CloveRunTestsCmd runTestCmd = CloveCommands.RunTests(_procMgr, binaryPath, _logger, tests, isRunSelective, _recorder, _cacheMgr.CacheRootPath);
            runTestCmd.Run();
        }
    }
}