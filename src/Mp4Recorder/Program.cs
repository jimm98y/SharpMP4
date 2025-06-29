using SharpISOBMFF;
using SharpMP4;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

SharpH26X.Log.SinkDebug = (o, e) => { };
SharpH26X.Log.SinkInfo = (o, e) => { };

using (Stream inputFileStream = new FileStream("bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read))
{
    var mp4 = new Container();
    mp4.Read(new IsoStream(inputFileStream));

    TrackBox inputVideoTrack = mp4.FindVideoTracks().First();
    TrackBox inputAudioTrack = mp4.FindAudioTracks().FirstOrDefault();
    IEnumerable<TrackBox> inputHintTracks = mp4.FindHintTracks();

    ContainerContext parsed = Mp4Reader.Parse(mp4);       

    using (Stream output = new BufferedStream(new FileStream("bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        IMp4Builder builder = new Mp4Builder(new SingleStreamOutput(output));

        var videoTrack = new H264Track();
        builder.AddTrack(videoTrack);

        var audioTrack = new AACTrack(2, 48000, 16, 6);
        builder.AddTrack(audioTrack);

        for (int t = 0; t < parsed.Tracks.Length; t++)
        {
            var parsedTrack = parsed.Tracks[t];
            if (t + 1 == inputVideoTrack.Children.OfType<TrackHeaderBox>().Single().TrackID)
            {
                foreach (var nal in parsedTrack.VideoNals)
                {
                    await builder.ProcessSampleAsync(videoTrack.TrackID, nal);
                }

                Mp4Sample sample = null;
                while ((sample = Mp4Reader.ReadSample(parsed, t + 1)) != null)
                {
                    var nalus = Mp4Reader.ReadAU(parsedTrack.NalLengthSize, sample.Data);
                    foreach (var nal in nalus)
                    {
                        await builder.ProcessSampleAsync(videoTrack.TrackID, nal);
                    }
                }
            }
            else if (t + 1 == inputAudioTrack.Children.OfType<TrackHeaderBox>().Single().TrackID)
            {
                Mp4Sample sample = null;
                while ((sample = Mp4Reader.ReadSample(parsed, t + 1)) != null)
                {
                    await builder.ProcessSampleAsync(audioTrack.TrackID, sample.Data);
                }
            }
            else
            {
                // unknown track
            }
        }

        await builder.FinalizeAsync();
    }
}