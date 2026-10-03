using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Common;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>What the tests of the tracks share: a logger that keeps what it is told, and a file written and read back.</summary>
internal static class TrackTestSupport
{
    public static byte[] Hex(string hex) => Convert.FromHexString(hex);

    /// <summary>A file of one track, written by <see cref="Mp4Builder"/> as <paramref name="write"/> feeds it, and read back.</summary>
    public static (Container Container, VideoReader Reader, uint TrackID) WriteAndRead(ITrack track, Action<Mp4Builder, uint> write)
    {
        using var output = new MemoryStream();
        var builder = new Mp4Builder(new SingleStreamOutput(output));
        builder.AddTrack(track);
        write(builder, track.TrackID);
        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        var reader = new VideoReader();
        reader.Parse(container);
        return (container, reader, reader.Tracks.Keys.Single());
    }

    /// <summary>The bytes a box writes, its header with them.</summary>
    public static byte[] BytesOf(Box box)
    {
        using var memory = new MemoryStream();
        new IsoStream(new StreamWrapper(memory)).WriteBox(box, "");
        return memory.ToArray();
    }
}

/// <summary>A logger that keeps the warnings and errors it is told.</summary>
internal sealed class CapturingLogger : IMp4Logger
{
    public List<string> Warnings { get; } = new();
    public List<string> Errors { get; } = new();

    public void LogError(string error) => Errors.Add(error);
    public void LogWarning(string warning) => Warnings.Add(warning);
    public void LogInfo(string info) { }
    public void LogDebug(string debug) { }
    public void LogTrace(string trace) { }

    public bool IsErrorEnabled { get; set; } = true;
    public bool IsWarningEnabled { get; set; } = true;
    public bool IsInfoEnabled { get; set; }
    public bool IsDebugEnabled { get; set; }
    public bool IsTraceEnabled { get; set; }
}
