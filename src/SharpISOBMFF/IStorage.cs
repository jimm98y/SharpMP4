using System;
using System.IO;

namespace SharpISOBMFF
{
    public interface IStorage : IDisposable
    {
        bool CanStreamSeek();
        void Flush();
        long GetLength();
        long GetPosition();
        long SeekFromCurrent(long offset);
        long SeekFromEnd(long offset);
        long SeekFromBeginning(long offset);
        void ReadExactly(byte[] data, int offset, int length);
        void Write(byte[] buffer, int offset, int length);
        int ReadByte();
        void WriteByte(byte value);
        int Read(byte[] buffer, int offset, int length);
    }

    public class StreamWrapper : IStorage
    {
        public Stream _stream;
        private bool _disposedValue;

        public StreamWrapper(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public void Flush()
        {
            _stream.Flush();
        }

        public long GetLength()
        {
            return _stream.Length;
        }

        public long GetPosition()
        {
            return _stream.Position;
        }

        public int ReadByte()
        {
            return _stream.ReadByte();
        }

        public void ReadExactly(byte[] data, int offset, int length)
        {
            _stream.ReadExactly(data, offset, length);
        }

        public long SeekFromBeginning(long offset)
        {
            return _stream.Seek(offset, SeekOrigin.Begin);
        }

        public long SeekFromCurrent(long offset)
        {
            return _stream.Seek(offset, SeekOrigin.Current);
        }

        public long SeekFromEnd(long offset)
        {
            return _stream.Seek(offset, SeekOrigin.End);
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            _stream.Write(buffer, offset, length);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _stream.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool CanStreamSeek()
        {
            return _stream.CanSeek;
        }

        public void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }

        public int Read(byte[] buffer, int offset, int length)
        {
            return _stream.Read(buffer, offset, length);
        }
    }
}
