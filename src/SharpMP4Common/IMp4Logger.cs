namespace SharpMP4.Common
{
    public interface IMp4Logger
    {
        void LogError(string error);
        void LogWarning(string warning);
        void LogInfo(string info);
        void LogDebug(string debug);
        void LogTrace(string trace);

        bool IsErrorEnabled { get; set; }
        bool IsWarningEnabled { get; set; }
        bool IsInfoEnabled { get; set; }
        bool IsDebugEnabled { get; set; }
        bool IsTraceEnabled { get; set; }
    }
}
