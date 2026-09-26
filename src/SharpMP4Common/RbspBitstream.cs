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

        public int ReadBit()
        {
            long bytePos = _bitsPosition / 8;

            if (_currentBytePosition != bytePos)
            {
                int bb = _stream.ReadByte();
                if (bb == -1)
                {
                    return -1;
                }

                byte b = (byte)bb;

                if (_skipPreventionBytes && _prevByte == 0 && _currentByte == 0 && b == 0x03)
                {
                    _prevByte = b;
                    bb = _stream.ReadByte();

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
                    int bb = _stream.ReadByte();
                    if (bb == -1)
                        break;

                    byte b = (byte)bb;

                    if (_skipPreventionBytes && _prevByte == 0 && _currentByte == 0 && b == 0x03)
                    {
                        _prevByte = b;
                        bb = _stream.ReadByte();

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
