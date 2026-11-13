// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
