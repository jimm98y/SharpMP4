using SharpISOBMFF;
using SharpMP4;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using (Stream inputFileStream = new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read))
{
    var fmp4 = new Mp4();
    fmp4.Read(new IsoStream(inputFileStream));

    TrackBox inputVideoTrack = fmp4.FindVideoTrack().First();
    TrackBox inputAudioTrack = fmp4.FindAudioTrack().FirstOrDefault();
    var inputHintTracks = fmp4.FindHintTrack();

    var parsedMDAT = fmp4.ParseMdat();

    using (Stream output = new BufferedStream(new FileStream("frag_bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        using (FragmentedMp4Builder builder = new FragmentedMp4Builder(new SingleStreamOutput(output), 2666, 60095))
        {
            var videoTrack = new H264Track();
            builder.AddTrack(videoTrack);

            var sourceAudioTrackInfo = inputAudioTrack.GetAudioSampleEntryBox();
            var audioTrack = new AACTrack((byte)sourceAudioTrackInfo.Channelcount, sourceAudioTrackInfo.Samplerate >> 16, sourceAudioTrackInfo.Samplesize);
            builder.AddTrack(audioTrack);

            List<RtpMovieHintTrack> hints = new List<RtpMovieHintTrack>();
            foreach (var hintTrack in inputHintTracks)
            {
                var ht = new RtpMovieHintTrack();
                ht.Timescale =  hintTrack.Children.OfType<MediaBox>().Single().Children.OfType<MediaHeaderBox>().Single().Timescale;
                builder.AddTrack(ht);
                hints.Add(ht);

                uint trackId = hintTrack.Children.OfType<TrackHeaderBox>().Single().TrackID;
                
                foreach(var moof in fmp4.Children.OfType<MovieFragmentBox>())
                {
                    foreach(var traf in moof.Children.OfType<TrackFragmentBox>())
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

            for (int t = 0; t < parsedMDAT.Count; t++)
            {
                var parsedTrack = parsedMDAT[t];
                if (t + 1 == inputVideoTrack.Children.OfType<TrackHeaderBox>().Single().TrackID)
                {
                    for (int i = 0; i < parsedTrack.Count; i++)
                    {
                        foreach (var nal in parsedTrack[i])
                        {
                            await videoTrack.ProcessSampleAsync(nal);
                        }
                    }
                }
                else if (t + 1 == inputAudioTrack.Children.OfType<TrackHeaderBox>().Single().TrackID)
                {
                    for (int i = 0; i < parsedTrack[0].Count; i++)
                    {
                        await audioTrack.ProcessSampleAsync(parsedTrack[0][i]);
                    }
                }
                else if (inputHintTracks.Select(x => x.Children.OfType<TrackHeaderBox>().Single().TrackID).Contains((uint)(t + 1)))
                {
                    for (int j = 0; j < hints.Count; j++)
                    {
                        if (hints[j].TrackID == t + 1)
                        {
                            for (int i = 0; i < parsedTrack[0].Count; i++)
                            {
                                await hints[j].ProcessSampleAsync(parsedTrack[0][i]);
                            }
                        }
                    }
                }
                else
                {
                    // unknown track
                }
            }

            await audioTrack.FlushAsync();
            await videoTrack.FlushAsync();
            await builder.FlushAsync();
            await builder.FinalizeAsync();
        }
    }
}
