using System;
using System.IO;

namespace SharpMP4.Common
{
    /// <summary>
    /// Bits read from, or written to, a stream, most significant first. Read ahead on and gone back from without seeking
    /// (<see cref="BeginPeek"/>): the bytes taken while peeking are kept to be read again, as <see cref="RbspBitstream"/>'s
    /// are - both read their bytes through a <see cref="ByteSource"/>.
    /// </summary>
    public class Bitstream
    {
        private long _bitsPosition;
        private long _currentBytePosition = -1;
        private byte _currentByte;

        private readonly ByteSource _bytes;

        public Bitstream(Stream stream)
        {
            _bytes = new ByteSource(stream);
        }

        public Stream BaseStream
        {
            get => _bytes.Stream;
        }

        public long BitsPosition
        {
            get => _bitsPosition;
            set => _bitsPosition = value;
        }

        public byte CurrentByte
        {
            get => _currentByte;
            set => _currentByte = value;
        }

        #region Reading ahead

        /// <summary>Where the bits were when a peek began, to go back to at its end.</summary>
        public readonly struct PeekState
        {
            internal PeekState(long bitsPosition, long currentBytePosition, byte currentByte, int replayPosition)
            {
                BitsPosition = bitsPosition;
                CurrentBytePosition = currentBytePosition;
                CurrentByte = currentByte;
                ReplayPosition = replayPosition;
            }

            internal long BitsPosition { get; }
            internal long CurrentBytePosition { get; }
            internal byte CurrentByte { get; }
            internal int ReplayPosition { get; }
        }

        /// <summary>
        /// Begins reading ahead: what is read until <see cref="EndPeek"/> is read again after it. Peeks can be nested.
        /// </summary>
        public PeekState BeginPeek() => new PeekState(_bitsPosition, _currentBytePosition, _currentByte, _bytes.BeginPeek());

        /// <summary>Ends reading ahead, going back to where the bits were when <paramref name="state"/> was taken.</summary>
        public void EndPeek(PeekState state)
        {
            _bitsPosition = state.BitsPosition;
            _currentBytePosition = state.CurrentBytePosition;
            _currentByte = state.CurrentByte;
            _bytes.EndPeek(state.ReplayPosition);
        }

        /// <summary>
        /// Ends reading ahead where it has got to, not going back: what was read is read. The bytes kept for an outer peek
        /// stay kept for it.
        /// </summary>
        public void AcceptPeek(PeekState state) => _bytes.AcceptPeek();

        /// <summary>The bytes read from the stream ahead of the bits, to be read again: the stream is that far ahead of them.</summary>
        public int PendingBytes => _bytes.PendingBytes;

        #endregion

        #region Bits

        public int ReadBit()
        {
            long bytePos = _bitsPosition >> 3;

            if (_currentBytePosition != bytePos)
            {
                int b = _bytes.ReadByte();
                if (b == -1)
                    throw new EndOfStreamException();
                _currentByte = (byte)b;
                _currentBytePosition = bytePos;
            }

            long posInByte = 7 - _bitsPosition % 8;
            int bit = _currentByte >> (int)posInByte & 1;
            ++_bitsPosition;
            return bit;
        }

        public void WriteBit(int value)
        {
            long posInByte = 7 - _bitsPosition % 8;
            int bit = (value & 1) << (int)posInByte;
            _currentByte = (byte)(_currentByte | bit);
            ++_bitsPosition;

            long bytePos = _bitsPosition >> 3;
            if (_currentBytePosition != bytePos)
            {
                if (_currentBytePosition != -1) // special case for the first bit
                {
                    _bytes.WriteByte(_currentByte);
                    _currentByte = 0;
                }
                _currentBytePosition = bytePos;
            }
        }

        public ulong ReadBits(uint count, out uint value)
        {
            if (count > 32)
                throw new ArgumentException("'count' cannot be greater than 32", nameof(count));

            uint originalCount = count;
            int res = 0;
            while (count > 0)
            {
                res <<= 1;
                res |= ReadBit();
                count--;
            }

            value = (uint)res;
            return originalCount;
        }

        public uint ReadBits(uint count)
        {
            ReadBits(count, out uint value);
            return value;
        }

        public ulong ReadBits(uint count, out ulong value)
        {
            if (count > 64)
                throw new ArgumentException("'count' cannot be greater than 64", nameof(count));

            uint originalCount = count;
            long res = 0;
            while (count > 0)
            {
                res <<= 1;
                res |= (long)ReadBit();
                count--;
            }

            value = (ulong)res;
            return originalCount;
        }

        public ulong ReadBits(ulong count)
        {
            ReadBits((uint)count, out ulong value);
            return value;
        }

        public ulong WriteBits(uint count, ulong value)
        {
            if (count > 64)
                throw new ArgumentException("'count' cannot be greater than 64", nameof(count));

            uint originalCount = count;
            while (count > 0)
            {
                int bits = (int)count - 1;
                ulong mask = 0x1u << bits;
                WriteBit((int)((value & mask) >> bits));
                count--;
            }

            return originalCount;
        }

        public uint WriteBits(uint count, uint value)
        {
            if (count > 32)
                throw new ArgumentException("'count' cannot be greater than 32", nameof(count));

            uint originalCount = count;
            while (count > 0)
            {
                int bits = (int)count - 1;
                ulong mask = 0x1u << bits;
                WriteBit((int)((value & mask) >> bits));
                count--;
            }

            return originalCount;
        }

        #endregion

        #region Bytes

        /// <summary>A byte, of the next eight bits wherever they start.</summary>
        public byte ReadByte() => (byte)ReadBits(8);

        /// <summary>A byte, as the next eight bits.</summary>
        public ulong WriteByte(byte value) => WriteBits(8, (uint)value);

        /// <summary>
        /// Up to <paramref name="count"/> whole bytes at a byte boundary, fewer where the stream ends first: how many. The
        /// next bit read is the first after them. A payload of megabytes read at once, rather than bit by bit.
        /// </summary>
        public int ReadAvailableBytes(byte[] buffer, int offset, int count)
        {
            if (_bitsPosition % 8 != 0)
                throw new InvalidOperationException("Not at a byte boundary.");

            int read = 0;
            while (read < count)
            {
                int n = _bytes.Read(buffer, offset + read, count - read);
                if (n <= 0)
                    break;
                read += n;
            }
            _bitsPosition += (long)read * 8;
            return read;
        }

        /// <summary>Whole bytes at a byte boundary, as <see cref="ReadAvailableBytes"/>: all of them, or the stream ends.</summary>
        public void ReadAlignedBytes(byte[] buffer, int offset, int count)
        {
            if (ReadAvailableBytes(buffer, offset, count) < count)
                throw new EndOfStreamException();
        }

        /// <summary>Writes whole bytes at a byte boundary straight to the stream; the next bit written is the first after them.</summary>
        public void WriteAlignedBytes(byte[] buffer, int offset, int count)
        {
            if (_bitsPosition % 8 != 0)
                throw new InvalidOperationException("Not at a byte boundary.");

            _bytes.Write(buffer, offset, count);
            _bitsPosition += (long)count * 8;
            // The byte the next bit goes in, as WriteBit keeps it
            _currentBytePosition = _bitsPosition >> 3;
            _currentByte = 0;
        }

        /// <summary>
        /// Moves on over whole bytes at a byte boundary, unread: of those read ahead first, then of the stream - by seeking
        /// where it can and no peek is on, else by reading them, kept where a peek is on.
        /// </summary>
        public void SkipBytes(long count)
        {
            if (_bitsPosition % 8 != 0)
                throw new InvalidOperationException("Not at a byte boundary.");

            _bytes.Skip(count);
            _bitsPosition += count * 8;
        }

        #endregion
    }
}
