using SharpISOBMFF;
using System;
using System.IO;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// A sample entry as a file has it - its type, its fields and every box in it - kept as its bytes, of a track that writes
    /// back the entry it was read with. The bytes are written once, as the track is made: a box the library does not know
    /// reads its bytes from the stream it was read from only as it is written, so while that stream is open. A writer is
    /// given a box of its own each time, read back from them - it puts the entry in its tree, and of a protected track
    /// changes it - from the one stream the copy keeps over them: a box the library does not know points into that stream
    /// rather than holding its bytes, so a box handed out is good until the copy is disposed, with its track.
    /// </summary>
    internal sealed class SampleEntryCopy : IDisposable
    {
        private readonly byte[] _bytes;
        private IsoStream _stream;

        private SampleEntryCopy(byte[] bytes)
        {
            _bytes = bytes;
            _stream = new IsoStream(new MemoryStream(_bytes, writable: false));
        }

        /// <summary>The bytes of an entry, or null of an entry that does not write: such a track writes the entry itself.</summary>
        public static SampleEntryCopy Of(Box entry)
        {
            if (entry == null)
                return null;

            // writing a box made in code gives it the header of its size then: left as it was, so the box can still change
            bool headerless = entry.Header == null;
            try
            {
                using var memory = new MemoryStream();
                using (var stream = new IsoStream(memory))
                    stream.WriteBox(entry, "");
                return new SampleEntryCopy(memory.ToArray());
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                return null;
            }
            finally
            {
                if (headerless)
                    entry.Header = null;
            }
        }

        /// <summary>A copy of the same bytes with a stream of its own, of a track's clone, which is disposed on its own.</summary>
        public SampleEntryCopy Clone() => new SampleEntryCopy(_bytes);

        /// <summary>A box of the entry, of its own: good until the copy is disposed.</summary>
        public Box Create()
        {
            if (_stream == null)
                throw new ObjectDisposedException(nameof(SampleEntryCopy));

            _stream.SeekFromBeginning(0);
            _stream.ReadBox(0, new SampleDescriptionBox(), out Box box, "");
            return box;
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _stream = null;
        }
    }
}
