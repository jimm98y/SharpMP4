using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System.Collections.Generic;
using System.IO;

SharpH26X.Log.SinkDebug = (o, e) => { };
SharpH26X.Log.SinkInfo = (o, e) => { };
SharpAV1.Log.SinkInfo = (o, e) => { };
SharpAV1.Log.SinkDebug = (o, e) => { };

using (Stream inputFileStream = new BufferedStream(new FileStream("bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = new Container();
    mp4.Read(new IsoStream(inputFileStream));

    Mp4Reader inputReader = new Mp4Reader();
    inputReader.Parse(mp4);
    IEnumerable<ITrack> inputTracks = inputReader.GetTracks();

    using (Stream output = new BufferedStream(new FileStream("bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        IMp4Builder outputBuilder = new Mp4Builder(new SingleStreamOutput(output));
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

                Mp4Sample sample = null;
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
                Mp4Sample sample = null;
                while ((sample = inputReader.ReadSample(inputTrack.TrackID)) != null)
                {
                    outputBuilder.ProcessTrackSample(mapping[inputTrack.TrackID], sample.Data, sample.Duration);
                }
            }
        }

        outputBuilder.FinalizeMedia();
    }
}