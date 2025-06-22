using System;
using System.IO;

namespace SharpISOBMFF
{
    public static class TemporaryStorage
    {
        public static ITemporaryStorageFactory Factory { get; set; } = new TemporaryMemoryStorageFactory();
    }

    public interface ITemporaryStorageFactory
    {
        IStorage Create();
    }

    public class TemporaryMemoryStorageFactory : ITemporaryStorageFactory
    {
        public IStorage Create()
        {
            return new TemporaryMemory();
        }
    }

    public class TemporaryMemory : IStorage
    {
        private readonly MemoryStream _stream;

        public Stream Stream { get { return _stream; } }

        private bool disposedValue;

        public TemporaryMemory()
        {
            _stream = new MemoryStream();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _stream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Flush()
        {
            Stream.Flush();
        }

        public long GetLength()
        {
            return Stream.Length;
        }

        public long GetPosition()
        {
            return Stream.Position;
        }

        public long SeekFromCurrent(long offset)
        {
            return Stream.Seek(offset, SeekOrigin.Current);
        }

        public long SeekFromEnd(long offset)
        {
            return Stream.Seek(offset, SeekOrigin.End);
        }

        public long SeekFromBeginning(long offset)
        {
            return Stream.Seek(offset, SeekOrigin.Begin);
        }

        public void ReadExactly(byte[] data, int offset, int length)
        {
            Stream.ReadExactly(data, offset, length);
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            Stream.Write(buffer, offset, length);
        }

        public int ReadByte()
        {
            return Stream.ReadByte();
        }

        public void WriteByte(byte value)
        {
            Stream.WriteByte(value);
        }

        public bool CanStreamSeek()
        {
            return _stream.CanSeek;
        }

        public int Read(byte[] buffer, int offset, int length)
        {
            return _stream.Read(buffer, offset, length);
        }
    }
}
