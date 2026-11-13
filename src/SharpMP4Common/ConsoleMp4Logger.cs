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
