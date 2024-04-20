using System;
using System.IO;

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
        Stream Stream { get; }
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
    }
}
