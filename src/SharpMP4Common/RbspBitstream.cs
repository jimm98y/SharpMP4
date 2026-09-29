using System;
using System.IO;

namespace SharpMP4.Common
{
    public class RbspBitstream
    {
        private bool _skipPreventionBytes = true;
        private bool _insertPreventionBytes = true;
        private int _prevByte = -1;
        private int _prevPrevByte = -1;
        private long _lastMarkPos;
        protected long _bitsPosition;
        protected long _currentBytePosition = -1;
        protected byte _currentByte;

        protected Stream _stream;

        public RbspBitstream(Stream stream)
        {
            this._stream = stream;
        }

        /// <summary>
        /// Reads a NAL unit where it already lies, in an array: no stream in between, so it costs no allocation and no
        /// virtual call a byte, and <see cref="Reset"/> moves it on to the next one.
        /// </summary>
        public RbspBitstream(byte[] buffer, int offset, int length)
        {
            Reset(buffer, offset, length);
        }

        // The array read from, where there is one rather than a stream.
        private byte[] _source;
        private int _sourcePosition;
        private int _sourceEnd;

        /// <summary>
        /// Starts over on another NAL unit, in an array: every bit of state as a new bitstream has it, the bytes read
        /// ahead and the mark too.
        /// </summary>
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

            _prevByte = -1;
            _prevPrevByte = -1;
            _lastMarkPos = 0;
            _bitsPosition = 0;
            _currentBytePosition = -1;
            _currentByte = 0;
            _replayPosition = 0;
            _replayLength = 0;
            _peekDepth = 0;
        }

        /// <summary>
        /// How many bytes are still to be read: of the array, or of the stream where it can tell, and those read ahead
        /// of the bits to be read again. Throws <see cref="NotSupportedException"/> for a stream that cannot tell.
        /// </summary>
        public long RemainingBytes => (_source != null ? _sourceEnd - _sourcePosition : _stream.Length - _stream.Position) + PendingBytes;

        private int ReadSourceByte() => _source != null
            ? (_sourcePosition < _sourceEnd ? _source[_sourcePosition++] : -1)
            : _stream.ReadByte();

        public Stream BaseStream
        {
            get => _stream;
            set => _stream = value;
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

        public virtual void CopyState(RbspBitstream bitstream)
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

        public virtual void Mark() => this._lastMarkPos = this._bitsPosition;
        public virtual long GetBitsSinceMark() => this._bitsPosition - this._lastMarkPos;

        /// <summary>Moves the mark back by a number of bits, as counted by GetBitsSinceMark.</summary>
        public virtual void MoveMarkBack(long bits) => this._lastMarkPos -= bits;

        // Reading ahead: the bytes taken from the stream while peeking are kept, to be read again once the peek ends,
        // so a stream is read ahead on as it is - one that cannot seek too - and nothing is copied.
        private byte[] _replay;
        private int _replayPosition;
        private int _replayLength;
        private int _peekDepth;

        /// <summary>Where the bits were when a peek began, to go back to at its end.</summary>
        public readonly struct PeekState
        {
            internal PeekState(RbspBitstream bitstream)
            {
                PrevByte = bitstream._prevByte;
                PrevPrevByte = bitstream._prevPrevByte;
                LastMarkPos = bitstream._lastMarkPos;
                BitsPosition = bitstream._bitsPosition;
                CurrentBytePosition = bitstream._currentBytePosition;
                CurrentByte = bitstream._currentByte;
                ReplayPosition = bitstream._replayPosition;
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
        public PeekState BeginPeek()
        {
            _peekDepth++;
            return new PeekState(this);
        }

        /// <summary>Ends reading ahead, going back to where the bits were when <paramref name="state"/> was taken.</summary>
        public void EndPeek(PeekState state)
        {
            _prevByte = state.PrevByte;
            _prevPrevByte = state.PrevPrevByte;
            _lastMarkPos = state.LastMarkPos;
            _bitsPosition = state.BitsPosition;
            _currentBytePosition = state.CurrentBytePosition;
            _currentByte = state.CurrentByte;
            _replayPosition = state.ReplayPosition;
            _peekDepth--;
        }

        /// <summary>The bytes read from the stream ahead of the bits, to be read again: the stream is that far ahead of them.</summary>
        public int PendingBytes => _replayLength - _replayPosition;

        /// <summary>
        /// The next byte: of those read ahead first, then of the stream, kept where a peek is on. -1 at the end. Not an
        /// emulation prevention byte skipped, nor the bits moved on: <see cref="ReadBit"/> and <see cref="ReadBytes"/> do
        /// that.
        /// </summary>
        private int NextByte()
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
                if (_replay == null)
                    _replay = new byte[64];
                else if (_replayLength == _replay.Length)
                    Array.Resize(ref _replay, _replay.Length * 2);
                _replay[_replayLength++] = (byte)b;
                _replayPosition = _replayLength;
            }
            return b;
        }

        public int ReadBit()
        {
            long bytePos = _bitsPosition / 8;

            if (_currentBytePosition != bytePos)
            {
                int bb = NextByte();
                if (bb == -1)
                {
                    return -1;
                }

                byte b = (byte)bb;

                if (_skipPreventionBytes && _prevByte == 0 && _currentByte == 0 && b == 0x03)
                {
                    _prevByte = b;
                    bb = NextByte();

                    if (bb == -1)
                    {
                        return -1;
                    }

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
            }

            long posInByte = 7 - _bitsPosition % 8;
            int bit = _currentByte >> (int)posInByte & 1;
            _bitsPosition++;

            return bit;
        }

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

                if (_insertPreventionBytes)
                {
                    if (_prevByte == 0x00 &&
                        _prevPrevByte == 0x00 &&
                        (_currentByte is 0x00 or 0x01 or 0x02 or 0x03))
                    {
                        _stream.WriteByte(0x03);
                        bytePos++;
                        _bitsPosition += 8;
                        _lastMarkPos += 8;
                        _prevByte = 0x03;
                    }
                }

                _stream.WriteByte(_currentByte);
                _currentBytePosition = bytePos;

                _prevPrevByte = _prevByte;
                _prevByte = _currentByte;

                _currentByte = 0;
            }
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
                if (_currentBytePosition != bytePos)
                {
                    int bb = NextByte();
                    if (bb == -1)
                        break;

                    byte b = (byte)bb;

                    if (_skipPreventionBytes && _prevByte == 0 && _currentByte == 0 && b == 0x03)
                    {
                        _prevByte = b;
                        bb = NextByte();

                        if (bb == -1)
                            break;

                        b = (byte)bb;
                        _bitsPosition += 8;
                        _lastMarkPos += 8;
                        bytePos++;
                    }
                    else
                    {
                        _prevByte = _currentBytePosition < 0 ? -1 : _currentByte;
                    }

                    _currentByte = b;
                    _currentBytePosition = bytePos;
                }

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

                if (_insertPreventionBytes &&
                    _prevByte == 0x00 &&
                    _prevPrevByte == 0x00 &&
                    (_currentByte is 0x00 or 0x01 or 0x02 or 0x03))
                {
                    _stream.WriteByte(0x03);
                    bytePos++;
                    _bitsPosition += 8;
                    _lastMarkPos += 8;
                    _prevByte = 0x03;
                }

                _stream.WriteByte(_currentByte);
                _currentBytePosition = bytePos;

                _prevPrevByte = _prevByte;
                _prevByte = _currentByte;

                _currentByte = 0;
            }
        }
    }
}
