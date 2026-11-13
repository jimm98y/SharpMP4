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

using System;
using Mp4Recorder;
using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System.Collections.Generic;
using System.IO;

var logger = ConsoleWithoutInfoDebugLogger.Instance;

using (Stream inputFileStream = new BufferedStream(new FileStream("bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = new Container(logger);
    mp4.Read(new IsoStream(inputFileStream) { Logger = logger });

    // the reader disposes the tracks it made; the clones written below are ours to dispose
    using VideoReader inputReader = new VideoReader(logger);
    inputReader.Parse(mp4);
    IEnumerable<ITrack> inputTracks = inputReader.GetTracks();

    using (Stream output = new BufferedStream(new FileStream("bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        IMp4Builder outputBuilder = new Mp4Builder(new SingleStreamOutput(output)) { Logger = logger };
        Dictionary<uint, uint> mapping = new Dictionary<uint, uint>();
        List<ITrack> outputTracks = new List<ITrack>();

        foreach (var inputTrack in inputTracks)
        {
            var outputTrack = inputTrack.Clone();
            outputTracks.Add(outputTrack);
            outputBuilder.AddTrack(outputTrack);
            mapping.Add(inputTrack.TrackID, outputTrack.TrackID);
        }

        foreach (var inputTrack in inputTracks)
        {
            if (inputTrack.HandlerType == HandlerTypes.Video)
            {
                var videoUnits = inputTrack.GetContainerSamples();
                foreach (var unit in videoUnits)
                {
                    outputBuilder.ProcessTrackSample(mapping[inputTrack.TrackID], unit);
                }

                MediaSample sample = null;
                while ((sample = inputReader.ReadSample(inputTrack.TrackID)) != null)
                {
                    IEnumerable<ArraySegment<byte>> units = inputReader.ParseSample(inputTrack.TrackID, sample.Data);
                    foreach (var unit in units)
                    {
                        outputBuilder.ProcessTrackSample(mapping[inputTrack.TrackID], unit);
                    }
                }
            }
            else
            {
                MediaSample sample = null;
                while ((sample = inputReader.ReadSample(inputTrack.TrackID)) != null)
                {
                    outputBuilder.ProcessTrackSample(mapping[inputTrack.TrackID], sample.Data, sample.Duration);
                }
            }
        }

        outputBuilder.FinalizeMedia();
        outputBuilder.Dispose();
        foreach (var outputTrack in outputTracks)
            outputTrack.Dispose();
    }
}
