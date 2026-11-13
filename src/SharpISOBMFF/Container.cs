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
using System.Collections.Generic;
using System.IO;

namespace SharpISOBMFF
{
    public class Container : IMp4Serializable
    {
        /// <summary>Of a class of no definition, none.</summary>
        public virtual string Syntax { get { return null; } }

        public virtual string DisplayName { get { return nameof(Container); } }
        protected IMp4Serializable parent = null;
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }
        public StreamMarker Padding { get; set; }
        public byte[] PaddingBytes { get; set; }
        public List<Box> Children { get; set; } = new List<Box>();
        public IMp4Logger Logger { get; set; } = DefaultMp4Logger.Instance;
        public bool IsTruncated { get; set; }

        public Container()
        {
        }

        public Container(IMp4Logger logger)
        {
            this.Logger = logger;
        }
        
        public ulong ReadSingleBox(IsoStream stream)
        {
            Box box = null;
            ulong size = 0;

            // where the stream ends here, it ends where a box does, as a file does; anywhere else, the file is cut short
            bool atEnd = stream.Peek(1).Length == 0;

            try
            {
                box = null;
                size += stream.ReadBox(size, null, out box, "");

                if (box != null)
                {
                    Children.Add(box);
                }
            }
            catch (IsoEndOfStreamException ie)
            {
                Padding = ie.Padding;
                PaddingBytes = ie.PaddingBytes;
            }
            catch (EndOfStreamException)
            {
                // This is to be expected at the end of the file; within a box, the file is cut short
                if (!atEnd)
                    IsTruncated = true;
            }
            catch (Exception ex)
            {
                this.Logger.LogDebug($"Error: {ex.Message}");
                throw;
            }

            if (stream.HasReadPastEnd)
                IsTruncated = true;

            return size;
        }

        public ulong Read(IsoStream stream, ulong readSize = 0)
        {
            ulong size = 0;

            try
            {
                while (readSize == 0 || size < readSize)
                {
                    ulong boxSize = ReadSingleBox(stream);
                    if (boxSize == 0)
                    {
                        break;
                    }
                    size += boxSize;
                }
            }
            catch(EndOfStreamException)
            {
                // This is to be expected
            }

            return size;
        }

        public ulong Write(IsoStream stream)
        {
            ulong size = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                size += stream.WriteBox(Children[i], "");
            }

            if(this.Padding != null)
            {
                size += stream.WriteUInt8ArrayTillEnd(this.Padding, "");
            }
            else if(this.PaddingBytes != null)
            {
                size += stream.WriteUInt8ArrayTillEnd(this.PaddingBytes, "");
            }

            return size;
        }

        public ulong CalculateSize()
        {
            ulong size = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                size += IsoStream.CalculateBoxSize(Children[i]);
            }

            if (this.Padding != null)
            {
                size += (ulong)(this.Padding.Length << 3);
            }

            return size;
        }

        /// <summary>The file's boxes, a line each of their fields, as <see cref="BoxStringBuilder"/> describes them.</summary>
        public override string ToString() { return BoxStringBuilder.Format(this); }

        public ulong Trace(BoxTracer tracer)
        {
            ulong size = 0;
            foreach (var box in Children)
            {
                size += tracer.Append(box);
            }

            // what is after the last box, of a stream that cannot seek: its bytes (of one that can, its Padding)
            if (this.Padding == null && this.PaddingBytes != null)
            {
                size += tracer.Field("padding", (ulong)this.PaddingBytes.Length << 3, this.PaddingBytes);
            }

            return size;
        }
    }
}
