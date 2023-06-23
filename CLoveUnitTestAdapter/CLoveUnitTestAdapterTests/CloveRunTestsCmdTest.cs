using CLoveUnitTestAdapter.Core;
using CLoveUnitTestAdapter.Core.Commands;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Net.Mime.MediaTypeNames;


namespace CLoveUnitTestAdapterTests
{

    class XProcessMgrMock : XTestProcessMgr
    {
        public XProcessMgrMock() 
            : base(null, null) { }

        public override XProcResult RunProcess(string exec, string args)
        {
            Exec = exec;
            Args = args;

            return new XProcResult();
        }

        public string Exec { get; private set; }
        public string Args { get; private set; }
    }


    class RecorderMock : ITestExecutionRecorder
    {
        public void RecordAttachments(IList<AttachmentSet> attachmentSets)
        {
            
        }

        public void RecordEnd(TestCase testCase, TestOutcome outcome)
        {
            
        }

        public void RecordResult(Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult testResult)
        {
            
        }

        public void RecordStart(TestCase testCase)
        {
            
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            
        }
    }

    [TestClass]
    public class CloveRunTestsCmdTest
    {

        [TestMethod]
        public void CheckCommandExecAndReportPathWithSpaces()
        {
            var ProcMock = new XProcessMgrMock();
            var RecorderMock = new RecorderMock();
            var TestBinPath = Path.Combine("Path", "Test.exe");
            var Tests = new List<TestCase>();
            var ReportPath = Path.Combine("Path With Spaces");

            CloveRunTestsCmd cmd = new CloveRunTestsCmd(
                ProcMock,
                TestBinPath,
                new NullXLogger(),
                Tests,
                false,
                RecorderMock,
                ReportPath
            );
            cmd.Run();

            string ExpectedBinPath = TestBinPath;
            string ExpectedArgs = $"--run-tests --report json --output \"{Path.Combine("Path With Spaces", "Test_runtests.json")}\"";


            Assert.AreEqual(ExpectedBinPath, ProcMock.Exec);
            Assert.AreEqual(ExpectedArgs, ProcMock.Args);
        }

    }
}
