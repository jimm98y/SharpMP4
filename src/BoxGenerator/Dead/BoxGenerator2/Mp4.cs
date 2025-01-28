using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SharpMP4
{
    public class Mp4 : IMp4Serializable
    {
        public StreamMarker Padding { get; set; }
        public byte[] PaddingBytes { get; set; }
        public SafeBoxHeader PaddingHeader { get; set; }

        public List<Box> Children { get; set; } = new List<Box>();

        public ulong Read(IsoStream stream, ulong readSize = 0)
        {
            Children.Clear();
            Padding = null;
            PaddingHeader = null;

            SafeBoxHeader header = null;
            Box box = null;
            ulong size = 0;

            try
            {
                while (true)
                {
                    header = null;
                    box = null;

                    header = stream.ReadBoxHeader();
                    ulong boxSize = header.GetBoxSizeInBits();

                    box = stream.ReadBoxContent(header);
                    if(box == null)
                    {
                        // header from 0 bytes
                        this.PaddingHeader = header;
                        if (stream.CanStreamSeek())
                        {
                            this.Padding = new StreamMarker(stream.GetCurrentOffset(), stream.GetStreamLength() - stream.GetCurrentOffset(), stream);
                        }
                        else
                        {
                            StreamMarker marker;
                            stream.ReadPadding(0, ulong.MaxValue, out marker);
                            this.Padding = marker;
                        }
                        break;
                    }
                    
                    ulong calculatedSize = box.CalculateSize();
                    if (boxSize != calculatedSize)
                    {
                        Debug.WriteLine($"Box size mismatch - calculated: {calculatedSize >> 3}, read: {boxSize >> 3}");
                    }

                    Children.Add(box);
                    size += boxSize;
                }
            }
            catch (IsoEndOfStreamException ie)
            {
                if (header == null)
                {
                    Padding = ie.Padding;
                    PaddingBytes = ie.PaddingBytes;
                }
                else
                {
                    PaddingHeader = header;
                    Padding = ie.Padding;
                    PaddingBytes = ie.PaddingBytes;
                }
            }
            catch (EndOfStreamException) 
            {
                // This is to be expected
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }

            return size;
        }

        public ulong Write(IsoStream stream)
        {
            ulong size = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                size += stream.WriteBox(Children[i]);
            }

            if(this.PaddingHeader != null && this.PaddingHeader != null)
            {
                size += this.PaddingHeader.Write(stream);
            }

            if(this.Padding != null)
            {
                size += stream.WritePadding(this.Padding);
            }
            else if(this.PaddingBytes != null)
            {
                size += stream.WritePadding(this.PaddingBytes);
            }

            return size;
        }

        public ulong CalculateSize()
        {
            ulong size = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                size += Children[i].CalculateSize();
            }

            if (this.PaddingHeader != null)
            {
                size += this.PaddingHeader.GetHeaderSizeInBits();
            }

            if (this.Padding != null)
            {
                size += (ulong)(this.Padding.Length << 3);
            }

            return size;
        }
    }
}
