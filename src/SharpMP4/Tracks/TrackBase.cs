using SharpISOBMFF;
using SharpMP4.Builders;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SharpMP4.Tracks
{
    public abstract class TrackBase
    {
        private ulong _nextFragmentCreateStartTime = 0;

        public abstract string HandlerName { get; }
        public abstract string HandlerType { get; }
        public abstract string Language { get; set; }

        public uint Timescale { get; set; }
        public uint TrackID { get; set; } = 1;
        public string CompatibleBrand { get; set; } = null;

        public uint SampleDuration { get; set; }
        public uint DefaultSampleFlags { get; set; }

        public ConcurrentQueue<byte[]> _samples = new ConcurrentQueue<byte[]>();
        private IMp4Builder _sink;

        public virtual async Task ProcessSampleAsync(byte[] sample)
        {
            if (SampleDuration == 0)
                throw new InvalidOperationException("SampleDuration must not be 0!");

            _nextFragmentCreateStartTime = _nextFragmentCreateStartTime + SampleDuration;

            if (Log.DebugEnabled) Log.Debug($"{HandlerType}: {_nextFragmentCreateStartTime / (double)Timescale}");

            _samples.Enqueue(sample);

            await _sink.NotifySampleAddedAsync();
        }

        public byte[] ReadSample()
        {
            if (_samples.TryDequeue(out var a))
            {
                return a;
            }

            throw new Exception();
        }

        public void SetSink(IMp4Builder fmp4)
        {
            _sink = fmp4;
        }

        public bool ContainsEnoughSamples(ulong duration)
        {
            return HasSamples() && (ulong)_samples.Count * SampleDuration * 1000 >= duration;
        }

        public bool HasSamples()
        {
            return _samples.Count > 0;
        }

        public virtual Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public abstract Box CreateSampleEntryBox();

        public abstract void FillTkhdBox(TrackHeaderBox tkhd);
    }
}
