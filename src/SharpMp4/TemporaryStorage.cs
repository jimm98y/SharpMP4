using System;
using System.IO;
using System.Threading.Tasks;

namespace SharpMp4
{
    public static class TemporaryStorage
    {
        public static ITemporaryStorageFactory Factory { get; set; } = new TemporaryMemoryStorageFactory();
    }

    public interface ITemporaryStorageFactory
    {
        ITemporaryStorage Create();
    }

    public class TemporaryFileStorageFactory : ITemporaryStorageFactory
    {
        public ITemporaryStorage Create()
        {
            return new TemporaryFile();
        }
    }

    public class TemporaryMemoryStorageFactory : ITemporaryStorageFactory
    {
        public ITemporaryStorage Create()
        {
            return new TemporaryMemory();
        }
    }

    public interface ITemporaryStorage : IDisposable
    {
        Task CopyToAsync(Stream stream);
        Task FlushAsync();
        long GetLength();
        long GetPosition();
        long Seek(long offset, SeekOrigin origin);
        Task ReadExactlyAsync(byte[] data, int offset, int length);
        Task WriteAsync(byte[] buffer, int offset, int length);
        byte ReadByte();
        ushort ReadUInt16();
        uint ReadUInt24();
        uint ReadUInt32();
    }

    public class TemporaryMemory : ITemporaryStorage
    {
        private readonly MemoryStream _stream;

        public Stream Stream {  get { return _stream; } }   

        private bool disposedValue;

        public TemporaryMemory()
        {
            if (Log.InfoEnabled) Log.Info($"{nameof(TemporaryStorage)}: Using {nameof(TemporaryMemory)}");
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

        public Task CopyToAsync(Stream stream)
        {
            return Stream.CopyToAsync(stream);
        }

        public Task FlushAsync()
        {
            return Stream.FlushAsync();
        }

        public long GetLength()
        {
            return Stream.Length;
        }

        public long GetPosition()
        {
            return Stream.Position;
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return Stream.Seek(offset, origin);
        }

        public Task ReadExactlyAsync(byte[] data, int offset, int length)
        {
            return Stream.ReadExactlyAsync(data, offset, length);
        }

        public Task WriteAsync(byte[] buffer, int offset, int length)
        {
            return Stream.WriteAsync(buffer, offset, length);
        }

        public byte ReadByte()
        {
            return IsoReaderWriter.ReadByte(Stream);
        }

        public ushort ReadUInt16()
        {
            return IsoReaderWriter.ReadUInt16(Stream);
        }

        public uint ReadUInt24()
        {
            return IsoReaderWriter.ReadUInt24(Stream);
        }

        public uint ReadUInt32()
        {
            return IsoReaderWriter.ReadUInt32(Stream);
        }
    }

    public class TemporaryFile : ITemporaryStorage
    {
        private FileStream _stream;

        public Stream Stream {  get { return _stream; } }   

        private bool _disposedValue;

        public TemporaryFile()
        {
            if (Log.InfoEnabled) Log.Info($"{nameof(TemporaryStorage)}: Using {nameof(TemporaryFile)}");
            _stream = File.Create(Path.GetRandomFileName(), 1024, FileOptions.DeleteOnClose);
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

        public Task CopyToAsync(Stream stream)
        {
            return Stream.CopyToAsync(stream);
        }

        public Task FlushAsync()
        {
            return Stream.FlushAsync();
        }

        public long GetLength()
        {
            return Stream.Length;
        }

        public long GetPosition()
        {
            return Stream.Position;
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return Stream.Seek(offset, origin);
        }

        public Task ReadExactlyAsync(byte[] data, int offset, int length)
        {
            return Stream.ReadExactlyAsync(data, offset, length);
        }

        public Task WriteAsync(byte[] buffer, int offset, int length)
        {
            return Stream.WriteAsync(buffer, offset, length);
        }

        public byte ReadByte()
        {
            return IsoReaderWriter.ReadByte(Stream);
        }

        public ushort ReadUInt16()
        {
            return IsoReaderWriter.ReadUInt16(Stream);
        }

        public uint ReadUInt24()
        {
            return IsoReaderWriter.ReadUInt24(Stream);
        }

        public uint ReadUInt32()
        {
            return IsoReaderWriter.ReadUInt32(Stream);
        }
    }
}
