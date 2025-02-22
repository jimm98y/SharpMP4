using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMP4
{
    public class Mp4 : IMp4Serializable
    {
        public virtual string DisplayName { get { return nameof(Mp4); } }
        protected IMp4Serializable parent = null;
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }
        public StreamMarker Padding { get; set; }
        public byte[] PaddingBytes { get; set; }
        public List<Box> Children { get; set; } = new List<Box>();

        public ulong Read(IsoStream stream, ulong readSize = 0)
        {
            Children.Clear();
            Padding = null;

            Box box = null;
            ulong size = 0;

            try
            {
                while (true)
                {
                    box = null;
                    size += stream.ReadBox(out box);
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
                // This is to be expected
            }
            catch (Exception ex)
            {
                Log.Debug($"Error: {ex.Message}");
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

            if(this.Padding != null)
            {
                size += stream.WriteUInt8ArrayTillEnd(this.Padding);
            }
            else if(this.PaddingBytes != null)
            {
                size += stream.WriteUInt8ArrayTillEnd(this.PaddingBytes);
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

            if (this.Padding != null)
            {
                size += (ulong)(this.Padding.Length << 3);
            }

            return size;
        }
    }
}
