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
    public bool IsErrorEnabled => true;

    public bool IsWarningEnabled => false;

    public bool IsInfoEnabled => true;

    public bool IsDebugEnabled => true;

    public bool IsTraceEnabled => false;

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
    public bool IsErrorEnabled => false;

    public bool IsWarningEnabled => false;

    public bool IsInfoEnabled => true;

    public bool IsDebugEnabled => false;

    public bool IsTraceEnabled => false;

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
