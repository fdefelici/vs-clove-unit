using CLoveUnitTestAdapter.Core.Commands;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;

namespace CLoveUnitTestAdapter.Core
{
    public class CloveCommands
    {

        public static CloveVersionCommand Version(XTestProcessMgr procMgr, string binaryPath, XLogger logger)
        {
            return new CloveVersionCommand(procMgr, binaryPath, logger);
        }

        public static CloveListTestsCmd ListTests(XTestProcessMgr procMgr, string binaryPath, XLogger logger, Action<TestCase> onTestFound)
        {
            return new CloveListTestsCmd(procMgr, binaryPath, logger, onTestFound);
        }
           
        public static CloveRunTestsCmd RunTests(XTestProcessMgr procMgr, string binaryPath, XLogger logger, IEnumerable<TestCase> tests, bool isRunTestSelective, ITestExecutionRecorder recorder, string reportBasePath)
        {
            return new CloveRunTestsCmd(procMgr, binaryPath, logger, tests, isRunTestSelective, recorder, reportBasePath);
        }
    }

    public abstract class ACloveCommand 
    {
        private XTestProcessMgr _procMgr;
        protected string _binaryPath;
        protected IXLogger _logger;
        public bool IsFailure { get { return !IsSuccess; } }
        public bool IsSuccess { get; internal set; }
        public string StdOut { get; internal set; }

        public ACloveCommand(XTestProcessMgr procMgr, string binaryPath, IXLogger logger)
        {
            _procMgr = procMgr;
            _binaryPath = binaryPath;
            _logger = logger;
        }

        public void Run()
        {
            IsSuccess = BeforeExecute();
            if (!IsSuccess) return;

            XProcResult exec = _procMgr.RunProcess(_binaryPath, GetArgs());
            StdOut = exec.StdOut;

            IsSuccess = AfterExecute(exec);
        }

        protected abstract string GetArgs();

        protected abstract bool BeforeExecute();
        protected abstract bool AfterExecute(XProcResult execResult);
    }
}
