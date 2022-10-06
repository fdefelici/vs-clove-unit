using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CLoveUnitTestAdapter.Core
{
    public class XProcResult
    {
        public int ExitCode { get; internal set; }
        public string StdOut { get; internal set; }
        public bool IsSuccess { get; internal set; }
        public bool IsError { get { return !IsSuccess; } }
    }

    public class XExecutor
    {
        //TODO: Loggare il comando
        public static bool Exec(string binaryPath, string[] args, Action<string> success, Action<int, string> error = null)
        {
            StringBuilder argsBuilder = new StringBuilder();
            foreach (string arg in args) argsBuilder.Append(arg).Append(" ");
            string argsString = argsBuilder.ToString();
            return Exec(binaryPath, argsString, success, error);
        }

        public static bool Exec(string binaryPath, string args, Action<string> success, Action<int, string> error = null)
        {
            Process process = new Process();
            process.StartInfo.FileName = binaryPath;
            process.StartInfo.Arguments = args;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            //TODO: Waiting timeout?
            //ibraries/Catch2Interface/Discoverer.cs::GetTestCaseInfo(...)
            process.WaitForExit();

            int code = process.ExitCode;
#pragma warning disable VSTHRD002
            string stdout = outputTask.Result; //skip wait warning because of process.WaitForExit() called before
#pragma warning restore VSTHRD002

            if (code == 0)
            {
                success(stdout);
                return true;
            }
            else
            {
                if (error != null) error(code, stdout);
                return false;
            }
        }


        public static XProcResult Exec(string binaryPath, string args)
        {
            Process process = new Process();
            process.StartInfo.FileName = binaryPath;
            process.StartInfo.Arguments = args;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            //TODO: Waiting timeout?
            //ibraries/Catch2Interface/Discoverer.cs::GetTestCaseInfo(...)
            process.WaitForExit();

            int code = process.ExitCode;
#pragma warning disable VSTHRD002
            string stdout = outputTask.Result; //skip wait warning because of process.WaitForExit() called before
#pragma warning restore VSTHRD002

        
            XProcResult result = new XProcResult();
            result.StdOut = stdout;
            result.ExitCode = code;
            result.IsSuccess = code == 0;

            return result;
        }
    }
}
