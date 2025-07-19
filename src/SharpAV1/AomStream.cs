using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpAV1
{
    public class AomStream : IDisposable
    {
        private int _bitsPosition;
        private int _currentBytePosition = -1;
        private byte _currentByte = 0;
        private int _prevByte = -1;
        private int _prevPrevByte = -1;

        private readonly Stream _stream;

        private int _rbspDataCounter = -1;
        private int _readNextBitsCounter = -1;
        private int _readNextBitsIndex = 0;

        private bool _disposedValue;

        public AomStream(Stream stream)
        {
            this._stream = stream ?? new MemoryStream();
        }

        #region Bit read/write

        private int ReadByte()
        {
            int ret = _stream.ReadByte();
            return ret;
        }

        private ulong WriteByte(byte value)
        {
            _stream.WriteByte(value);
            return 8;
        }

        private int ReadBit()
        {
            int bytePos = _bitsPosition / 8;

            if (_currentBytePosition != bytePos)
            {
                int bb = ReadByte();
                if (bb == -1)
                {
                    return -1;
                }

                byte b = (byte)bb;
                _prevByte = _currentByte;

                _currentByte = b;
                _currentBytePosition = bytePos;
            }

            int posInByte = 7 - _bitsPosition % 8;
            int bit = _currentByte >> posInByte & 1;
            ++_bitsPosition;
            return bit;
        }

        private long ReadBits(int count)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));

            long res = 0;
            while (count > 0)
            {
                res = res << 1;
                int u1 = ReadBit();

                if (u1 == -1)
                    return -1;

                res |= (byte)u1;
                count--;
            }

            return res;
        }

        public void WriteBit(int value)
        {
            int posInByte = 7 - (int)_bitsPosition % 8;
            int bit = (value & 1) << posInByte;
            _currentByte = (byte)(_currentByte | bit);
            ++_bitsPosition;

            int bytePos = _bitsPosition / 8;
            if (_currentBytePosition != bytePos)
            {
                if (_currentBytePosition < 0)
                {
                    _currentBytePosition = bytePos;
                    return;
                }
                               
                WriteByte(_currentByte);
                _currentBytePosition = bytePos;

                _prevPrevByte = _prevByte;
                _prevByte = _currentByte;

                _currentByte = 0;
            }
        }

        private void WriteBits(int count, ulong value)
        {
            if (count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count > 0)
            {
                int bits = count - 1;
                ulong mask = 0x1ul << bits;
                WriteBit((int)((value & mask) >> bits));
                count--;
            }
        }

        #endregion // Bit read/write

        public ulong ReadLeb128(out int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteLeb128(int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadFixed(int count, out int value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            ulong read = ReadUnsignedInt(count, out uint v, name);
            value = (int)v;
            return read;
        }

        public ulong WriteFixed(int count, int value, string name)
        {
            return WriteUnsignedInt(count, (uint)value, name);
        }

        public ulong ReadVariable(long length, out int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteVariable(long length, int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadUvlc(out uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteUvlc(uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadSignedIntVar(int length, out int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteSignedIntVar(int length, int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadUnsignedInt(int count, out uint value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            long ret = ReadBits((int)count);
            if (ret == -1)
                throw new EndOfStreamException();
            value = (uint)ret;
            LogEnd(name, (ulong)count, value);
            return (ulong)count;
        }

        public ulong WriteUnsignedInt(int length, uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadLeVar(int length, out int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteLeVar(int length, int value, string name)
        {
            throw new NotImplementedException();
        }

        public int GetPosition()
        {
            throw new NotImplementedException();
        }

        public static int Clip3(int v, int limit, int feature_value)
        {
            throw new NotImplementedException();
        }

        public static int GetQIndex(int v, int segmentId)
        {
            throw new NotImplementedException();
        }

        private int _logLevel = 0;

        private void LogBegin(string name)
        {
            string padding = "-";
            for (int i = 0; i < _logLevel; i++)
            {
                padding += "-";
            }

            Log.Info($"{padding} {name}");
        }

        private void LogEnd<T>(string name, ulong size, T value)
        {
            string padding = "-";
            for (int i = 0; i < _logLevel; i++)
            {
                padding += "-";
            }

            string endPadding = "";
            for (int i = 0; i < 64 - padding.Length - name.Length - size.ToString().Length - 2; i++)
            {
                endPadding += " ";
            }

            Log.Info($"{padding} {name}{endPadding}{size}   {value}");
        }

        #region IDisposable implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion // Disposable implementation
    }
}
