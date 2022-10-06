using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLoveUnitTestAdapter.Extension;

namespace CLoveUnitTestAdapter.Core
{
    public class CloveVersionCommand : ACloveCommand
    {
        public XVersion Result { get; internal set; }

        public CloveVersionCommand(XTestProcessMgr procMgr, string binaryPath, XLogger logger)
            : base(procMgr, binaryPath, logger)
        {
            Result = null;
        }

        protected override string GetArgs()
        {
            return "--version";
        }

        protected override bool BeforeExecute()
        {
            return true;
        }

        protected override bool AfterExecute(XProcResult execResult)
        {
            if (execResult.IsError)
            {
                _logger.Erro("Failed checking clove-unit version!");
                _logger.Erro(StdOut);
                return false;
            }

            string versionStr = StdOut;
            _logger.Debug($"clove-unit version found: {versionStr}");


            XVersion versionFound = XVersion.FromSemVerString(versionStr);
            if (versionFound == null)
            {
                _logger.Erro("Invalid clove-unit version!");
                return false;
            }

            Result = versionFound;
            return true;
        }
    }
}
