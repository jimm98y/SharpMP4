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
    public bool IsErrorEnabled
    {
        get => true;
        set { }
    }

    public bool IsWarningEnabled
    {
        get => false;
        set { }
    }

    public bool IsInfoEnabled
    {
        get => true;
        set { }
    }

    public bool IsDebugEnabled
    {
        get => true;
        set { }
    }

    public bool IsTraceEnabled
    {
        get => false;
        set { }
    }

    public void LogDebug(string debug)
    {
        Debug.WriteLine(debug);
    }

    public void LogError(string error)
    {
        Debug.WriteLine(error);
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
    public bool IsErrorEnabled { get; set; } = false;

    public bool IsWarningEnabled { get; set; } = true;

    public bool IsInfoEnabled { get; set; } = true;

    public bool IsDebugEnabled { get; set; } = false;

    public bool IsTraceEnabled { get; set; } = false;

    public void LogDebug(string debug)
    {
    }

    public void LogError(string error)
    {
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
