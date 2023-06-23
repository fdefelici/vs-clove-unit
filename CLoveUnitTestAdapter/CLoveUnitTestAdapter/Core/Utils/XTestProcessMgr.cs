
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Threading;

namespace CLoveUnitTestAdapter.Core
{
    public class XNoDebugRunContext : IRunContext
    {
        public bool KeepAlive => throw new NotImplementedException();

        public bool InIsolation => throw new NotImplementedException();

        public bool IsDataCollectionEnabled => throw new NotImplementedException();

        public bool IsBeingDebugged => false;

        public string TestRunDirectory => throw new NotImplementedException();

        public string SolutionDirectory => throw new NotImplementedException();

        public IRunSettings RunSettings => throw new NotImplementedException();

        public ITestCaseFilterExpression GetTestCaseFilter(IEnumerable<string> supportedProperties, Func<string, TestProperty> propertyProvider)
        {
            throw new NotImplementedException();
        }
    }

    public class XDebugProcess
    {
        public int ExitCode { get; private set; }
        private bool _exited;

        public XDebugProcess(Process process)
        {
            process.EnableRaisingEvents = true;
            process.Exited += OnExited;

            ExitCode = -1;
            _exited = false;
        }

        public int WaitForExit()
        {
            lock (this)
            {
                while (!_exited)
                {
                    Monitor.Wait(this);
                }
            }

            return ExitCode;
        }

        private void OnExited(object sender, EventArgs e)
        {
            if (sender is Process process)
            {
                lock (this)
                {
                    ExitCode = process.ExitCode;
                    _exited = true;

                    process.Exited -= OnExited;

                    Monitor.Pulse(this);
                }
            }
        }
    }

    public class XTestProcessMgr
    {
        private readonly IRunContext _runContext;
        private readonly IFrameworkHandle _fwkHandle;

        public XTestProcessMgr(IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            _runContext = runContext;
            _fwkHandle = frameworkHandle;
        }

        public virtual XProcResult RunProcess(string exec, string args)
        {
            if (_runContext.IsBeingDebugged) return RunDebug(exec, args);
            else return RunNormal(exec, args);
        }

        private XProcResult RunNormal(string exec, string args)
        {
            return XExecutor.Exec(exec, args);
        }

        private XProcResult RunDebug(string exec, string args)
        {
            string workDir = Path.GetDirectoryName(exec);

            int pid = _fwkHandle.LaunchProcessWithDebuggerAttached(
                exec, workDir, args, new Dictionary<string, string>()
            );

            Process proc = Process.GetProcessById(pid);
            //NOTE: After waiting when calling ExitCode, the following exception is thrown.
            //      seems that debug process wasn't really 'ready' with the exit code.
            //proc.WaitForExit()
            //int code = proc.ExitCode;  
            /*
               An exception occurred while invoking executor 'executor://cloveunittestexecutor/': Process was not started by this object, so requested information cannot be determined.
               Stack trace:
                  at System.Diagnostics.Process.EnsureState(State state)
                  at System.Diagnostics.Process.get_ExitCode()
                  ....
             */

            //As workaround using this 'passive' polling on exist status with XDebugProcessAdapter.
            XDebugProcess dgbProc = new XDebugProcess(proc);
            dgbProc.WaitForExit();

            XProcResult result = new XProcResult();
            result.StdOut = "";  //NOTE: cannot read stdout when in debug mode
            result.ExitCode = proc.ExitCode;
            result.IsSuccess = result.ExitCode == 0;

            return result;
        }

        public static XTestProcessMgr NoDebug()
        {
            return new XTestProcessMgr(new XNoDebugRunContext(), null);
        }

        public static XTestProcessMgr FromContext(IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            return new XTestProcessMgr(runContext, frameworkHandle);
        }
    }
}
