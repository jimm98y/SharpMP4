using SharpMP4.Common;

namespace SharpMP4.Bench
{
    /// <summary>
    /// A logger that reports a level as enabled but throws the message away. DefaultMp4Logger
    /// writes to the console, which would be what was measured rather than the formatting; this
    /// isolates the cost of building a message per syntax element.
    /// </summary>
    internal sealed class DiscardingLogger : IMp4Logger
    {
        public DiscardingLogger(bool enabled)
        {
            IsErrorEnabled = enabled;
            IsWarningEnabled = enabled;
            IsInfoEnabled = enabled;
            IsDebugEnabled = enabled;
            IsTraceEnabled = enabled;
        }

        public bool IsErrorEnabled { get; set; }
        public bool IsWarningEnabled { get; set; }
        public bool IsInfoEnabled { get; set; }
        public bool IsDebugEnabled { get; set; }
        public bool IsTraceEnabled { get; set; }

        /// <summary>Counted rather than written, so the call cannot be optimised away.</summary>
        public long Messages { get; private set; }

        public void LogError(string error) => Messages++;
        public void LogWarning(string warning) => Messages++;
        public void LogInfo(string info) => Messages++;
        public void LogDebug(string debug) => Messages++;
        public void LogTrace(string trace) => Messages++;
    }
}
