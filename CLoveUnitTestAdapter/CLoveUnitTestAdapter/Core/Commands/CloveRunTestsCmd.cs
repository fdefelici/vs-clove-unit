using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace CLoveUnitTestAdapter.Core.Commands
{
    public class CloveRunTestsCmd : ACloveCommand
    {
        private IEnumerable<TestCase> _tests;
        private bool _isRunTestSelective;
        private ITestExecutionRecorder _recorder;
        private string _jsonReportPath;

        public CloveRunTestsCmd(
            XTestProcessMgr procMgr, string binaryPath, IXLogger logger, 
            IEnumerable<TestCase> tests, bool isRunTestSelective,
            ITestExecutionRecorder recorder, string reportBasePath) 
            : base(procMgr, binaryPath, logger)
        {
            _tests = tests;
            _isRunTestSelective = isRunTestSelective;
            _recorder = recorder;

            string fileName = $"{Path.GetFileNameWithoutExtension(binaryPath)}{CloveConfig.RunTestsFileNameSuffix}";
            _jsonReportPath = Path.Combine(reportBasePath, fileName);
        }

        protected override string GetArgs()
        {
            StringBuilder runArgs = new StringBuilder();
            runArgs.Append($"--run-tests --report json --output \"{_jsonReportPath}\"");
            if (_isRunTestSelective)
            {
                foreach (TestCase eachTest in _tests)
                {
                    runArgs.Append($" --include {eachTest.FullyQualifiedName}");
                }
            }
            return runArgs.ToString();
        }

        protected override bool BeforeExecute()
        {
            //Start all tests in UI
            foreach (TestCase eachTest in _tests)
            {
                _recorder.RecordStart(eachTest);
            }
            return true;
        }

        protected override bool AfterExecute(XProcResult execResult)
        {
            if (execResult.IsError)
            {
                _logger.Erro("Tests Execution failed!");
                _logger.Trace(execResult.StdOut);
                return false;
            }

            _logger.Debug("Test Execution completed!");
            string jsonStr = File.ReadAllText(_jsonReportPath);
            //string jsonStr = execResult.StdOut;

            CloveJsonReport json = JsonConvert.DeserializeObject<CloveJsonReport>(jsonStr);

            foreach (TestCase eachTest in _tests)
            {
                //TODO: add in test.Properties instead of splitting fully name?!
                string[] splitted = eachTest.FullyQualifiedName.Split('.');
                string suiteName = splitted[0];
                string testName = splitted[1];

                CloveJsonSuite jsonSuite = json.result.suites[suiteName];
                CloveJsonTest jsonTest = jsonSuite.tests[testName];

                TestResult testResult = new TestResult(eachTest);

                //NOTE: Add standard out only when 1 test is running,
                //      because by now, can just take the stdout of the whole run tests process
                //      and with multiple tests, all tests will have same Standard Out message.
                //      By now could be a good compromise, considering that printing on console 
                //      is not a good practice, but could help for debugging.
                if (_tests.Count() == 1) 
                { 
                    testResult.Messages.Add(
                        new TestResultMessage(TestResultMessage.StandardOutCategory, StdOut));
                }

                switch (jsonTest.status)
                {
                    case "PASS":
                        {
                            testResult.Outcome = TestOutcome.Passed;
                            testResult.Duration = new TimeSpan(jsonTest.duration / 100);
                            break;
                        }
                    case "FAIL":
                        {
                            testResult.Outcome = TestOutcome.Failed;
                            testResult.Duration = new TimeSpan(jsonTest.duration / 100);

                            string assertMsg;
                            string type = jsonTest.type;
                            string exp = jsonTest.expected;
                            string act = jsonTest.actual;
                            if (type == "STRING")
                            {
                                exp = JsonConvert.SerializeObject(exp); //Used to make char escaping
                                act = JsonConvert.SerializeObject(act); //Used to make char escaping
                            }

                            switch (jsonTest.assert)
                            {
                                case "EQ":
                                    {
                                        assertMsg =
                                            $"Expected:{Environment.NewLine}" +
                                            $"   {exp}{Environment.NewLine}" +
                                            $"Actual:{Environment.NewLine}" +
                                            $"   {act}";
                                        break;
                                    }
                                case "NE":
                                    {
                                        assertMsg =
                                            $"Not Expected:{Environment.NewLine}" +
                                            $"   {exp}{Environment.NewLine}" +
                                            $"Actual:{Environment.NewLine}" +
                                            $"   {act}";
                                        break;
                                    }
                                case "GT":
                                    {
                                        assertMsg =
                                            $"Expected:{Environment.NewLine}" +
                                            $"   {exp} > {act}{Environment.NewLine}" +
                                            $"But wasn't";
                                        break;
                                    }
                                case "GTE":
                                    {
                                        assertMsg =
                                            $"Expected:{Environment.NewLine}" +
                                            $"   {exp} >= {act}{Environment.NewLine}" +
                                            $"But wasn't";
                                        break;
                                    }
                                case "LT":
                                    {
                                        assertMsg =
                                            $"Expected:{Environment.NewLine}" +
                                            $"   {exp} < {act}{Environment.NewLine}" +
                                            $"But wasn't";
                                        break;
                                    }
                                case "LTE":
                                    {
                                        assertMsg =
                                            $"Expected:{Environment.NewLine}" +
                                            $"   {exp} <= {act}{Environment.NewLine}" +
                                            $"But wasn't";
                                        break;
                                    }
                                case "FAIL": { assertMsg = $"a fail assertion has been met!"; break; }
                                default: { assertMsg = "<undefined>"; break; }
                            }

                            testResult.ErrorMessage = assertMsg;

                            string fileName = Path.GetFileName(eachTest.CodeFilePath);
                            long line = jsonTest.line;
                            testResult.ErrorStackTrace = $"at {fileName} in {eachTest.CodeFilePath}:line {line}{Environment.NewLine}";
                            break;
                        }
                    case "SKIP":
                        {
                            testResult.Outcome = TestOutcome.Skipped;
                            testResult.Duration = new TimeSpan(jsonTest.duration / 100);
                            break;
                        }
                    default:
                        {
                            testResult.Outcome = TestOutcome.None;
                            break;
                        }
                }

                //NOTE: RecordEnd seems superfluous in terms of UI output (because of RecordResult call)
                //      but it seems to slightly increase UI updates.
                _recorder.RecordEnd(eachTest, testResult.Outcome);
                _recorder.RecordResult(testResult);
            }
            _logger.Debug("Test Result Parsing completed!");
            return true;
        }
    }

#pragma warning disable CS0649
    public struct CloveJsonReport
    {
        public CloveJsonResult result;
    }

    public struct CloveJsonResult
    {
        public Dictionary<string, CloveJsonSuite> suites;
    }
    public struct CloveJsonSuite
    {
        public Dictionary<string, CloveJsonTest> tests;
    }

    public struct CloveJsonTest
    {
        public string status;
        public long duration;
        public string assert;
        public string type;
        public string expected;
        public string actual;
        public long line;
    }
#pragma warning restore CS0649
}

