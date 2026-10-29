using SharpH264;
using SharpH26X;
using SharpISOBMFF;
using SharpMP4.Readers;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// What an inspector of a file is given, of what is read: each field of a box of where it is in bits, by the hooks of
/// <see cref="BoxTracer"/>; each syntax element of a NAL unit, by an <see cref="IItuTracer"/>; and where each sample
/// is, of <see cref="VideoReader.GetSampleTimings"/> and <see cref="MediaSample.Offset"/>.
/// </summary>
[TestClass]
public class InspectionTests
{
    private sealed class Fields : BoxTracer
    {
        public readonly List<(string Name, ulong Position, ulong Bits, object Value)> Leaves = new();
        public readonly List<(string Kind, string Name, ulong Position)> Nodes = new();

        protected override void OnBegin(string name, IMp4Serializable value, ulong position) => Nodes.Add(("begin", value.DisplayName, position));
        protected override void OnEnd(IMp4Serializable value, ulong position, ulong bits) => Nodes.Add(("end", value.DisplayName, position + bits));
        protected override void OnField(string name, ulong position, ulong bits, object value) => Leaves.Add((name, position, bits, value));
    }

    // stbl of an stts of two entries: 3 samples of 1024, 1 of 512
    private static readonly byte[] Stbl = Hex("00000028" + "7374626C" +
        "00000020" + "73747473" + "00000000" + "00000002" + "00000003" + "00000400" + "00000001" + "00000200");

    [TestMethod]
    public void TellsEachFieldOfWhereItIsItsBitsAndItsTypedValue()
    {
        new IsoStream(new StreamWrapper(new MemoryStream(Stbl))).ReadBox(0, null, out Box box, "");
        var fields = new Fields();
        fields.Append(box);

        CollectionAssert.AreEqual(new[] { "size", "type", "size", "type", "version", "flags", "entry_count",
            "sample_count[0]", "sample_delta[0]", "sample_count[1]", "sample_delta[1]" }, fields.Leaves.Select(x => x.Name).ToArray());

        // the fields one after another, every bit of the box theirs
        ulong position = 0;
        foreach (var leaf in fields.Leaves)
        {
            Assert.AreEqual(position, leaf.Position, leaf.Name);
            position += leaf.Bits;
        }
        Assert.AreEqual((ulong)Stbl.Length * 8, position);

        var count = fields.Leaves.Single(x => x.Name == "sample_delta[0]");
        Assert.IsInstanceOfType<uint>(count.Value);
        Assert.AreEqual(1024u, count.Value);

        CollectionAssert.AreEqual(new[]
        {
            ("begin", "SampleTableBox", 0UL), ("begin", "TimeToSampleBox", 64UL),
            ("end", "TimeToSampleBox", 320UL), ("end", "SampleTableBox", 320UL),
        }, fields.Nodes);
    }

    private sealed class Outer : BoxTracer
    {
        public readonly List<string> Begun = new();
        public Box Root;

        protected override bool Enter(Box box) => ReferenceEquals(box, Root);
        protected override void OnBegin(string name, IMp4Serializable value, ulong position) => Begun.Add(value.DisplayName);
    }

    [TestMethod]
    public void TracesNotWhatIsInABoxNotEntered()
    {
        new IsoStream(new StreamWrapper(new MemoryStream(Stbl))).ReadBox(0, null, out Box box, "");
        var tracer = new Outer { Root = box };
        Assert.AreEqual((ulong)Stbl.Length * 8, tracer.Append(box));
        CollectionAssert.AreEqual(new[] { "SampleTableBox", "TimeToSampleBox" }, tracer.Begun, "the box in it begins, and nothing of it");
        Assert.AreEqual((ulong)Stbl.Length * 8, tracer.Position);
    }

    [TestMethod]
    public void TellsTheSyntaxAClassIsDefinedOf()
    {
        IMp4Serializable box = new MovieHeaderBox();
        StringAssert.Contains(box.Syntax, "class MovieHeaderBox extends FullBox('mvhd', version, 0)");
        StringAssert.Contains(box.Syntax, "unsigned int(32)\ttimescale;");
        StringAssert.Contains(new TimeToSampleBox().Syntax, "extends FullBox('stts', version = 0, 0)");
        StringAssert.Contains(new TimeToSampleBox().Syntax, "unsigned int(32)\tsample_delta;");
        Assert.IsNull(new UnknownBox(IsoStream.FromFourCC("zzzz")).Syntax, "of a class of no definition, none");
    }

    [TestMethod]
    public void TellsTheSyntaxOfAVideoHeader()
    {
        IItuSerializable sps = new SeqParameterSetRbsp();
        StringAssert.Contains(sps.Syntax, "seq_parameter_set_rbsp()");
        StringAssert.Contains(new NalUnit(0).Syntax, "nal_ref_idc All u(2)");
        StringAssert.Contains(new SharpAV1.AV1Context().ObuHeaderSyntax, "obu_type f(4)");
    }

    [TestMethod]
    public void DescribesAsBeforeWhereNothingIsOverridden()
    {
        new IsoStream(new StreamWrapper(new MemoryStream(Stbl))).ReadBox(0, null, out Box box, "");
        StringAssert.Contains(box.ToString(), "192             sample_count[0]          00000000000000000000000000000011 = 3");
    }

    private sealed class Tracer : IItuTracer
    {
        public readonly List<(string Name, ulong End, ulong Bits, object Value)> Fields = new();
        public readonly List<string> Classes = new();

        public void OnBeginClass(string name, IItuSerializable value, ulong position) => Classes.Add("+" + name);
        public void OnEndClass(string name, IItuSerializable value, ulong position) => Classes.Add("-" + name);
        public void OnField(string name, ulong end, ulong bits, object value) => Fields.Add((name, end, bits, value));
    }

    // 16x16 grey by x264: its SPS, of two emulation prevention bytes - the 0x03 of each 00 00 03
    private const string H264Sps = "6764000aacb23d80880000030008000003019078913240";

    /// <summary>Where an element of a NAL unit begins: back from its end by its bits, and by each emulation prevention byte in it.</summary>
    private static ulong StartOf(byte[] nal, ulong end, ulong bits)
    {
        var prevention = new HashSet<ulong>();
        for (int i = 0, zeros = 0; i < nal.Length; i++)
        {
            if (zeros >= 2 && nal[i] == 3)
            {
                prevention.Add((ulong)i);
                zeros = 0;
            }
            else
                zeros = nal[i] == 0 ? zeros + 1 : 0;
        }
        ulong position = end;
        while (bits > 0)
        {
            position--;
            if (!prevention.Contains(position >> 3))
                bits--;
        }
        return position;
    }

    [TestMethod]
    public void TracesEachSyntaxElementOfWhereItEndsInTheNalUnit()
    {
        byte[] nal = Hex(H264Sps);
        var tracer = new Tracer();
        using var stream = new ItuStream(nal, 0, nal.Length) { Tracer = tracer };
        var context = new H264Context();
        var header = new NalUnit((uint)nal.Length);
        context.NalHeader = header;
        header.Read(context, stream);
        context.SeqParameterSetRbsp = new SeqParameterSetRbsp();
        context.SeqParameterSetRbsp.Read(context, stream);

        CollectionAssert.AreEqual(new[] { "forbidden_zero_bit", "nal_ref_idc", "nal_unit_type", "profile_idc" },
            tracer.Fields.Take(4).Select(x => x.Name).ToArray());
        Assert.AreEqual((1UL, 1UL), (tracer.Fields[0].End, tracer.Fields[0].Bits));
        Assert.AreEqual((16UL, 8UL), (tracer.Fields[3].End, tracer.Fields[3].Bits));
        Assert.AreEqual(100UL, Convert.ToUInt64(tracer.Fields[3].Value));

        // one after another, of no gaps, the emulation prevention bytes skipped: an Exp-Golomb element of its whole code, a
        // signed one once
        for (int i = 1; i < tracer.Fields.Count; i++)
            Assert.AreEqual(tracer.Fields[i - 1].End, StartOf(nal, tracer.Fields[i].End, tracer.Fields[i].Bits), tracer.Fields[i].Name);
        Assert.AreEqual((ulong)nal.Length * 8, tracer.Fields[^1].End, "to the end of the NAL unit");

        // num_units_in_tick, 00 00 00 01, of the 0x03 at byte 11 in it
        var tick = tracer.Fields.Single(x => x.Name == "num_units_in_tick");
        Assert.AreEqual(1UL, tick.Value);
        Assert.AreEqual(tick.End - 40, StartOf(nal, tick.End, tick.Bits));

        CollectionAssert.Contains(tracer.Classes, "+seq_parameter_set_data");
        Assert.AreEqual(tracer.Classes.Count(x => x[0] == '+'), tracer.Classes.Count(x => x[0] == '-'));
    }

    [TestMethod]
    public void TellsWhereEachSampleIsOfTheFile()
    {
        var (file, data) = ReaderSampleTableTests.BuildFile(20, i => 10 + i * 3, compositionOffset: i => 0, rap: i => i % 5 == 0);
        AssertOffsets(file, data);
    }

    [TestMethod]
    public void TellsWhereEachSampleIsOfAFragmentedFile()
    {
        var (file, data) = ReaderSampleTableTests.BuildFileWithSamplesInMoovAndFragments(5, 12);
        AssertOffsets(file, data);
    }

    private static void AssertOffsets(byte[] file, byte[][] data)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        var reader = new VideoReader();
        reader.Parse(container);
        uint trackID = reader.Tracks.Keys.Single();

        var timings = reader.GetSampleTimings(trackID);
        Assert.AreEqual(data.Length, timings.Length);
        for (int i = 0; i < data.Length; i++)
        {
            Assert.AreEqual((uint)data[i].Length, timings[i].Size, $"sample {i}: size");
            CollectionAssert.AreEqual(data[i], file.AsSpan((int)timings[i].Offset, data[i].Length).ToArray(), $"sample {i}: where it is");

            var sample = reader.ReadSample(trackID);
            Assert.AreEqual(timings[i].Offset, sample.Offset, $"sample {i}: where it was read");
        }
    }
}
