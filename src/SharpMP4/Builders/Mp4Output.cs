// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

using SharpMP4.Common;
using System;
using System.IO;

namespace SharpMP4.Builders
{
    public interface IMp4Output
    {
        /// <summary>
        /// Retrieves the stream to store the output.
        /// </summary>
        /// <param name="sequenceNumber">Sequence number. 0 = initialization that contains the MOOV. 1 and onwards = MOOF fragment.</param>
        /// <returns>Stream to store the fragment.</returns>
        Stream GetStream(uint sequenceNumber);

        /// <summary>
        /// Flush stream.
        /// </summary>
        /// <param name="output">Stream returned by <see cref="GetStream(uint)"/>.</param>
        /// <param name="sequenceNumber">Sequence number. 0 = initialization that contains the MOOV. 1 and onwards = MOOF fragment.</param>
        void Flush(Stream output, uint sequenceNumber);
        
        /// <summary>
        /// The logger used by the implementation of this interface.
        /// </summary>
        IMp4Logger Logger { get; set; }
    }

    /// <summary>
    /// Single stream output.
    /// </summary>
    public class SingleStreamOutput : IMp4Output
    {
        private readonly Stream _output;

        public IMp4Logger Logger { get; set; }

        public SingleStreamOutput(Stream output, IMp4Logger logger)
        {
            // var output = File.Open("test.mp4", FileMode.Create, FileAccess.Write, FileShare.Read); // record and allow simultaneous playback
            _output = output ?? throw new ArgumentNullException(nameof(output));
            Logger = logger;
        }

        public SingleStreamOutput(Stream output) : this(output, DefaultMp4Logger.Instance)
        {
        }

        public Stream GetStream(uint sequenceNumber)
        {
            // sequence number is ignored, fMP4 is all stored inside a single stream: MOOV, MOOF, MDAT, MOOF, MDAT ...
            return _output;
        }

        public void Flush(Stream output, uint sequenceNumber)
        {
            output.Flush();
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

        public IMp4Logger Logger { get; set; }

        public MultiStreamFileOutput(string path, string fileName, IMp4Logger logger, string fileExtension = "mp4")
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            _fileExtension = fileExtension ?? throw new ArgumentNullException(nameof(fileExtension));

            this.Logger = logger;
        }

        public MultiStreamFileOutput(string path, string fileName, string fileExtension = "mp4")
            : this(path, fileName, DefaultMp4Logger.Instance, fileExtension)
        {
        }

        public Stream GetStream(uint sequenceNumber)
        {
            try
            {
                string path = Path.Combine(_path, $"{_fileName}{sequenceNumber}.{_fileExtension}");
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }

                var outputStream = File.Create(path);
                return outputStream;
            }
            catch (Exception ex)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError($"{nameof(IMp4Output)}: Failed to create file: {ex.Message}");
                }

                throw;
            }
        }

        public void Flush(Stream output, uint sequenceNumber)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            output.Flush();
            output.Dispose();
        }
    }

    public class FragmentBlobEventArgs : EventArgs
    {
        public FragmentBlobEventArgs(int sequenceNumber, byte[] data)
        {
            SequenceNumber = sequenceNumber;
            Data = data;
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

        public IMp4Logger Logger { get; set; }

        public FragmentedBlobOutput(IMp4Logger logger)
        {
            Logger = logger;
        }

        public FragmentedBlobOutput()
            : this(DefaultMp4Logger.Instance)
        {
        }

        public Stream GetStream(uint sequenceNumber)
        {
            _currentSequenceNumber = sequenceNumber;
            MemoryStream outputStream = new MemoryStream();
            _currentOutputStream = outputStream;
            return outputStream;
        }

        public void Flush(Stream output, uint sequenceNumber)
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
            outputStream.Flush();
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
