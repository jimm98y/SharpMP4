using System;
using System.IO;

namespace SharpMP4.Common
{
    /// <summary>
    /// The bits of a NAL unit's RBSP: skipping the emulation prevention bytes as it reads - a 0x03 after two 0x00 - and
    /// putting them in as it writes. Its bytes come through a <see cref="ByteSource"/>, as <see cref="Bitstream"/>'s do:
    /// of an array or a stream, read ahead on and gone back from without seeking.
    /// </summary>
    public class RbspBitstream
    {
        private bool _skipPreventionBytes = true;
        private bool _insertPreventionBytes = true;
        private int _prevByte = -1;
        private int _prevPrevByte = -1;
        private long _lastMarkPos;
        private long _bitsPosition;
        private long _currentBytePosition = -1;
        private byte _currentByte;

        private readonly ByteSource _bytes;

        public RbspBitstream(Stream stream)
        {
            _bytes = new ByteSource(stream);
        }

        /// <summary>
        /// Reads a NAL unit where it already lies, in an array: no stream in between, so it costs no allocation and no
        /// virtual call a byte, and <see cref="Reset"/> moves it on to the next one.
        /// </summary>
        public RbspBitstream(byte[] buffer, int offset, int length)
        {
            _bytes = new ByteSource(buffer, offset, length);
        }

        /// <summary>
        /// Starts over on another NAL unit, in an array: every bit of state as a new bitstream has it, the bytes read
        /// ahead and the mark too.
        /// </summary>
        public void Reset(byte[] buffer, int offset, int length)
        {
            _bytes.Reset(buffer, offset, length);

            _prevByte = -1;
            _prevPrevByte = -1;
            _lastMarkPos = 0;
            _bitsPosition = 0;
            _currentBytePosition = -1;
            _currentByte = 0;
        }

        /// <summary>
        /// How many bytes are still to be read: of the array, or of the stream where it can tell, and those read ahead of
        /// the bits to be read again. Throws <see cref="NotSupportedException"/> for a stream that cannot tell.
        /// </summary>
        public long RemainingBytes => _bytes.RemainingBytes;

        public Stream BaseStream
        {
            get => _bytes.Stream;
            set => _bytes.Stream = value;
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

        public bool SkipPreventionBytes
        {
            get => _skipPreventionBytes;
            set => _skipPreventionBytes = value;
        }

        public bool InsertPreventionBytes
        {
            get => _insertPreventionBytes;
            set => _insertPreventionBytes = value;
        }

        public void CopyState(RbspBitstream bitstream)
        {
            this._prevByte = bitstream._prevByte;
            this._prevPrevByte = bitstream._prevPrevByte;
            this._currentByte = bitstream._currentByte;
            this.InsertPreventionBytes = bitstream.InsertPreventionBytes;
            this.SkipPreventionBytes = bitstream.SkipPreventionBytes;
            this._bitsPosition = bitstream._bitsPosition;
            this._currentBytePosition = bitstream._currentBytePosition;
            this._lastMarkPos = bitstream._lastMarkPos;
        }

        public void Mark() => this._lastMarkPos = this._bitsPosition;
        public long GetBitsSinceMark() => this._bitsPosition - this._lastMarkPos;

        /// <summary>Moves the mark back by a number of bits, as counted by GetBitsSinceMark.</summary>
        public void MoveMarkBack(long bits) => this._lastMarkPos -= bits;

        #region Reading ahead

        /// <summary>Where the bits were when a peek began, to go back to at its end.</summary>
        public readonly struct PeekState
        {
            internal PeekState(RbspBitstream bitstream, int replayPosition)
            {
                PrevByte = bitstream._prevByte;
                PrevPrevByte = bitstream._prevPrevByte;
                LastMarkPos = bitstream._lastMarkPos;
                BitsPosition = bitstream._bitsPosition;
                CurrentBytePosition = bitstream._currentBytePosition;
                CurrentByte = bitstream._currentByte;
                ReplayPosition = replayPosition;
            }

            internal int PrevByte { get; }
            internal int PrevPrevByte { get; }
            internal long LastMarkPos { get; }
            internal long BitsPosition { get; }
            internal long CurrentBytePosition { get; }
            internal byte CurrentByte { get; }
            internal int ReplayPosition { get; }
        }

        /// <summary>
        /// Begins reading ahead: what is read until <see cref="EndPeek"/> is read again after it. Peeks can be nested.
        /// </summary>
        public PeekState BeginPeek() => new PeekState(this, _bytes.BeginPeek());

        /// <summary>Ends reading ahead, going back to where the bits were when <paramref name="state"/> was taken.</summary>
        public void EndPeek(PeekState state)
        {
            _prevByte = state.PrevByte;
            _prevPrevByte = state.PrevPrevByte;
            _lastMarkPos = state.LastMarkPos;
            _bitsPosition = state.BitsPosition;
            _currentBytePosition = state.CurrentBytePosition;
            _currentByte = state.CurrentByte;
            _bytes.EndPeek(state.ReplayPosition);
        }

        /// <summary>The bytes read from the stream ahead of the bits, to be read again: the stream is that far ahead of them.</summary>
        public int PendingBytes => _bytes.PendingBytes;

        #endregion

        /// <summary>The next bit, an emulation prevention byte skipped; -1 at the end.</summary>
        public int ReadBit()
        {
            long bytePos = _bitsPosition / 8;

            if (_currentBytePosition != bytePos && !LoadByte(ref bytePos))
                return -1;

            long posInByte = 7 - _bitsPosition % 8;
            int bit = _currentByte >> (int)posInByte & 1;
            _bitsPosition++;

            return bit;
        }

        /// <summary>
        /// The byte the bit at <paramref name="bytePos"/> is in, loaded: an emulation prevention byte before it skipped,
        /// the bits and the mark moved on over it. False at the end.
        /// </summary>
        private bool LoadByte(ref long bytePos)
        {
            int bb = _bytes.ReadByte();
            if (bb == -1)
                return false;

            byte b = (byte)bb;

            if (_skipPreventionBytes && _prevByte == 0 && _currentByte == 0 && b == 0x03)
            {
                _prevByte = b;
                bb = _bytes.ReadByte();

                if (bb == -1)
                    return false;

                b = (byte)bb;
                _bitsPosition += 8;
                _lastMarkPos += 8;
                bytePos++;
            }
            else
            {
                // Before the first byte is loaded there is no byte before it. The current
                // byte then holds its default zero, and shifting that in would make a stream
                // that opens 00 03 look like one with an emulation prevention byte in it.
                _prevByte = _currentBytePosition < 0 ? -1 : _currentByte;
            }

            _currentByte = b;
            _currentBytePosition = bytePos;
            return true;
        }

        /// <summary>A bit, an emulation prevention byte put in before a byte that needs one.</summary>
        public void WriteBit(int value)
        {
            int posInByte = 7 - (int)_bitsPosition % 8;
            int bit = (value & 1) << posInByte;
            _currentByte = (byte)(_currentByte | bit);
            ++_bitsPosition;

            long bytePos = _bitsPosition / 8;
            if (_currentBytePosition != bytePos)
            {
                if (_currentBytePosition < 0)
                {
                    _currentBytePosition = bytePos;
                    return;
                }

                WriteCurrentByte(ref bytePos);
            }
        }

        /// <summary>
        /// The byte assembled written, an emulation prevention byte before it where the two before it are 0x00 and it is
        /// 0x00 to 0x03: the bits and the mark moved on over that.
        /// </summary>
        private void WriteCurrentByte(ref long bytePos)
        {
            if (_insertPreventionBytes &&
                _prevByte == 0x00 &&
                _prevPrevByte == 0x00 &&
                (_currentByte is 0x00 or 0x01 or 0x02 or 0x03))
            {
                _bytes.WriteByte(0x03);
                bytePos++;
                _bitsPosition += 8;
                _lastMarkPos += 8;
                _prevByte = 0x03;
            }

            _bytes.WriteByte(_currentByte);
            _currentBytePosition = bytePos;

            _prevPrevByte = _prevByte;
            _prevByte = _currentByte;

            _currentByte = 0;
        }

        /// <summary>
        /// Reads whole bytes from a byte aligned position, dropping emulation prevention bytes
        /// exactly as <see cref="ReadBit"/> does, and leaves the state as eight calls to it per byte
        /// would. For the bulk of a NAL unit - the coded slice data behind a header - which would
        /// otherwise cost eight calls a byte.
        /// </summary>
        /// <returns>How many bytes were read: fewer than asked for only at the end of the stream.</returns>
        public int ReadBytes(byte[] buffer, int offset, int count)
        {
            if (_bitsPosition % 8 != 0)
                throw new InvalidOperationException("Bytes can only be read from a byte aligned position.");

            int read = 0;
            while (read < count)
            {
                long bytePos = _bitsPosition / 8;

                // The byte is loaded the way ReadBit loads it for its first bit; the other seven
                // come out of it without touching the stream.
                if (_currentBytePosition != bytePos && !LoadByte(ref bytePos))
                    break;

                buffer[offset + read++] = _currentByte;
                _bitsPosition += 8;
            }

            return read;
        }

        /// <summary>
        /// Writes whole bytes at a byte aligned position, putting emulation prevention bytes in
        /// exactly as <see cref="WriteBit"/> does, and leaves the state as eight calls to it per
        /// byte would. The counterpart of <see cref="ReadBytes"/>.
        /// </summary>
        public void WriteBytes(byte[] buffer, int offset, int count)
        {
            if (_bitsPosition % 8 != 0)
                throw new InvalidOperationException("Bytes can only be written at a byte aligned position.");

            for (int i = 0; i < count; i++)
            {
                // What WriteBit does for the first bit it is ever given.
                if (_currentBytePosition < 0)
                    _currentBytePosition = _bitsPosition / 8;

                _currentByte = (byte)(_currentByte | buffer[offset + i]);
                _bitsPosition += 8;
                long bytePos = _bitsPosition / 8;
                WriteCurrentByte(ref bytePos);
            }
        }
    }
}
