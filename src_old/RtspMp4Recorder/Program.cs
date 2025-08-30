using Serilog;
using Serilog.Extensions.Logging;
using SharpMp4;
using SharpRTSPClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

var serilogLogger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Error()
                .WriteTo.Debug()
                .CreateLogger();
var microsoftLogger = new SerilogLoggerFactory(serilogLogger);

var client = new RTSPClient(microsoftLogger);
var tracks = new List<TrackBase>();

SemaphoreSlim semaphoreVideo = new SemaphoreSlim(1);
TrackBase videoTrack = null;
client.NewVideoStream += async (o, e) =>
{
    await semaphoreVideo.WaitAsync();
    try
    {
        if (e.StreamConfigurationData is H264StreamConfigurationData h264cfg)
        {
            videoTrack = new H264Track()
            {
                // For this particular stream, SPS does not contain VUI and SDP does not contain framerate.
                // FPS of the stream seems to be 5 fps => 5000 / 1000 = 5
                TimescaleFallback = 5000,
                FrameTickFallback = 1000
            };
            tracks.Add(videoTrack);

            await videoTrack.ProcessSampleAsync(h264cfg.SPS);
            await videoTrack.ProcessSampleAsync(h264cfg.PPS);
        }
        else if(e.StreamConfigurationData is H265StreamConfigurationData h265cfg)
        {
            videoTrack = new H265Track();
            tracks.Add(videoTrack);

            await videoTrack.ProcessSampleAsync(h265cfg.VPS);
            await videoTrack.ProcessSampleAsync(h265cfg.SPS);
            await videoTrack.ProcessSampleAsync(h265cfg.PPS);
        }
        else
        {
            throw new NotSupportedException("Unsupported video codec");
        }
    }
    finally
    {
        semaphoreVideo.Release();
    }
};
client.ReceivedVideoData += async (o, e) =>
{
    await semaphoreVideo.WaitAsync();
    try
    {
        foreach (var nal in e.Data)
        {
            // NALU here has Annex-B (0 0 0 1 prefix, strip it)
            await videoTrack.ProcessSampleAsync(nal.Slice(4).ToArray());
        }
    }
    finally
    {
        semaphoreVideo.Release();
    }
};

SemaphoreSlim semaphoreAudio = new SemaphoreSlim(1);
TrackBase audioTrack = null;
client.NewAudioStream += (o, e) =>
{
    if (e.StreamConfigurationData is AACStreamConfigurationData aaccfg)
    {
        audioTrack = new AACTrack((byte)aaccfg.ChannelConfiguration, aaccfg.SamplingFrequency, 16);
        tracks.Add(audioTrack);
    }
    else
    {
        throw new NotSupportedException("Unsupported audio codec");
    }
};

client.ReceivedAudioData += async (o, e) =>
{
    await semaphoreAudio.WaitAsync();
    try
    {
        foreach (var frame in e.Data)
        {
            await audioTrack.ProcessSampleAsync(frame.ToArray());
        }
    }
    finally
    {
        semaphoreAudio.Release();
    }
};

using (var output = File.Open("output.mp4", FileMode.Create, FileAccess.Write, FileShare.Read))
{
    using (var writer = new FragmentedMp4Builder(new SingleStreamOutput(output)))
    {
        client.SetupMessageCompleted += (o, e) =>
        {
            foreach (var track in tracks)
            {
                writer.AddTrack(track);
            }
        };

        client.Connect("rtsp://stream.strba.sk:1935/strba/VYHLAD_JAZERO.stream", RTPTransport.TCP);

        Console.WriteLine("Recording started.");
        Console.WriteLine("Press any key to stop recording");

        while (!Console.KeyAvailable)
        {
            System.Threading.Thread.Sleep(250);
        }

        client.Stop();

        await writer.FlushAsync();
    }
}

Console.WriteLine("Recording completed.");
