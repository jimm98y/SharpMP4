using System;

namespace SharpMP4Common
{
    public interface IMp4Logger
    {
        void LogError(string error);
        void LogWarning(string warning);
        void LogInfo(string info);
        void LogDebug(string debug);
        void LogTrace(string trace);

        bool IsErrorEnabled { get; }
        bool IsWarningEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsDebugEnabled { get; }
        bool IsTraceEnabled { get; }
    }

    /// <summary>
    /// Implementation of <see cref="IMp4Logger"/> that does not log
    /// anywhere.
    /// </summary>
    public sealed class NullMp4Logger : IMp4Logger
    {
        public static readonly NullMp4Logger Instance = new();
        
        private NullMp4Logger() { }
        public void LogError(string error) { }
        public void LogWarning(string warning) { }
        public void LogInfo(string info) { }
        public void LogDebug(string debug) { }
        public void LogTrace(string trace) { }
        public bool IsErrorEnabled => false;
        public bool IsWarningEnabled => false;
        public bool IsInfoEnabled => false;
        public bool IsDebugEnabled => false;
        public bool IsTraceEnabled => false;
    }

    /// <summary>
    /// Implementation of <see cref="IMp4Logger"/> that logs to the console.
    /// The error level is logged to the standard error stream, while all other levels are logged to the standard output stream.
    /// </summary>
    public sealed class ConsoleMp4Logger : IMp4Logger
    {
        public static readonly ConsoleMp4Logger Instance = new();

        public void LogError(string error) => Console.Error.WriteLine($"ERROR: {error}");
        public void LogWarning(string warning) => Console.WriteLine($"WARNING: {warning}");
        public void LogInfo(string info) => Console.WriteLine($"INFO: {info}");
        public void LogDebug(string debug) => Console.WriteLine($"DEBUG: {debug}");
        public void LogTrace(string trace) => Console.WriteLine($"TRACE: {trace}");
        public bool IsErrorEnabled => true;
        public bool IsWarningEnabled => true;
        public bool IsInfoEnabled => true;
        public bool IsDebugEnabled => true;
        public bool IsTraceEnabled => true;
    }

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
