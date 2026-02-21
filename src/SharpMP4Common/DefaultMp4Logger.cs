using System;

namespace SharpMP4.Common
{
    /// <summary>
    /// A configurable MP4 logger that logs to the console. Logging is disabled
    /// by default.
    /// </summary>
    public sealed class DefaultMp4Logger : IMp4Logger
    {
        private bool _loggingEnabled = false;

        public bool LoggingEnabled
        {
            get => _loggingEnabled;
            set => _loggingEnabled = value;
        }

        public bool IsErrorEnabled => _loggingEnabled;

        public bool IsWarningEnabled => _loggingEnabled;

        public bool IsInfoEnabled => _loggingEnabled;

        public bool IsDebugEnabled => _loggingEnabled;

        public bool IsTraceEnabled => _loggingEnabled;

        public void LogDebug(string debug)
        {
            if (LoggingEnabled)
                Console.WriteLine(debug);
        }

        public void LogError(string error)
        {
            if (LoggingEnabled)
                Console.WriteLine(error);
        }

        public void LogInfo(string info)
        {
            if (LoggingEnabled)
                Console.WriteLine(info);
        }

        public void LogTrace(string trace)
        {
            if (LoggingEnabled)
                Console.WriteLine(trace);
        }

        public void LogWarning(string warning)
        {
            if (LoggingEnabled)
                Console.WriteLine(warning);
        }
    }
}
