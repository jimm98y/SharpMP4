using System;
using System.IO;
using System.Threading.Tasks;

namespace SharpMp4
{
    public interface IMp4Output
    {
        /// <summary>
        /// Retrieves the stream to store the output.
        /// </summary>
        /// <param name="sequenceNumber">Sequence number. 0 = initialization that contains the MOOV. 1 and onwards = MOOF fragment.</param>
        /// <returns>Stream to store the fragment.</returns>
        Task<Stream> GetStreamAsync(uint sequenceNumber);

        /// <summary>
        /// Flush stream.
        /// </summary>
        /// <param name="output">Stream returned by <see cref="GetStreamAsync(uint)"/>.</param>
        /// <param name="sequenceNumber">Sequence number. 0 = initialization that contains the MOOV. 1 and onwards = MOOF fragment.</param>
        /// <returns><see cref="Task"/></returns>
        Task FlushAsync(Stream output, uint sequenceNumber);
    }

    /// <summary>
    /// Single stream output.
    /// </summary>
    public class SingleStreamOutput : IMp4Output
    {
        private readonly Stream _output;

        public SingleStreamOutput(Stream output)
        {
            // var output = File.Open("test.mp4", FileMode.Create, FileAccess.Write, FileShare.Read); // record and allow simultaneous playback
            this._output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public Task<Stream> GetStreamAsync(uint sequenceNumber)
        {
            // sequence number is ignored, fMP4 is all stored inside a single stream: MOOV, MOOF, MDAT, MOOF, MDAT ...
            return Task.FromResult(_output);
        }

        public Task FlushAsync(Stream output, uint sequenceNumber)
        {
            return output.FlushAsync();
        }
    }

    /// <summary>
    /// Output multiple files, one for each fragment.
    /// </summary>
    public class MultiStreamFileOutput : IMp4Output
    {
        private readonly string _path;
        private readonly string _fileName;
        private readonly string _fileExtension;

        public MultiStreamFileOutput(string path, string fileName, string fileExtension = "mp4")
        {
            this._path = path ?? throw new ArgumentNullException(nameof(path));
            this._fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            this._fileExtension = fileExtension ?? throw new ArgumentNullException(nameof(fileExtension));
        }

        public Task<Stream> GetStreamAsync(uint sequenceNumber)
        {
            try
            {
                string path = Path.Combine(this._path, $"{_fileName}{sequenceNumber}.{_fileExtension}");
                if(!Directory.Exists(_path)) 
                {
                    Directory.CreateDirectory(_path);
                }

                var outputStream = File.Create(path);
                return Task.FromResult((Stream)outputStream);
            }
            catch(Exception ex)
            {
                if (Log.ErrorEnabled) Log.Error($"Failed to create file: {ex.Message}");
                throw;
            }
        }

        public async Task FlushAsync(Stream output, uint sequenceNumber)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            await output.FlushAsync();
            output.Dispose();
        }
    }

    public class FragmentBlobEventArgs : EventArgs
    {
        public FragmentBlobEventArgs(int sequenceNumber, byte[] data)
        {
            this.SequenceNumber = sequenceNumber;
            this.Data = data;
        }

        public int SequenceNumber { get; private set; }
        public byte[] Data { get; private set; }
    }

    public class FragmentedBlobOutput : IMp4Output, IDisposable
    {
        public event EventHandler<FragmentBlobEventArgs> OnFragmentReady;
        private uint _currentSequenceNumber = 0;
        private Stream _currentOutputStream = null;
        private bool _disposedValue;

        public Task<Stream> GetStreamAsync(uint sequenceNumber)
        {
            _currentSequenceNumber = sequenceNumber;
            MemoryStream outputStream = new MemoryStream();
            _currentOutputStream = outputStream;
            return Task.FromResult((Stream)outputStream);
        }

        public async Task FlushAsync(Stream output, uint sequenceNumber)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(FragmentedBlobOutput));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (_currentOutputStream != output)
                throw new Exception("Threading error, invalid output stream!");

            if (_currentSequenceNumber != sequenceNumber)
                throw new Exception("Threading error, invalid sequence number!");

            var outputStream = (MemoryStream)output;
            await outputStream.FlushAsync();
            var data = outputStream.ToArray();
            outputStream.Dispose();

            OnFragmentReady?.Invoke(this, new FragmentBlobEventArgs((int)sequenceNumber, data));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_currentOutputStream != null)
                    {
                        _currentOutputStream.Dispose();
                        _currentOutputStream = null;
                    }
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
