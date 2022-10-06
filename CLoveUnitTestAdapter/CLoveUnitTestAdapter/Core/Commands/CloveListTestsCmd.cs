using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;

namespace CLoveUnitTestAdapter.Core
{
    public class CloveListTestsCmd : ACloveCommand
    {
        private readonly Action<TestCase> _onTestFound;
        public int TestCount { get; internal set; }

        public CloveListTestsCmd(XTestProcessMgr procMgr, string binaryPath, XLogger logger, Action<TestCase> onTestFound)
            : base(procMgr, binaryPath, logger)  
        {
            _onTestFound = onTestFound;
        }

        protected override string GetArgs()
        {
            //TODO: Change to a file report?!
            return "--list-tests --report csv --output stdout";
        }

        protected override bool BeforeExecute()
        {
            TestCount = 0;
            return true;
        }

        protected override bool AfterExecute(XProcResult execResult)
        {
            if (execResult.IsError)
            {
                _logger.Erro("Test discovery failed!");
                _logger.Erro(StdOut);
                return false;
            }

            _logger.Debug("Tests Discovery completed!");

            string[] lines = StdOut.Split('\n');
            //Skipping following lines:
            // - First line (i=0) is the CVS Header
            // - Last line (i=Length-1) is empty after last \n
            for (int i = 1; i < lines.Length - 1; i++)
            {
                string[] line = lines[i].Split(',');
                if (line.Length != 4)
                {
                    _logger.Warn($"Test line with wrong format skipped: \"{lines[i]}\"");
                    continue;
                }
                string suiteName = line[0];
                string testName = line[1];
                string testFilePath = line[2];
                string lineNumStr = line[3];

                if (!int.TryParse(lineNumStr, out int lineNum))
                {
                    _logger.Warn($"Test line with wrong format skipped: \"{line}\"");
                    continue;
                }

                TestCase test = new TestCase();
                test.FullyQualifiedName = suiteName + "." + testName;
                test.DisplayName = testName;
                test.Source = _binaryPath;
                test.CodeFilePath = testFilePath;
                test.LineNumber = lineNum;
                test.ExecutorUri = new Uri(CloveConfig.TestExecutorUri);

                TestCount++;
                _onTestFound.Invoke(test);
            }

            _logger.Debug("Tests Parsing completed!");
            return true;
        }

    }
}
