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

namespace SharpAVX
{
    /// <summary>What reads itself from an AomStream: a unit of so many bytes.</summary>
    public interface IAomSerializable
    {
        void Read(AomStream stream, int size);
    }

    /// <summary>The state a codec's syntax is read into, unit by unit: an OBU of AV1 or AV2, a frame of VP9.</summary>
    public interface IAomContext : IAomSerializable
    {
    }

    /// <summary>The context of a stream of OBUs (AV1, AV2).</summary>
    public interface IAomObuContext : IAomContext
    {
        int SelectedOperatingPoint { get; set; }
        int ObuSizeLen { get; }
        byte[] LastObuFrameHeader { get; set; }
    }
}
