﻿using SharpMp4;
using System.IO;
using System.Linq;

using (Stream fs = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    using (var fmp4 = await FragmentedMp4.ParseAsync(fs))
    {
        uint videoTrackId = fmp4.FindVideoTrackID().First();
        uint audioTrackId = fmp4.FindAudioTrackID().First();
        var parsedMDAT = await fmp4.ParseMdatAsync();

        using (Stream output = new BufferedStream(new FileStream("frag_bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
        {
            using (FragmentedMp4Builder builder = new FragmentedMp4Builder(new SingleStreamOutput(output)))
            {
                var videoTrack = new H264Track();
                builder.AddTrack(videoTrack);

                var sourceAudioTrackInfo = fmp4.FindAudioTracks().First().GetAudioSampleEntryBox();
                var audioTrack = new AACTrack((byte)sourceAudioTrackInfo.ChannelCount, (int)sourceAudioTrackInfo.SampleRate, sourceAudioTrackInfo.SampleSize);
                builder.AddTrack(audioTrack);

                foreach (var parsedTrack in parsedMDAT)
                {
                    if (parsedTrack.Key == videoTrackId)
                    {
                        for (int i = 0; i < parsedTrack.Value.Count; i++)
                        {
                            foreach (var nal in parsedTrack.Value[i])
                            {
                                await videoTrack.ProcessSampleAsync(nal);
                            }
                        }
                    }
                    else if (parsedTrack.Key == audioTrackId)
                    {
                        for (int i = 0; i < parsedTrack.Value[0].Count; i++)
                        {
                            await audioTrack.ProcessSampleAsync(parsedTrack.Value[0][i]);
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
            }
        }
    }
 }