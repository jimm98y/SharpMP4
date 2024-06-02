using Serilog;
using Serilog.Extensions.Logging;
using SharpMp4;
using SharpRTSPtoWebRTC.RTSP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

var serilogLogger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Error()
                .WriteTo.Debug()
                .CreateLogger();
var microsoftLogger = new SerilogLoggerFactory(serilogLogger)
    .CreateLogger("Debug");

var client = new RTSPClient(microsoftLogger);
var tracks = new List<TrackBase>();

SemaphoreSlim semaphoreVideo = new SemaphoreSlim(1);
var videoTrack = new H264Track() 
{
    // For this particular stream, SPS does not contain VUI and SDP does not contain framerate.
    // FPS of the stream seems to be 11 fps => 11000 / 1000 = 11
    TimescaleOverride = 11000,
    FrameTickOverride = 1000 
}; 
tracks.Add(videoTrack);
client.Received_SPS_PPS += async (sps, pps, timestamp) =>
{
    await semaphoreVideo.WaitAsync();
    try
    {
        await videoTrack.ProcessSampleAsync(sps);
        await videoTrack.ProcessSampleAsync(pps);
    }
    finally
    {
        semaphoreVideo.Release();
    }
};
client.Received_VPS_SPS_PPS += async (vps, sps, pps, timestamp) =>
{
    await semaphoreVideo.WaitAsync();
    try
    {
        await videoTrack.ProcessSampleAsync(vps);
        await videoTrack.ProcessSampleAsync(sps);
        await videoTrack.ProcessSampleAsync(pps);
    }
    finally
    {
        semaphoreVideo.Release();
    }
};
client.Received_NALs += async (nals, timestamp) =>
{
    await semaphoreVideo.WaitAsync();
    try
    {
        foreach (var nal in nals)
        {
            await videoTrack.ProcessSampleAsync(nal);
        }
    }
    finally
    {
        semaphoreVideo.Release();
    }
};
/*
SemaphoreSlim semaphoreAudio = new SemaphoreSlim(1);
var audioTrack = new AACTrack(2, 44100, 24);
tracks.Add(audioTrack);
client.Received_AAC += async (format, aac, objectType, frequencyIndex, channelConfiguration, timestamp, payloadType) =>
{
    await semaphoreAudio.WaitAsync();
    try
    {
        foreach (var frame in aac)
        {
            await audioTrack.ProcessSampleAsync(frame);
        }
    }
    finally
    {
        semaphoreAudio.Release();
    }
};
*/

using (var output = File.Open("output.mp4", FileMode.Create, FileAccess.Write, FileShare.Read))
{
    using (var writer = new FragmentedMp4Builder(new SingleStreamOutput(output)))
    {
        foreach (var track in tracks)
        {
            writer.AddTrack(track);
        }

        client.Connect("rtsp://stream.strba.sk:1935/strba/VYHLAD_JAZERO.stream", RTSPClient.RTP_TRANSPORT.TCP, null, null);
        Console.WriteLine("Recording started.");
        Console.WriteLine("Press any key to stop recording");

        while (!(Console.KeyAvailable))
        { }

        client.Stop();

        await writer.FlushAsync();
    }
}

Console.WriteLine("Recording completed.");
