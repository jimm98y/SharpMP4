using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="FragmentedMp4Builder"/> and the track runs it writes.</summary>
public class FragmentedMp4BuilderTests
{
    /// <summary>
    /// A fragment carries its timing in trun rather than in stts and ctts, so reordered pictures
    /// need the composition time offset recorded there; without it a player shows the samples in
    /// decode order.
    /// </summary>
    [Fact]
    public void WritesCompositionOffsetsIntoTheTrackRun()
    {
        // Decode order 0, 3, 1, 2 against a presentation order of 0, 1, 2, 3.
        var presentation = new[] { 0, 3, 1, 2 };
        var file = BuildFile(4, i => (presentation[i] - i) * 20);

        var trun = Assert.Single(Boxes.FindAll<TrackRunBox>(file));
        Assert.Equal(1, trun.Version);
        Assert.Equal(0x800u, trun.Flags & 0x800);   // sample_composition_time_offset present
        Assert.Equal([0, 40, -20, -20], trun._TrunEntry.Select(e => e.SampleCompositionTimeOffset0).ToArray());
    }

    /// <summary>A stream that is not reordered should not pay for the extra field per sample.</summary>
    [Fact]
    public void WritesNoCompositionOffsetsWhenNothingIsReordered()
    {
        var file = BuildFile(4, _ => 0);

        var trun = Assert.Single(Boxes.FindAll<TrackRunBox>(file));
        Assert.Equal(0, trun.Version);
        Assert.Equal(0u, trun.Flags & 0x800);
    }

    /// <summary>
    /// Builds a single fragment holding every sample and parses the file back. The fragment length
    /// is far longer than the media, so the samples cannot be split across track runs.
    /// </summary>
    private static Container BuildFile(int sampleCount, Func<int, int> compositionOffset)
    {
        using var output = new MemoryStream();
        var builder = new FragmentedMp4Builder(new SingleStreamOutput(output), maxFragmentLengthInMs: 60_000);

        var track = new AACTrack(2, 44100, 16);
        builder.AddTrack(track);

        for (int i = 0; i < sampleCount; i++)
        {
            builder.ProcessRawSample(track.TrackID, new byte[64], 20,
                isRandomAccessPoint: true, new TemporaryMemory(), compositionOffset(i));
        }

        builder.FinalizeMedia();

        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(output.ToArray()))));
        return container;
    }
}
