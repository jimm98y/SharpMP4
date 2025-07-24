using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

SharpH26X.Log.SinkDebug = (o, e) => { };
SharpH26X.Log.SinkInfo = (o, e) => { };

using (Stream inputFileStream = new BufferedStream(new FileStream("bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = new Container();
    mp4.Read(new IsoStream(inputFileStream));

    ContainerContext parsed = Mp4Reader.Parse(mp4);
    IEnumerable<ITrack> parsedTracks = parsed.GetTracks();

    using (Stream output = new BufferedStream(new FileStream("bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        IMp4Builder builder = new Mp4Builder(new SingleStreamOutput(output));

        foreach(var track in parsedTracks)
        {
            builder.AddTrack(track);
        }

        for (int t = 0; t < parsedTracks.Count(); t++)
        {
            var parsedTrack = parsed.Tracks[t];
            if (parsedTrack.Track.HandlerType == HandlerTypes.Video)
            {
                var videoUnits = parsedTrack.Track.GetContainerSamples();
                foreach (var unit in videoUnits)
                {
                    builder.ProcessTrackSample(parsedTrack.Track.TrackID, unit);
                }

                Mp4Sample sample = null;
                while ((sample = Mp4Reader.ReadSample(parsed, parsedTrack.Track.TrackID)) != null)
                {
                    IEnumerable<byte[]> units = Mp4Reader.ParseSample(parsed, parsedTrack.Track.TrackID, sample.Data);
                    foreach (var unit in units)
                    {
                        builder.ProcessTrackSample(parsedTrack.Track.TrackID, unit);
                    }
                }
            }
            else
            {
                Mp4Sample sample = null;
                while ((sample = Mp4Reader.ReadSample(parsed, parsedTrack.Track.TrackID)) != null)
                {
                    builder.ProcessTrackSample(parsedTrack.Track.TrackID, sample.Data, sample.Duration);
                }
            }
        }

        builder.FinalizeMedia();
    }
}