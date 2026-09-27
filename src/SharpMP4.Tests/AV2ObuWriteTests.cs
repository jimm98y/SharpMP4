using SharpAV2;
using SharpAVX;
using SharpMP4.Common;

namespace SharpMP4.Tests;

/// <summary>Tests against writing AV2 OBUs again.</summary>
[TestClass]
public class AV2ObuWriteTests
{
    // The top left 16x16 of akiyo's first four frames, encoded by AVM v1.0.0 (avmenc --limit=4
    // --cpu-used=6): each a temporal unit of length-delimited OBUs (Annex B) - a temporal delimiter,
    // the sequence header and a key frame's tile group, then a frame and two TIP frames.
    private static readonly string[] TemporalUnits =
    {
        "01081704800a000cffc1e6000000e6ee6809abf3bbf526e65690980110f0000000d81c0e649d4ba5abefb0b6eac1cc77a3395d2273a3ec104ea65fbf8a03698f48cd0041d16fb1956c2e2a5e988938fa4310c7817fb1ae90181f3a24c87ee793642e512d141adda0e418007ce3e7e8fe6b3dfdb191d67d19a8f15914c0295dc4c3c7fdfb0af9c5529f9d07cdbc3fd342d01a11c85f4cfa7ee68bb1b42f40756972b83d75563ddace0bf3a8e34cbe1be2dbffcc80",
        "0108091cf80931011000c0a3",
        "01080538e04ac540",
        "01080538e06bc540",
    };

    /// <summary>Each OBU of the temporal units, after its length: the bytes open_bitstream_unit reads.</summary>
    private static IEnumerable<byte[]> Obus()
    {
        foreach (string unit in TemporalUnits)
        {
            byte[] bytes = Convert.FromHexString(unit);
            int pos = 0;
            while (pos < bytes.Length)
            {
                int size = 0, shift = 0;
                byte b;
                do
                {
                    b = bytes[pos++];
                    size |= (b & 0x7f) << shift;
                    shift += 7;
                } while ((b & 0x80) != 0);
                yield return bytes.AsSpan(pos, size).ToArray();
                pos += size;
            }
        }
    }

    private static AV2Obu Read(AV2Context context, byte[] obu)
    {
        using var stream = new AomStream(new MemoryStream(obu), new DefaultMp4Logger());
        context.Read(stream, obu.Length);
        return context.LastObu;
    }

    private static byte[] Write(AV2Context context, AV2Obu obu)
    {
        var written = new MemoryStream();
        using (var stream = new AomStream(written, new DefaultMp4Logger()))
            context.Write(stream, obu);
        return written.ToArray();
    }

    /// <summary>
    /// Each OBU read is written again as it was, bit for bit, by a context that has only written: the
    /// frames after the key frame depend on it as they do on the reader's.
    /// </summary>
    [TestMethod]
    public void WritesEachObuAsItWasRead()
    {
        var reader = new AV2Context { RecordSyntax = true };
        var writer = new AV2Context();

        int count = 0;
        foreach (byte[] obu in Obus())
        {
            CollectionAssert.AreEqual(obu, Write(writer, Read(reader, obu)), $"OBU {count}");
            count++;
        }
        Assert.AreEqual(9, count);
    }

    /// <summary>An element changed in the state it was read into is written with its new value.</summary>
    [TestMethod]
    public void WritesAnEditedElement()
    {
        var reader = new AV2Context { RecordSyntax = true };
        var writer = new AV2Context();
        byte[][] obus = Obus().ToArray();
        Write(writer, Read(reader, obus[0]));
        AV2Obu sequenceHeader = Read(reader, obus[1]);
        Assert.AreEqual(0, sequenceHeader.Edit()._SeqLevelIdx);

        sequenceHeader.Edit()._SeqLevelIdx = 5;
        byte[] written = Write(writer, sequenceHeader);

        var check = new AV2Context();
        Read(check, obus[0]);
        using var stream = new AomStream(new MemoryStream(written), new DefaultMp4Logger());
        check.Read(stream, written.Length);
        Assert.AreEqual(5, check._SeqLevelIdx);
        Assert.AreEqual(written.Length * 8, stream.GetPosition(), "read to the end of the OBU, and no further");
        Assert.AreEqual(obus[1].Length, written.Length);
    }

    /// <summary>
    /// A sequence header written from the state it was read into alone, as an encoder writes one, is
    /// the one read: all it codes, the state holds.
    /// </summary>
    [TestMethod]
    public void WritesASequenceHeaderFromItsState()
    {
        var reader = new AV2Context { RecordSyntax = true };
        var writer = new AV2Context();
        byte[][] obus = Obus().ToArray();
        Write(writer, Read(reader, obus[0]));

        CollectionAssert.AreEqual(obus[1], Write(writer, Read(reader, obus[1]).FromState()));
    }
}
