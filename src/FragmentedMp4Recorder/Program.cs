using SharpMp4;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using (Stream fs = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var fmp4 = await FragmentedMp4.ParseAsync(fs);
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
                
                foreach (var nal in parsedMDAT.VideoNALUs)
                {
                    await videoTrack.ProcessSampleAsync(nal);
                }
                var nals = await fmp4.ReadNextTrackAsync(parsedMDAT, (int)videoTrackId);
                while (nals != null)
                {
                    foreach (var nal in nals)
                    {
                        await videoTrack.ProcessSampleAsync(nal);
                    }
                    nals = await fmp4.ReadNextTrackAsync(parsedMDAT, (int)videoTrackId);
                }

                var frames = await fmp4.ReadNextTrackAsync(parsedMDAT, (int)audioTrackId);
                while(frames != null)
                {
                    foreach (var frame in frames)
                    {
                        await audioTrack.ProcessSampleAsync(frame);
                    }
                    frames = await fmp4.ReadNextTrackAsync(parsedMDAT, (int)audioTrackId);
                }

                await audioTrack.FlushAsync();
                await videoTrack.FlushAsync();
                await builder.FlushAsync();
            }
        }
    }
 }