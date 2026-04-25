using System.IO;

namespace SharpMP4.Common
{
    public class RbspBitstream
    {
        private bool _skipStartCodes = true;
        private bool _insertStartCodes = true;
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

        public bool SkipStartCodes
        {
            get => _skipStartCodes;
            set => _skipStartCodes = value;
        }

        public bool InsertStartCodes
        {
            get => _insertStartCodes;
            set => _insertStartCodes = value;
        }

        public virtual void CopyState(RbspBitstream bitstream)
        {
            this._prevByte = bitstream._prevByte;
            this._prevPrevByte = bitstream._prevPrevByte;
            this._currentByte = bitstream._currentByte;
            this.InsertStartCodes = bitstream.InsertStartCodes;
            this.SkipStartCodes = bitstream.SkipStartCodes;
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

                if (_skipStartCodes && _prevByte == 0 && _currentByte == 0 && b == 0x03)
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
                    _prevByte = _currentByte;
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

                if (_skipStartCodes)
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
    }
}
