using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMP4
{
    public class Mp4
    {
        private IsoStream _stream;
        public IsoStream Stream { get { return _stream; } }

        public Mp4(Stream stream)
        {
            this._stream = new IsoStream(stream);
        }

        public static Mp4 Create(Stream stream)
        {
            Mp4 mp4 = new Mp4(stream);
            return mp4;
        }

        public async Task<Mp4BoxHeader> ReadNextBoxHeaderAsync()
        {
            BoxHeader header = new BoxHeader();
            long headerOffset = this._stream.GetCurrentOffset();
            ulong headerSize = await header.ReadAsync(this._stream, 0);
            return new Mp4BoxHeader(header, headerOffset, headerSize);
        }

        public async Task<Box> ReadNextBoxAsync(Mp4BoxHeader header)
        {
            var box = BoxFactory.CreateBox(IsoStream.ToFourCC(header.Header.Type));
            ulong size = await box.ReadAsync(this._stream, header.BoxSize);
            
            if (size != header.BoxSize)
                throw new Exception("Box not fully read!");
            return box;
        }
    }

    public class Mp4BoxHeader
    {
        public BoxHeader Header { get; set; }
        public long HeaderOffset { get; set; }
        public ulong HeaderSize { get; set; }
        public ulong BoxSize { get; set; }

        public Mp4BoxHeader(BoxHeader header, long headerOffset, ulong headerSize)
        {
            this.Header = header;
            this.HeaderOffset = headerOffset;
            this.HeaderSize = headerSize;
            this.BoxSize = header.GetBoxSizeInBits() - headerSize;
        }
    }
}
