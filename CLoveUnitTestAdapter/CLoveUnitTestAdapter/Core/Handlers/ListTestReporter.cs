using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLoveUnitTestAdapter.Core.Handlers
{
    public interface IListTestReporter
    {
        void ReportTest(TestCase test);
    }

    public class ListTestUIReporter : IListTestReporter
    {
        private ITestCaseDiscoverySink _sink;

        public ListTestUIReporter(ITestCaseDiscoverySink sink)
        {
            _sink = sink;
        }

        public void ReportTest(TestCase test)
        {
            _sink.SendTestCase(test);
        }
    }


    public class ListTestTaskReporter : IListTestReporter
    {
        private List<TestCase> _tests;

        public ListTestTaskReporter(out List<TestCase> outTests)
        {
            outTests = new List<TestCase>();
            _tests = outTests;
        }

        public void ReportTest(TestCase test)
        {
            _tests.Add(test);
        }
    }

}
