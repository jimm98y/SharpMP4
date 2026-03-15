using SharpMP4.Common;
using System;
using System.Diagnostics;

namespace SampleVideoParser;

/// <summary>
/// Info messages write to the console. Debug and error messages write
/// to <see cref="Debug.WriteLine"/>. Other messages are ignored.
/// </summary>
internal class IsoBmffLogger : IMp4Logger
{
    private FileLogger logger;

    public IsoBmffLogger(FileLogger logger)
    {
        this.logger = logger;
    }

    public bool IsErrorEnabled { get; set; } = true;

    public bool IsWarningEnabled { get; set; } = true;

    public bool IsInfoEnabled { get; set; } = true;

    public bool IsDebugEnabled { get; set; } = true;

    public bool IsTraceEnabled { get; set; } = true;

    public void LogDebug(string debug)
    {
        Debug.WriteLine(debug);
    }

    public void LogError(string error)
    {
        Console.WriteLine(error);
        this.logger.Log(error + "\r\n");
    }

    public void LogInfo(string info)
    {
        Console.WriteLine(info);
    }

    public void LogTrace(string trace)
    {
    }

    public void LogWarning(string warning)
    {
    }
}

/// <summary>
/// Info messages write to the debug output. Other messages are ignored.
/// </summary>
internal class H26XLogger : IMp4Logger
{
    private FileLogger logger;

    public H26XLogger(FileLogger logger)
    {
        this.logger = logger;
    }

    public bool IsErrorEnabled { get; set; } = true;

    public bool IsWarningEnabled { get; set; } = true;

    public bool IsInfoEnabled { get; set; } = true;

    public bool IsDebugEnabled { get; set; } = true;

    public bool IsTraceEnabled { get; set; } = true;

    public void LogDebug(string debug)
    {
    }

    public void LogError(string error)
    {
        this.logger.Log(error + "\r\n");
    }

    public void LogInfo(string info)
    {
        Debug.WriteLine(info);
    }

    public void LogTrace(string trace)
    {
    }

    public void LogWarning(string warning)
    {
    }
}
