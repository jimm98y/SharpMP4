using System;
using SharpISOBMFF;
using SharpMP4.Common;
using System.Collections.Generic;

namespace SharpMP4.Tracks
{
    public abstract class TrackBase : ITrack
    {
        public abstract string HandlerName { get; }
        public abstract string HandlerType { get; }
        public abstract string Language { get; set; }

        public uint Timescale { get; set; }
        public uint TrackID { get; set; } = 1;
        public string CompatibleBrand { get; set; } = null;

        public int DefaultSampleDuration { get; set; }
        public uint DefaultSampleFlags { get; set; }

        public IMp4Logger Logger { get; set; } = new DefaultMp4Logger();

        /// <summary>
        /// Overrides any auto-detected timescale.
        /// </summary>
        public uint TimescaleOverride { get; set; }

        /// <summary>
        /// Overrides any auto-detected frame tick.
        /// </summary>
        public int FrameTickOverride { get; set; }

        /// <summary>
        /// If it is not possible to retrieve timescale from the video, use this value as a fallback.
        /// </summary>
        public uint TimescaleFallback { get; set; }

        /// <summary>
        /// If it is not possible to retrieve frame tick from the video, use this value as a fallback.
        /// </summary>
        public int FrameTickFallback { get; set; }

        public abstract Box CreateSampleEntryBox();

        public abstract void FillTkhdBox(TrackHeaderBox tkhd);

        /// <summary>
        /// Reads a sample that is a slice of a larger buffer - the buffer a reader fills and then
        /// writes over - which is what a track implements.
        /// </summary>
        public abstract void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint);

        /// <summary>The same, for a sample that is an array of its own.</summary>
        public void ProcessSample(byte[] sample, out ArraySegment<byte> output, out bool isRandomAccessPoint) =>
            ProcessSample(sample, 0, sample == null ? 0 : sample.Length, out output, out isRandomAccessPoint);

        /// <summary>A slice as an array of its own, for the few bytes that are kept rather than written on.</summary>
        protected static byte[] CopyOf(byte[] buffer, int offset, int length)
        {
            var copy = new byte[length];
            System.Buffer.BlockCopy(buffer, offset, copy, 0, length);
            return copy;
        }

        /// <summary>
        /// The sample being assembled, for a track that builds one out of several of the units it
        /// is fed. It is reused from one sample to the next and grows to the largest, so a long
        /// stream costs this buffer rather than a list of every unit in every sample.
        /// </summary>
        private byte[] _sample = new byte[64 * 1024];
        private int _sampleLength;

        /// <summary>Whether anything has been put in the sample being assembled.</summary>
        protected bool HasSample => _sampleLength > 0;

        /// <summary>Adds bytes to the sample being assembled, copied out of the buffer they are in.</summary>
        protected void AppendToSample(byte[] buffer, int offset, int length)
        {
            Reserve(length);
            System.Buffer.BlockCopy(buffer, offset, _sample, _sampleLength, length);
            _sampleLength += length;
        }

        /// <summary>Adds one byte to the sample being assembled.</summary>
        protected void AppendToSample(byte value)
        {
            Reserve(1);
            _sample[_sampleLength++] = value;
        }

        /// <summary>
        /// The finished sample, or nothing if none was assembled. It points into the buffer the
        /// next sample is assembled in, so the caller writes or copies it before feeding more.
        /// </summary>
        protected ArraySegment<byte> TakeSample()
        {
            if (_sampleLength == 0)
                return default;

            var sample = new ArraySegment<byte>(_sample, 0, _sampleLength);
            _sampleLength = 0;
            return sample;
        }

        private void Reserve(int bytes)
        {
            if (_sampleLength + bytes <= _sample.Length)
                return;

            int size = _sample.Length;
            while (size < _sampleLength + bytes)
                size *= 2;

            Array.Resize(ref _sample, size);
        }

        public virtual IEnumerable<byte[]> GetContainerSamples()
        {
            return [];
        }

        public IEnumerable<ArraySegment<byte>> ParseSample(byte[] sample) =>
            ParseSample(sample, 0, sample == null ? 0 : sample.Length);

        public virtual IEnumerable<ArraySegment<byte>> ParseSample(byte[] buffer, int offset, int length)
        {
            return [ new ArraySegment<byte>(buffer, offset, length) ];
        }

        public abstract ITrack Clone();
    }
}
