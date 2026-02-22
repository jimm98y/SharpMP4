using System;

namespace SharpMP4.Common
{
    /// <summary>
    /// Implementation of <see cref="IMp4Logger"/> that logs to the console.
    /// The error level is logged to the standard error stream, while all other levels are logged to the standard output stream.
    /// </summary>
    public sealed class ConsoleMp4Logger : IMp4Logger
    {
        public static readonly ConsoleMp4Logger Instance = new ConsoleMp4Logger();

        public void LogError(string error) => Console.Error.WriteLine($"ERROR: {error}");
        public void LogWarning(string warning) => Console.WriteLine($"WARNING: {warning}");
        public void LogInfo(string info) => Console.WriteLine($"INFO: {info}");
        public void LogDebug(string debug) => Console.WriteLine($"DEBUG: {debug}");
        public void LogTrace(string trace) => Console.WriteLine($"TRACE: {trace}");

        public bool IsErrorEnabled { get; set; } = true;

        public bool IsWarningEnabled { get; set; } = true;

        public bool IsInfoEnabled { get; set; } = true;

        public bool IsDebugEnabled { get; set; } = true;

        public bool IsTraceEnabled { get; set; } = true;
    }
}
