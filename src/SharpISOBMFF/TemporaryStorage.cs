using SharpMP4.Common;
using System;
using System.IO;

namespace SharpISOBMFF
{
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

            Logger = logger ?? DefaultMp4Logger.Instance;
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
        private readonly FileStream _stream;

        public Stream Stream { get { return _stream; } }

        /// <summary>Where the file is: in the temporary folder, until it is disposed of.</summary>
        public string FilePath { get { return _stream.Name; } }

        private bool _disposedValue;

        public IMp4Logger Logger { get; set; }

        public TemporaryFile(IMp4Logger logger = null)
        {
            // NOTE: Make sure to only log if the user actually passed a valid logger
            logger?.LogInfo($"Temporary Storage: Using {nameof(TemporaryFile)}");
            
            // in the temporary folder, not the current directory - which may not be writable, and is not where such a file
            // is looked for - deleted as it is closed
            _stream = File.Create(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), 1024, FileOptions.DeleteOnClose);

            Logger = logger ?? DefaultMp4Logger.Instance;
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

    /// <summary>
    /// A stream that cannot seek, which counts the bytes read from it and written to it: its position, which it cannot
    /// tell itself. Where it started at the start of a file, that is an offset of the file.
    /// </summary>
    internal sealed class ForwardOnlyStorage : IStorage
    {
        private readonly IStorage _inner;
        private long _position;

        public ForwardOnlyStorage(IStorage inner)
        {
            _inner = inner;
        }

        public IMp4Logger Logger
        {
            get => _inner.Logger;
            set => _inner.Logger = value;
        }

        public bool CanStreamSeek() => false;

        public long GetPosition() => _position;

        public long GetLength() => _inner.GetLength();

        public int Read(byte[] buffer, int offset, int length)
        {
            int read = _inner.Read(buffer, offset, length);
            if (read > 0)
                _position += read;
            return read;
        }

        public int ReadByte()
        {
            int b = _inner.ReadByte();
            if (b >= 0)
                _position++;
            return b;
        }

        public void ReadExactly(byte[] data, int offset, int length)
        {
            _inner.ReadExactly(data, offset, length);
            _position += length;
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            _inner.Write(buffer, offset, length);
            _position += length;
        }

        public void WriteByte(byte value)
        {
            _inner.WriteByte(value);
            _position++;
        }

        public void Flush() => _inner.Flush();

        public long SeekFromBeginning(long offset) => _position = _inner.SeekFromBeginning(offset);

        public long SeekFromCurrent(long offset) => _position = _inner.SeekFromCurrent(offset);

        public long SeekFromEnd(long offset) => _position = _inner.SeekFromEnd(offset);

        public void Dispose() => _inner.Dispose();
    }
}
