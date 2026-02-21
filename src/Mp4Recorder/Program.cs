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

    VideoReader inputReader = new VideoReader(logger);
    inputReader.Parse(mp4);
    IEnumerable<ITrack> inputTracks = inputReader.GetTracks();

    using (Stream output = new BufferedStream(new FileStream("bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        IMp4Builder outputBuilder = new Mp4Builder(new SingleStreamOutput(output)) { Logger = logger };
        Dictionary<uint, uint> mapping = new Dictionary<uint, uint>();

        foreach (var inputTrack in inputTracks)
        {
            var outputTrack = inputTrack.Clone();
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
                    IEnumerable<byte[]> units = inputReader.ParseSample(inputTrack.TrackID, sample.Data);
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
    }
}
