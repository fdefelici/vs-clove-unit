using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using System.Linq;

namespace CLoveUnitTestAdapter.Core
{
    public class RunTestsHandler
    {
        private readonly CloveCacheMgr _cacheMgr;
        private readonly XTestProcessMgr _procMgr;
        private readonly IFrameworkHandle _reporter;
        private readonly XLogger _logger;
        private bool _canceled;
        private RunTestsOnBinaryHandler _runner;

        public RunTestsHandler(CloveCacheMgr cacheMgr, XTestProcessMgr procMgr, IFrameworkHandle reporter, XLogger logger)
        {
            _cacheMgr = cacheMgr;
            _procMgr = procMgr;
            _reporter = reporter;
            _logger = logger;
            _canceled = false;
            _runner = new RunTestsOnBinaryHandler(_cacheMgr, procMgr, _reporter, _logger);
        }

        public void Execute(IEnumerable<TestCase> tests)
        {
            var testsPerBinary = tests.GroupBy(each => each.Source).ToList();
            foreach (var testGroup in testsPerBinary)
            {
                if (_canceled) return;
                XBinaryFile binary = new XBinaryFile(testGroup.Key);
                _runner.Execute(binary, testGroup);
            }
        }

        public void Cancel()
        {
            _canceled = true;
        }
    }
}
