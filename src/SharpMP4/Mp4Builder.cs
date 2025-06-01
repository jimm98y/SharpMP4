using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpMP4
{
    /// <summary>
    /// MP4 builder.
    /// </summary>
    public class Mp4Builder : IDisposable, IMp4Builder
    {
        public uint MovieTimescale { get; set; } = 1000;

        private IMp4Output _output;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private readonly List<TrackBase> _tracks = new List<TrackBase>();
        private readonly List<ulong> _trackEndTimes = new List<ulong>();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="output">Output stream. Will be progressively written while recording. <see cref="IMp4Output"/>.</param>
        public Mp4Builder(IMp4Output output)
        {
            this._output = output;
        }

        /// <summary>
        /// Add a track to the fMP4.
        /// </summary>
        /// <param name="track">Track to add: <see cref="TrackBase"/>.</param>
        public void AddTrack(TrackBase track)
        {
            _tracks.Add(track);
            _trackEndTimes.Add(0);
            track.TrackID = (uint)_tracks.IndexOf(track) + 1;
            track.SetSink(this);
        }

        public async Task NotifySampleAdded()
        {
            // make sure only 1 thread at a time can write
            await _semaphore.WaitAsync();
            try
            {
                // TODO
            }
            finally
            {
                _semaphore.Release();
            }
        }

        #region IDisposable implementation

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable implementation
    }
}
