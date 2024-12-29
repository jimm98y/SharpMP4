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

        public async Task<BoxHeader> ReadNextBoxHeader()
        {
            BoxHeader header = new BoxHeader();
            ulong size = await header.ReadAsync(this._stream);
            return header;
        }
    }
}
