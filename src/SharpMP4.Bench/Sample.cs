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
