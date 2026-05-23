using SharpMP4.Common;
using System;
using System.IO;

namespace SharpISOBMFF
{
    public interface IStorage : IDisposable
    {
        IMp4Logger Logger { get; set; }
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

        public IMp4Logger Logger { get; set; }

        public StreamWrapper(Stream stream, IMp4Logger logger = null)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Logger = logger ?? new DefaultMp4Logger();
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

    internal class StorageStream(IStorage storage) : Stream
    {
        public override bool CanRead => true;

        public override bool CanSeek => storage.CanStreamSeek();

        public override bool CanWrite => true;

        public override long Length => storage.GetLength();

        public override long Position
        {
            get => storage.GetPosition();
            set => storage.SeekFromBeginning(value);
        }

        public override void Flush()
        {
            storage.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return storage.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) =>
            origin switch
            {
                SeekOrigin.Begin => storage.SeekFromBeginning(offset),
                SeekOrigin.Current => storage.SeekFromCurrent(offset),
                SeekOrigin.End => storage.SeekFromEnd(offset),
                _ => throw new ArgumentOutOfRangeException(nameof(origin))
            };

        public override void SetLength(long value)
        {
            throw new NotSupportedException("IStorage does not support modifying lengths");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            storage.Write(buffer, offset, count);
        }
    }
}
