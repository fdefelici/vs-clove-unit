using System.Threading;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace CLoveUnitTestAdapter.Core
{
    public interface IXLogger
    {
        bool DebugEnabled { get; set; }

        void Info(string message);
        void Warn(string message);
        void Erro(string message);
        void Trace(string message);
        void Debug(string message);
    }

    public class NullXLogger : IXLogger
    {
        public bool DebugEnabled { get; set; }
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Erro(string message) { }
        public void Trace(string message) { }
        public void Debug(string message) { }
    }


    public class XLogger : IXLogger
    {
        public static XLogger From(IMessageLogger logger)
        {
            return new XLogger(logger);
        }

        private readonly IMessageLogger _logger;
        private const string LOG_FORMAT = "[CLoveUnitTestAdapter][{0}] {1}"; 

        public bool DebugEnabled { get; set; }

        public XLogger(IMessageLogger logger)
        {
            _logger = logger;
            DebugEnabled = false;
        }

        public void Info(string message)
        {
            Log(TestMessageLevel.Informational, "INFO", message);
        }

        public void Warn(string message)
        {
            Log(TestMessageLevel.Warning, "WARN", message);
        }

        public void Erro(string message)
        {
            Log(TestMessageLevel.Error, "ERRO", message);
        }

        public void Trace(string message)
        {
            Log(TestMessageLevel.Informational, "TRAC", message);
        }

        //TODO: Understand of to make logging only for debugging
        public void Debug(string message)
        {
            if (!DebugEnabled) return;
            Log(TestMessageLevel.Informational, "DEBG", message);
        }


        private void Log(TestMessageLevel level, string levelStr, string message)
        {
            Log(level, string.Format(LOG_FORMAT, levelStr, message));
        }

        private void Log(TestMessageLevel level, string message)
        {   
            _logger.SendMessage(level, message);
        }

        
    }
}
