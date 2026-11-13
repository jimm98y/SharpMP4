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

namespace SharpH26X
{
    /// <summary>
    /// Raised when a bitstream cannot be parsed: it ended earlier than the syntax requires, or it
    /// declares a structure that could not fit into what is left of it.
    /// </summary>
    /// <remarks>
    /// Parsing input from elsewhere is expected to fail sometimes, so failures are reported with a
    /// type the caller can catch. Letting NullReferenceException or OverflowException escape leaves
    /// callers unable to tell a malformed file from a defect in the library.
    /// </remarks>
    public class ItuEndOfStreamException : Exception
    {
        public ItuEndOfStreamException(string message) : base(message)
        {
        }

        public ItuEndOfStreamException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
