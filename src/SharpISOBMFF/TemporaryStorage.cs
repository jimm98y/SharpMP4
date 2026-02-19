using SharpMP4Common;
using System;
using System.IO;

namespace SharpISOBMFF
{
    public static class TemporaryStorage
    {
        public static ITemporaryStorageFactory Factory { get; set; } = new TemporaryFileStorageFactory();
    }

    public interface ITemporaryStorageFactory
    {
        IStorage Create(IMp4Logger logger = null);
    }

    public class TemporaryMemoryStorageFactory : ITemporaryStorageFactory
    {
        public IStorage Create(IMp4Logger logger = null)
        {
            return new TemporaryMemory(logger);
        }
    }

    public class TemporaryMemory : IStorage
    {
        private readonly MemoryStream _stream;

        public Stream Stream { get { return _stream; } }

        private bool disposedValue;

        public IMp4Logger Logger { get; set; }

        public TemporaryMemory(IMp4Logger logger = null)
        {
            _stream = new MemoryStream();

            Logger = logger ?? new DefaultMp4Logger();
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

    public class TemporaryFileStorageFactory : ITemporaryStorageFactory
    {
        public IStorage Create(IMp4Logger logger = null)
        {
            return new TemporaryFile(logger);
        }
    }

    public class TemporaryFile : IStorage
    {
        private FileStream _stream;

        public Stream Stream { get { return _stream; } }

        private bool _disposedValue;

        public IMp4Logger Logger { get; set; }

        public TemporaryFile(IMp4Logger logger = null)
        {
            // NOTE: Make sure to only log if the user actually passed a valid logger
            logger?.LogInfo($"{nameof(TemporaryStorage)}: Using {nameof(TemporaryFile)}");
            
            _stream = File.Create(Path.GetRandomFileName(), 1024, FileOptions.DeleteOnClose);

            Logger = logger ?? new DefaultMp4Logger();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _stream.Close();
                }

                _disposedValue = true;
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
