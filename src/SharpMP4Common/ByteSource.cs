using System;
using System.IO;

namespace SharpMP4.Common
{
    /// <summary>
    /// The bytes a bitstream reads and writes: of an array, or of a stream. Read ahead on and gone back from without
    /// seeking: the bytes taken from the source while a peek is on are kept, to be read again once it ends, so a stream is
    /// read ahead on as it is - one that cannot seek too - and nothing is copied. What <see cref="Bitstream"/> and
    /// <see cref="RbspBitstream"/> share; sealed, so that their byte reads are calls that can be inlined.
    /// </summary>
    internal sealed class ByteSource
    {
        private Stream _stream;

        // The array read from, where there is one rather than a stream.
        private byte[] _source;
        private int _sourcePosition;
        private int _sourceEnd;

        // The bytes read ahead: _replay[_replayPosition.._replayLength] are still to be read again.
        private byte[] _replay;
        private int _replayPosition;
        private int _replayLength;
        private int _peekDepth;

        public ByteSource(Stream stream)
        {
            _stream = stream;
        }

        public ByteSource(byte[] buffer, int offset, int length)
        {
            Reset(buffer, offset, length);
        }

        /// <summary>Starts over on other bytes, in an array: the bytes read ahead let go.</summary>
        public void Reset(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || length < 0 || offset + length > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            _stream = null;
            _source = buffer;
            _sourcePosition = offset;
            _sourceEnd = offset + length;
            _replayPosition = 0;
            _replayLength = 0;
            _peekDepth = 0;
        }

        public Stream Stream
        {
            get => _stream;
            set => _stream = value;
        }

        /// <summary>
        /// How many bytes are still to be read: of the array, or of the stream where it can tell, and those read ahead to
        /// be read again. Throws <see cref="NotSupportedException"/> for a stream that cannot tell.
        /// </summary>
        public long RemainingBytes => (_source != null ? _sourceEnd - _sourcePosition : _stream.Length - _stream.Position) + PendingBytes;

        /// <summary>The bytes read from the source ahead of the reads, to be read again: the source is that far ahead of them.</summary>
        public int PendingBytes => _replayLength - _replayPosition;

        /// <summary>Begins reading ahead: where the reads are, to go back to at its end. Peeks can be nested.</summary>
        public int BeginPeek()
        {
            _peekDepth++;
            return _replayPosition;
        }

        /// <summary>Ends reading ahead, going back to where the reads were when it began.</summary>
        public void EndPeek(int replayPosition)
        {
            _replayPosition = replayPosition;
            _peekDepth--;
        }

        /// <summary>Ends reading ahead where it has got to: what was read is read. The bytes kept for an outer peek stay kept.</summary>
        public void AcceptPeek() => _peekDepth--;

        /// <summary>The next byte: of those read ahead first, then of the source, kept where a peek is on. -1 at the end.</summary>
        public int ReadByte()
        {
            if (_replayPosition < _replayLength)
                return _replay[_replayPosition++];

            if (_peekDepth == 0)
            {
                // nothing left to read again: the buffer starts over
                _replayPosition = _replayLength = 0;
                return ReadSourceByte();
            }

            int b = ReadSourceByte();
            if (b >= 0)
            {
                Keep(1);
                _replay[_replayLength++] = (byte)b;
                _replayPosition = _replayLength;
            }
            return b;
        }

        /// <summary>
        /// Up to <paramref name="count"/> bytes: of those read ahead first, then of the source, kept where a peek is on.
        /// How many there were; 0 at the end.
        /// </summary>
        public int Read(byte[] buffer, int offset, int count)
        {
            int pending = _replayLength - _replayPosition;
            if (pending > 0)
            {
                int taken = Math.Min(pending, count);
                Buffer.BlockCopy(_replay, _replayPosition, buffer, offset, taken);
                _replayPosition += taken;
                return taken;
            }

            if (_peekDepth == 0)
            {
                _replayPosition = _replayLength = 0;
                return ReadSource(buffer, offset, count);
            }

            int read = ReadSource(buffer, offset, count);
            if (read > 0)
            {
                Keep(read);
                Buffer.BlockCopy(buffer, offset, _replay, _replayLength, read);
                _replayLength += read;
                _replayPosition = _replayLength;
            }
            return read;
        }

        /// <summary>
        /// Moves on over so many bytes, unread: of those read ahead first, then of the source - by seeking where it can and
        /// no peek is on, else by reading them, kept where a peek is on.
        /// </summary>
        public void Skip(long count)
        {
            long left = count;
            int pending = _replayLength - _replayPosition;
            if (pending > 0)
            {
                int taken = (int)Math.Min(pending, left);
                _replayPosition += taken;
                left -= taken;
            }

            if (left > 0 && _peekDepth == 0)
            {
                _replayPosition = _replayLength = 0;
                if (_source != null)
                {
                    int taken = (int)Math.Min(left, _sourceEnd - _sourcePosition);
                    _sourcePosition += taken;
                    if (taken < left)
                        throw new EndOfStreamException();
                    return;
                }
                if (_stream.CanSeek)
                {
                    _stream.Seek(left, SeekOrigin.Current);
                    return;
                }
            }

            var buffer = left > 0 ? new byte[(int)Math.Min(left, 1 << 16)] : null;
            while (left > 0)
            {
                int read = Read(buffer, 0, (int)Math.Min(left, buffer.Length));
                if (read <= 0)
                    throw new EndOfStreamException();
                left -= read;
            }
        }

        public void WriteByte(byte value) => _stream.WriteByte(value);

        public void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);

        private int ReadSourceByte() => _source != null
            ? (_sourcePosition < _sourceEnd ? _source[_sourcePosition++] : -1)
            : _stream.ReadByte();

        private int ReadSource(byte[] buffer, int offset, int count)
        {
            if (_source == null)
                return _stream.Read(buffer, offset, count);
            int taken = Math.Min(count, _sourceEnd - _sourcePosition);
            Buffer.BlockCopy(_source, _sourcePosition, buffer, offset, taken);
            _sourcePosition += taken;
            return taken;
        }

        private void Keep(int count)
        {
            if (_replay == null)
                _replay = new byte[Math.Max(64, count)];
            else if (_replayLength + count > _replay.Length)
                Array.Resize(ref _replay, Math.Max(_replay.Length * 2, _replayLength + count));
        }
    }
}
