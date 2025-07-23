using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.IO;

SharpH26X.Log.SinkDebug = (o, e) => { };
SharpH26X.Log.SinkInfo = (o, e) => { };

using (Stream inputFileStream = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var fmp4 = new Container();
    fmp4.Read(new IsoStream(inputFileStream));

    ContainerContext parsed = Mp4Reader.Parse(fmp4);
    IEnumerable<ITrack> parsedTracks = parsed.GetTracks();

    using (Stream output = new BufferedStream(new FileStream("frag_bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        IMp4Builder builder = new FragmentedMp4Builder(new SingleStreamOutput(output), 2000);

        foreach(var track in parsedTracks)
        {
            builder.AddTrack(track);
        }

        for (int t = 0; t < parsed.Tracks.Length; t++)
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
                    var units = Mp4Reader.ParseSample(parsed, parsedTrack.Track.TrackID, sample.Data);
                    foreach (var unit in units)
                    {
                        builder.ProcessTrackSample(parsedTrack.Track.TrackID, unit);
                    }
                }
            }
            else if (parsedTrack.Track.HandlerType == HandlerTypes.Sound)
            {
                // sound
                Mp4Sample sample = null;
                while ((sample = Mp4Reader.ReadSample(parsed, parsedTrack.Track.TrackID)) != null)
                {
                    builder.ProcessTrackSample(parsedTrack.Track.TrackID, sample.Data);
                }
            }
            else
            {
                // other
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
