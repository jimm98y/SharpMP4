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

using (Stream inputFileStream = new BufferedStream(new FileStream("C:\\Temp\\002.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = new Container();
    mp4.Read(new IsoStream(inputFileStream));

    Mp4Reader reader = new Mp4Reader();
    reader.Parse(mp4);
    IEnumerable<ITrack> inputTracks = reader.GetTracks();

    using (Stream output = new BufferedStream(new FileStream("C:\\Temp\\002_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        IMp4Builder builder = new Mp4Builder(new SingleStreamOutput(output));
        Dictionary<uint, uint> mapping = new Dictionary<uint, uint>();

        foreach (var inputTrack in inputTracks)
        {
            var outputTrack = inputTrack.Clone();
            builder.AddTrack(outputTrack);
            mapping.Add(inputTrack.TrackID, outputTrack.TrackID);
        }

        foreach (var inputTrack in inputTracks)
        {
            if (inputTrack.HandlerType == HandlerTypes.Video)
            {
                var videoUnits = inputTrack.GetContainerSamples();
                foreach (var unit in videoUnits)
                {
                    builder.ProcessTrackSample(mapping[inputTrack.TrackID], unit);
                }

                Mp4Sample sample = null;
                while ((sample = reader.ReadSample(inputTrack.TrackID)) != null)
                {
                    IEnumerable<byte[]> units = reader.ParseSample(inputTrack.TrackID, sample.Data);
                    foreach (var unit in units)
                    {
                        builder.ProcessTrackSample(mapping[inputTrack.TrackID], unit);
                    }
                }
            }
            else
            {
                Mp4Sample sample = null;
                while ((sample = reader.ReadSample(inputTrack.TrackID)) != null)
                {
                    builder.ProcessTrackSample(mapping[inputTrack.TrackID], sample.Data, sample.Duration);
                }
            }
        }

        builder.FinalizeMedia();
    }
}