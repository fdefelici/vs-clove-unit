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
        
        //TODO: To differenziate log instead of using UUID just use the binary name....
        public static XLogger From(IMessageLogger logger)
        {
            return new XLogger(logger);
        }

        private IMessageLogger _logger;
        private const string LOG_FORMAT = "[CLoveUnitTestAdapter][{0}] {1}"; //TODO: Put in CloveConfig?

        public bool DebugEnabled { get; set; }

        public XLogger(IMessageLogger logger)
        {
            _logger = logger;
            DebugEnabled = false;
        }

        public void Info(string message)
        {
            Log(TestMessageLevel.Informational, string.Format(LOG_FORMAT, "INFO", message));
        }

        public void Warn(string message)
        {
            Log(TestMessageLevel.Warning, string.Format(LOG_FORMAT, "WARN", message));
        }

        public void Erro(string message)
        {
            Log(TestMessageLevel.Error, string.Format(LOG_FORMAT, "ERRO", message));
        }

        public void Trace(string message)
        {
            Log(TestMessageLevel.Informational, string.Format(LOG_FORMAT, "TRAC", message));
        }

        //TODO: Understand of to make logging only for debugging
        public void Debug(string message)
        {
            if (!DebugEnabled) return;
            Log(TestMessageLevel.Informational, string.Format(LOG_FORMAT, "DEBG", message));
        }

        private void Log(TestMessageLevel level, string message)
        {   
            _logger.SendMessage(level, message);
        }

        
    }
}
