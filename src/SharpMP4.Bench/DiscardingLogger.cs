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
