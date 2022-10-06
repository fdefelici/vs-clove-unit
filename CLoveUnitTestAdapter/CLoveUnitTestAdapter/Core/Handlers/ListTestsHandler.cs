using CLoveUnitTestAdapter.Core.Handlers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using System.Linq;

namespace CLoveUnitTestAdapter.Core
{
    public class ListTestsHandler
    {
        private readonly CloveCacheMgr _cacheMgr;
        private readonly CloveRunSettings _settings;
        private readonly XTestProcessMgr _procMgr;
        private readonly IListTestReporter _reporter;
        private readonly XLogger _logger;
        public static ListTestsHandler ForUI(CloveCacheMgr cacheMgr, CloveRunSettings settings, XTestProcessMgr procMgr, ITestCaseDiscoverySink discoverySink, XLogger logger)
        {
            return new ListTestsHandler(cacheMgr, settings, procMgr, new ListTestUIReporter(discoverySink), logger);
        }
        public static ListTestsHandler ForTask(CloveCacheMgr cacheMgr, CloveRunSettings settings, XTestProcessMgr procMgr, out List<TestCase> outTests, XLogger logger)
        {
            return new ListTestsHandler(cacheMgr, settings, procMgr, new ListTestTaskReporter(out outTests), logger);
        }

        private ListTestsHandler(CloveCacheMgr cacheMgr, CloveRunSettings settings, XTestProcessMgr procMgr, IListTestReporter discoverySink, XLogger logger)
        {
            _cacheMgr = cacheMgr;
            _settings = settings;
            _procMgr = procMgr;
            _reporter = discoverySink;
            _logger = logger;
        }

        public void Execute(IEnumerable<string> sources)
        {
            List<XBinaryFile> cloveBinaries = FilterCloveBinaries(sources, _cacheMgr, _logger);

            foreach (XBinaryFile binary in cloveBinaries)
            {
                _logger.Debug($"Discovering: {binary.FilePath}");

                // Validate clove-unit version
                CloveVersionCommand versionCmd = CloveCommands.Version(_procMgr, binary.FilePath, _logger);
                versionCmd.Run();
                if (versionCmd.IsFailure) return;

                XVersion versionFound = versionCmd.Result;
                bool isCompatible = versionFound.HasSameMinor(CloveConfig.SupportedCloveVersion);
                if (!isCompatible)
                {
                    string supported = CloveConfig.SupportedCloveVersion.AsMinorString();
                    string current = versionFound.AsString();
                    _logger.Erro($"clove-unit.h v{current} has been detected for: ${binary.FilePath}");
                    _logger.Info($"This adapter is compatible with clove-unit.h v{supported}");
                    _logger.Info($"Please update this adapter (if any) or use a compatible clove-unit.h!");
                    return;
                }

                // Extract Tests from a valid clove-unit binary
                CloveListTestsCmd listTestsCmd = CloveCommands.ListTests(_procMgr, binary.FilePath, _logger, _reporter.ReportTest);
                listTestsCmd.Run();

                if (listTestsCmd.IsFailure) return;

                int testCount = listTestsCmd.TestCount;

                CloveCache cache = _cacheMgr.LoadCache(binary.FileNameNoExt);
                cache.WriteProp(CloveConfig.CacheProp_DiscoveredTests, testCount);

                _logger.Info($"Found {testCount} test(s) in {binary.FilePath}");
            }
        }

        private List<XBinaryFile> FilterCloveBinaries(IEnumerable<string> sources, CloveCacheMgr cacheMgr, XLogger logger)
        {
            logger.Debug($"Binaries Filtering started for: {sources.Count()}");

            List<XBinaryFile> result = new List<XBinaryFile>();

            foreach (string binaryPath in sources) //sources seems contain absolute file path
            {
                XBinaryFile binary = new XBinaryFile(binaryPath);
                if (!_settings.ExecNameRegex.IsMatch(binary.FileNameNoExt))
                {
                    logger.Debug($"Binary skipped, because 'TargetNameRegex' is not satisfied by target name {binary.FileNameNoExt} [{binary.FilePath}]");
                    continue;
                }

                logger.Debug($"Marked as clove-unit executable: {binary.FilePath}");
                string binName = binary.FileNameNoExt;
                CloveCache cache;
                if (!cacheMgr.HasCache(binName))
                {
                    logger.Debug($"Cache created for: {binary.FilePath}");
                    cache = cacheMgr.CreateCache(binName);
                } 
                else
                {
                    logger.Debug($"Cache loaded for: {binary.FilePath}");
                    cache = cacheMgr.LoadCache(binName);
                }
                cache.WriteProp(CloveConfig.CacheProp_IsCloveBinary, true);

                result.Add(binary);

                //NOTE: binary auto-discovery removed for now in favor of the TargeNameRegex above
                //      Code just commented out and not deleted because could come in handy in the future
                /*
                CloveCache cache;
                bool isCloveBinary;

                logger.Debug($"Evaluating binary: {binary.FilePath}");
                string binName = binary.FileNameNoExt;
                if (!cacheMgr.HasCache(binName))
                {
                    cache = cacheMgr.CreateCache(binName);
                    isCloveBinary = binary.Contains(CloveConfig.BinaryMagicStringRaw);
                    if (!isCloveBinary)
                    {
                        logger.Debug($"It isn't a clove-unit executable!");
                        cache.WriteProp(CloveConfig.CacheProp_IsCloveBinary, false);
                    }
                    else
                    {
                        logger.Debug($"It's a clove-unit executable!");
                        cache.WriteProp(CloveConfig.CacheProp_IsCloveBinary, true);
                    }
                }
                else
                {
                    cache = cacheMgr.LoadCache(binName);

                    isCloveBinary = cache.ReadProp(CloveConfig.CacheProp_IsCloveBinary, false);
                    if (!isCloveBinary)
                    {
                        logger.Debug($"It isn't a clove-unit executable (cached)!");
                    }
                    else
                    {
                        logger.Debug($"It's a clove-unit executable (cached)!");
                    }
                }

                if (isCloveBinary) result.Add(binary);
                */
            }
            logger.Debug($"Binaries filtering ended. Selected {result.Count()} out {sources.Count()}");
            return result;
        }
    }
}
