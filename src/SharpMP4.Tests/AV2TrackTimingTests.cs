using SharpISOBMFF;
using SharpMP4.Tracks;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>Tests against what <see cref="AV2Track"/> takes of its configuration: its timing, and what a clone has.</summary>
[TestClass]
public class AV2TrackTimingTests
{
    // akiyo's first temporal unit's temporal delimiter and sequence header, by AVM v1.0.0, as AV2TrackTests has them
    private const string DelimiterAndSequenceHeader = "01081704800a000cffc1e6000000e6ee6809abf3bbf526e65690";

    /// <summary>
    /// A content interpretation OBU (obu_type 24) of a timing_info only: a tick of num_units_in_display_tick 1001 of
    /// time_scale 30000, equal_picture_interval with a picture of one tick, led by its length.
    /// </summary>
    private const string ContentInterpretationWithTiming = "0b" + "60" + "04" + "000003e9" + "00007530" + "e0";

    /// <summary>
    /// The timing of a content interpretation OBU's timing_info, after the sequence header as before it: the track took
    /// the fallback, 24000 and 1001, whatever the stream said.
    /// </summary>
    [TestMethod]
    public void TakesTheTimingOfTheContentInterpretation()
    {
        var after = new AV2Track();
        after.ProcessSample(Hex(DelimiterAndSequenceHeader), out _, out _);
        Assert.AreEqual(24000u, after.Timescale, "the fallback, before the stream says");
        after.ProcessSample(Hex(ContentInterpretationWithTiming), out _, out _);
        Assert.AreEqual(30000u, after.Timescale);
        Assert.AreEqual(1001, after.DefaultSampleDuration);

        var before = new AV2Track();
        before.ProcessSample(Hex(ContentInterpretationWithTiming), out _, out _);
        before.ProcessSample(Hex(DelimiterAndSequenceHeader), out _, out _);
        Assert.AreEqual(30000u, before.Timescale);
        Assert.AreEqual(1001, before.DefaultSampleDuration);

        // a file's timing stands
        var file = new AV2Track(90000, 3000);
        file.ProcessSample(Hex(DelimiterAndSequenceHeader + ContentInterpretationWithTiming), out _, out _);
        Assert.AreEqual(90000u, file.Timescale);
        Assert.AreEqual(3000, file.DefaultSampleDuration);
    }

    /// <summary>
    /// A clone writes the 'av2C' of its original at once, and a track of no configuration yet has no container samples
    /// rather than null.
    /// </summary>
    [TestMethod]
    public void ClonesWithTheConfiguration()
    {
        Assert.AreEqual(0, new AV2Track().GetContainerSamples().Count());

        var original = new AV2Track(30000, 1001) { Language = "ita" };
        original.ProcessSample(Hex(DelimiterAndSequenceHeader + ContentInterpretationWithTiming), out _, out _);

        var clone = (AV2Track)original.Clone();
        Assert.AreEqual("ita", clone.Language);
        CollectionAssert.AreEqual(original.SequenceHeaderObuRaw, clone.SequenceHeaderObuRaw);
        CollectionAssert.AreEqual(BytesOf(original.CreateSampleEntryBox()), BytesOf(clone.CreateSampleEntryBox()));
        Assert.AreEqual(2, clone.CreateSampleEntryBox().Children.OfType<AV2CodecConfigurationBox>().Single().ConfigObus.Count());
    }
}
