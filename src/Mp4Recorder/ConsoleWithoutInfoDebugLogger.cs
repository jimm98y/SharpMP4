using SharpMP4.Common;

namespace Mp4Recorder;

/// <summary>
/// A console logger that does not log info/debug.
/// </summary>
internal class ConsoleWithoutInfoDebugLogger : IMp4Logger
{
    public static readonly ConsoleWithoutInfoDebugLogger Instance = new();

    public bool IsErrorEnabled => true;

    public bool IsWarningEnabled => true;

    public bool IsInfoEnabled => false;

    public bool IsDebugEnabled => false;

    public bool IsTraceEnabled => true;

    public void LogDebug(string debug)
    {
    }

    public void LogError(string error)
    {
        ConsoleMp4Logger.Instance.LogError(error);
    }

    public void LogInfo(string info)
    {
    }

    public void LogTrace(string trace)
    {
        ConsoleMp4Logger.Instance.LogTrace(trace);
    }

    public void LogWarning(string warning)
    {
        ConsoleMp4Logger.Instance.LogWarning(warning);
    }
}
