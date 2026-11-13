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

namespace SharpH26X
{
    /// <summary>
    /// What an <see cref="ItuStream"/> reads or writes, as it does: each syntax element of where it ends, of its size and
    /// value, and each class around them. Where is in bits of the NAL unit's bytes as they are, its emulation prevention
    /// bytes counted - an element of an emulation prevention byte in it begins 8 bits before its end and its size.
    /// </summary>
    public interface IItuTracer
    {
        /// <summary>A class begins, at the position given.</summary>
        void OnBeginClass(string name, IItuSerializable value, ulong position);

        /// <summary>A class that began ends, at the position given.</summary>
        void OnEndClass(string name, IItuSerializable value, ulong position);

        /// <summary>A syntax element, ending at the position given, of the bits given, and its value.</summary>
        void OnField(string name, ulong end, ulong bits, object value);
    }
}
