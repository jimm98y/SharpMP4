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
                IH26XTrack h26xTrack = parsedTrack.Track as IH26XTrack;
                if (h26xTrack != null)
                {
                    var videoNalus = h26xTrack.GetVideoNALUs();
                    foreach (var nal in videoNalus)
                    {
                        await builder.ProcessSampleAsync(parsedTrack.Track.TrackID, nal);
                    }

                    Mp4Sample sample = null;
                    while ((sample = Mp4Reader.ReadSample(parsed, parsedTrack.Track.TrackID)) != null)
                    {
                        var nalus = Mp4Reader.ReadAU(h26xTrack.NalLengthSize, sample.Data);
                        foreach (var nal in nalus)
                        {
                            await builder.ProcessSampleAsync(parsedTrack.Track.TrackID, nal);
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else if (parsedTrack.Track.HandlerType == HandlerTypes.Sound)
            {
                // sound
                Mp4Sample sample = null;
                while ((sample = Mp4Reader.ReadSample(parsed, parsedTrack.Track.TrackID)) != null)
                {
                    await builder.ProcessSampleAsync(parsedTrack.Track.TrackID, sample.Data, -1);
                }
            }
            else
            {
                // other
                Mp4Sample sample = null;
                while ((sample = Mp4Reader.ReadSample(parsed, parsedTrack.Track.TrackID)) != null)
                {
                    await builder.ProcessSampleAsync(parsedTrack.Track.TrackID, sample.Data, sample.Duration);
                }
            }
        }

        await builder.FinalizeAsync();
    }
}
