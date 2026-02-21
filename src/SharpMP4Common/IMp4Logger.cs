namespace SharpMP4.Common
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
}
