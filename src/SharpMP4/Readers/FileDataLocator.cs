using SharpISOBMFF;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Readers
{
    /// <summary>
    /// Where the bytes at an offset of a file are, as a chunk offset, a run's data offset or an item's extent gives it: in
    /// the data of the file's 'mdat' boxes. Read from a stream that seeks, the data of each is the file itself, and the offset
    /// is where it is in it. Read from one that cannot, the data of each was copied into temporary storage as it was read, and
    /// the offset is found in the copy of the 'mdat' it falls in.
    /// </summary>
    internal sealed class FileDataLocator
    {
        private readonly StreamMarker[] _data;

        public FileDataLocator(IEnumerable<StreamMarker> data)
        {
            _data = data.Where(x => x?.Stream != null).ToArray();
        }

        /// <summary>The data of the 'mdat' boxes of a file's top level.</summary>
        public static FileDataLocator Of(Container container, params StreamMarker[] more) =>
            new FileDataLocator(container.Children.OfType<MediaDataBox>().Select(x => x.Data).Concat(more));

        /// <summary>
        /// The stream so many bytes at an offset of the file are read from, and where in it they are. False where they are in
        /// none of the data read: past what a stream that cannot seek was copied of.
        /// </summary>
        public bool TryLocate(long offset, long length, out IsoStream stream, out long position) =>
            TryLocate(offset, length, out stream, out position, out _);

        /// <summary>
        /// As <see cref="TryLocate(long, long, out IsoStream, out long)"/>, and how many bytes there are from there to the
        /// end of the data they are in: of the 'mdat', or of the file.
        /// </summary>
        public bool TryLocate(long offset, long length, out IsoStream stream, out long position, out long available)
        {
            foreach (var data in _data)
            {
                if (data.SourcePosition >= 0 && offset >= data.SourcePosition && offset + length <= data.SourcePosition + data.Length)
                {
                    stream = data.Stream;
                    position = data.Position + (offset - data.SourcePosition);
                    available = data.SourcePosition + data.Length - offset;
                    return true;
                }
            }

            // a stream that seeks is the file, which has the bytes wherever they are: in a box other than 'mdat' too, or in
            // one shorter than its samples, where the read says the file ends
            foreach (var data in _data)
            {
                if (!data.IsCopy)
                {
                    stream = data.Stream;
                    position = offset;
                    available = stream.GetStreamLength() - offset;
                    return true;
                }
            }

            stream = null;
            position = -1;
            available = 0;
            return false;
        }
    }
}
