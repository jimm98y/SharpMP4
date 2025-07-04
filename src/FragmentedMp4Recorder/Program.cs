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

using (Stream inputFileStream = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var fmp4 = new Container();
    fmp4.Read(new IsoStream(inputFileStream));

    TrackBox inputVideoTrack = fmp4.FindVideoTracks().First();
    TrackBox inputAudioTrack = fmp4.FindAudioTracks().FirstOrDefault();
    var inputHintTracks = fmp4.FindHintTracks();

    var parsed = Mp4Reader.Parse(fmp4);

    using (Stream output = new BufferedStream(new FileStream("frag_bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        IMp4Builder builder = new FragmentedMp4Builder(new SingleStreamOutput(output), 2666);
        var videoTrack = new H264Track();
        builder.AddTrack(videoTrack);

        var sourceAudioTrackInfo = inputAudioTrack.GetAudioSampleEntryBox();
        var audioTrack = new AACTrack((byte)sourceAudioTrackInfo.Channelcount, sourceAudioTrackInfo.Samplerate >> 16, sourceAudioTrackInfo.Samplesize);
        builder.AddTrack(audioTrack);

        List<RtpMovieHintTrack> hints = new List<RtpMovieHintTrack>();
        foreach (var hintTrack in inputHintTracks)
        {
            var ht = new RtpMovieHintTrack();
            ht.Timescale = hintTrack.Children.OfType<MediaBox>().Single().Children.OfType<MediaHeaderBox>().Single().Timescale;
            builder.AddTrack(ht);
            hints.Add(ht);

            uint trackId = hintTrack.Children.OfType<TrackHeaderBox>().Single().TrackID;

            foreach (var moof in fmp4.Children.OfType<MovieFragmentBox>())
            {
                foreach (var traf in moof.Children.OfType<TrackFragmentBox>())
                {
                    if (traf.Children.OfType<TrackFragmentHeaderBox>().Single().TrackID != trackId)
                        continue;

                    ht.SampleDuration = traf.Children.OfType<TrackRunBox>().SingleOrDefault()._TrunEntry.FirstOrDefault().SampleDuration;
                    break;
                }

                if (ht.SampleDuration != 0)
                    break;
            }
        }

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
            else if (inputHintTracks.Select(x => x.Children.OfType<TrackHeaderBox>().Single().TrackID).Contains((uint)(t + 1)))
            {
                for (int j = 0; j < hints.Count; j++)
                {
                    if (hints[j].TrackID == t + 1)
                    {
                        Mp4Sample sample = null;
                        while ((sample = Mp4Reader.ReadSample(parsed, t + 1)) != null)
                        {
                            await builder.ProcessSampleAsync(hints[j].TrackID, sample.Data);
                        }
                    }
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
