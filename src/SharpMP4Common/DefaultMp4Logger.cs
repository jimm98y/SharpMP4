using System;

namespace SharpMP4.Common
{
    /// <summary>
    /// A configurable MP4 logger that logs to the console. Logging is disabled
    /// by default.
    /// </summary>
    public sealed class DefaultMp4Logger : IMp4Logger
    {
        // The level properties report the master switch too, so callers can ask whether a
        // message would be logged and skip building it. Parsers consult these on every syntax
        // element, and formatting a message only for the logger to drop it dominated the cost of
        // reading a bitstream.
        private bool _loggingEnabled = false;

        private bool _isErrorEnabled = true;
        private bool _isWarningEnabled = true;
        private bool _isInfoEnabled = true;
        private bool _isDebugEnabled = true;
        private bool _isTraceEnabled = true;

        public bool IsLoggingEnabled
        {
            get => _loggingEnabled;
            set => _loggingEnabled = value;
        }

        public bool IsErrorEnabled
        {
            get => _loggingEnabled && _isErrorEnabled;
            set => _isErrorEnabled = value;
        }

        public bool IsWarningEnabled
        {
            get => _loggingEnabled && _isWarningEnabled;
            set => _isWarningEnabled = value;
        }

        public bool IsInfoEnabled
        {
            get => _loggingEnabled && _isInfoEnabled;
            set => _isInfoEnabled = value;
        }

        public bool IsDebugEnabled
        {
            get => _loggingEnabled && _isDebugEnabled;
            set => _isDebugEnabled = value;
        }

        public bool IsTraceEnabled
        {
            get => _loggingEnabled && _isTraceEnabled;
            set => _isTraceEnabled = value;
        }

        public void LogDebug(string debug)
        {
            if (IsLoggingEnabled && IsDebugEnabled)
                Console.WriteLine(debug);
        }

        public void LogError(string error)
        {
            if (IsLoggingEnabled && IsErrorEnabled)
                Console.WriteLine(error);
        }

        public void LogInfo(string info)
        {
            if (IsLoggingEnabled && IsInfoEnabled)
                Console.WriteLine(info);
        }

        public void LogTrace(string trace)
        {
            if (IsLoggingEnabled && IsTraceEnabled)
                Console.WriteLine(trace);
        }

        public void LogWarning(string warning)
        {
            if (IsLoggingEnabled && IsWarningEnabled)
                Console.WriteLine(warning);
        }
    }
}
