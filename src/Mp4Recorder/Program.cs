using SharpISOBMFF;
using SharpMP4;
using SharpMP4.Builders;
using SharpMP4.Tracks;
using System.IO;
using System.Linq;

SharpH26X.Log.SinkDebug = (o, e) => { };
SharpH26X.Log.SinkInfo = (o, e) => { };

using (Stream inputFileStream = new FileStream("bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read))
{
    var mp4 = new Mp4();
    mp4.Read(new IsoStream(inputFileStream));

    TrackBox inputVideoTrack = mp4.FindVideoTrack().First();
    TrackBox inputAudioTrack = mp4.FindAudioTrack().FirstOrDefault();
    var inputHintTracks = mp4.FindHintTrack();

    var parsedMDAT = mp4.ParseMdat();

    using (Stream output = new BufferedStream(new FileStream("bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        Mp4Builder builder = new Mp4Builder(new SingleStreamOutput(output));
        {
            var videoTrack = new H264Track();
            builder.AddTrack(videoTrack);

            var audioTrack = new AACTrack(2, 48000, 16, 6);
            builder.AddTrack(audioTrack);

            for (int t = 0; t < parsedMDAT.Count; t++)
            {
                var parsedTrack = parsedMDAT[t];
                if (t + 1 == inputVideoTrack.Children.OfType<TrackHeaderBox>().Single().TrackID)
                {
                    for (int i = 0; i < parsedTrack.Count; i++)
                    {
                        foreach (var nal in parsedTrack[i])
                        {
                            await builder.ProcessSampleAsync(videoTrack.TrackID, nal);
                        }
                    }
                }
                else if (t + 1 == inputAudioTrack.Children.OfType<TrackHeaderBox>().Single().TrackID)
                {
                    for (int i = 0; i < parsedTrack[0].Count; i++)
                    {
                        await builder.ProcessSampleAsync(audioTrack.TrackID, parsedTrack[0][i]);
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
}