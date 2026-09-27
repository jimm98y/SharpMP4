using SharpISOBMFF;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMP4.Bench
{
    /// <summary>Loads the file the harness runs against, and the NAL units inside it.</summary>
    internal static class Sample
    {
        /// <summary>The committed sample, so the harness works on a fresh clone.</summary>
        public static string DefaultPath => Path.Combine(AppContext.BaseDirectory, "bunny.mp4");

        /// <summary>
        /// Pulls every NAL unit out of the first video track. A NAL unit is what a parser is handed
        /// by a container, so it is the unit worth fuzzing.
        /// </summary>
        public static List<byte[]> ReadNalUnits(byte[] file, out uint trackId)
        {
            var container = new Container();
            container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));

            var reader = new VideoReader();
            reader.Parse(container);

            trackId = 0;
            var nalUnits = new List<byte[]>();

            foreach (var track in reader.GetTracks())
            {
                if (track.HandlerType != HandlerTypes.Video)
                    continue;

                trackId = track.TrackID;

                // The parameter sets live in the sample entry rather than among the samples, so
                // they have to be taken from the track or nothing that follows can be parsed.
                if (track is H264Track h264)
                {
                    nalUnits.AddRange(h264.SpsRaw.Values);
                    nalUnits.AddRange(h264.PpsRaw.Values);
                }

                break;
            }

            if (trackId == 0)
                return nalUnits;

            while (true)
            {
                var sample = reader.ReadSample(trackId);
                if (sample == null || sample.Data == null)
                    break;

                // The bench keeps every NAL unit, so each is copied out of the reader's buffer.
                foreach (var nalUnit in reader.ParseSample(trackId, sample.Data))
                    nalUnits.Add(nalUnit.ToArray());
            }

            return nalUnits;
        }
    }
}
