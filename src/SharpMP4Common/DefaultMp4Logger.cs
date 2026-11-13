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
    /// A configurable MP4 logger that logs to the console. Logging is disabled
    /// by default.
    /// </summary>
    public sealed class DefaultMp4Logger : IMp4Logger
    {
        /// <summary>
        /// The one logger everything uses where it is given none: a logger is consulted on every syntax element, so
        /// one made for each reader, track and stream - a stream for each NAL unit - is so much garbage. Settings made
        /// on it hold for all of them.
        /// </summary>
        public static DefaultMp4Logger Instance { get; } = new DefaultMp4Logger();

        // The level properties report the master switch too, so callers can ask whether a
        // message would be logged and skip building it. Parsers consult these on every syntax
        // element, and formatting a message only for the logger to drop it dominated the cost of
        // reading a bitstream.
        private bool _loggingEnabled = false;

        private bool _isErrorEnabled = true;
        private bool _isWarningEnabled = true;
        private bool _isInfoEnabled = true;
        private bool _isDebugEnabled = true;
        private bool _isTraceEnabled = true;

        public bool IsLoggingEnabled
        {
            get => _loggingEnabled;
            set => _loggingEnabled = value;
        }

        public bool IsErrorEnabled
        {
            get => _loggingEnabled && _isErrorEnabled;
            set => _isErrorEnabled = value;
        }

        public bool IsWarningEnabled
        {
            get => _loggingEnabled && _isWarningEnabled;
            set => _isWarningEnabled = value;
        }

        public bool IsInfoEnabled
        {
            get => _loggingEnabled && _isInfoEnabled;
            set => _isInfoEnabled = value;
        }

        public bool IsDebugEnabled
        {
            get => _loggingEnabled && _isDebugEnabled;
            set => _isDebugEnabled = value;
        }

        public bool IsTraceEnabled
        {
            get => _loggingEnabled && _isTraceEnabled;
            set => _isTraceEnabled = value;
        }

        public void LogDebug(string debug)
        {
            if (IsLoggingEnabled && IsDebugEnabled)
                Console.WriteLine(debug);
        }

        public void LogError(string error)
        {
            if (IsLoggingEnabled && IsErrorEnabled)
                Console.WriteLine(error);
        }

        public void LogInfo(string info)
        {
            if (IsLoggingEnabled && IsInfoEnabled)
                Console.WriteLine(info);
        }

        public void LogTrace(string trace)
        {
            if (IsLoggingEnabled && IsTraceEnabled)
                Console.WriteLine(trace);
        }

        public void LogWarning(string warning)
        {
            if (IsLoggingEnabled && IsWarningEnabled)
                Console.WriteLine(warning);
        }
    }
}
