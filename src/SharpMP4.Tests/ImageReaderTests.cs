using System.Buffers.Binary;
using System.Text;
using SharpISOBMFF;
using SharpMP4.Readers;
using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Tests on <see cref="ImageReader"/>, on HEIF files made here byte by byte: items of several extents, items in the
/// 'idat', no 'iref', no 'pitm', and the end of the items.
/// </summary>
[TestClass]
public class ImageReaderTests
{
    private static byte[] Box(string type, params byte[][] content)
    {
        int size = 8 + content.Sum(x => x.Length);
        var box = new List<byte>(size);
        box.AddRange(U32((uint)size));
        box.AddRange(Encoding.ASCII.GetBytes(type));
        foreach (var part in content)
            box.AddRange(part);
        return box.ToArray();
    }

    private static byte[] FullBox(string type, byte version, uint flags, params byte[][] content) =>
        Box(type, [[version, (byte)(flags >> 16), (byte)(flags >> 8), (byte)flags], .. content]);

    private static byte[] U16(int value) => [(byte)(value >> 8), (byte)value];

    private static byte[] U32(uint value)
    {
        var bytes = new byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(bytes, value);
        return bytes;
    }

    private static byte[] U64(ulong value)
    {
        var bytes = new byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(bytes, value);
        return bytes;
    }

    private static byte[] Ascii(string text) => Encoding.ASCII.GetBytes(text);

    /// <summary>
    /// A HEIF file of two items: item 1 of two extents of the 'mdat', with something between them, at offsets of 8 bytes
    /// (offset_size 8); item 2 in the 'idat'. No 'iref', as nothing refers to anything; a 'pitm' or not.
    /// </summary>
    private static (byte[] File, byte[] First, byte[] Second) BuildImage(bool primaryItem)
    {
        byte[] first = Ascii("first extent of item 1;");
        byte[] between = Ascii("<not of any item>");
        byte[] second = Ascii("second extent of item 1");
        byte[] idat = Ascii("..item 2 in the idat..");
        byte[] item2 = idat[2..^2];

        var ftyp = Box("ftyp", Ascii("heic"), U32(0), Ascii("mif1"), Ascii("heic"));

        byte[] Meta(long mdatData) => FullBox("meta", 0, 0,
            FullBox("hdlr", 0, 0, U32(0), Ascii("pict"), U32(0), U32(0), U32(0), [0]),
            primaryItem ? FullBox("pitm", 0, 0, U16(1)) : [],
            FullBox("iinf", 0, 0, U16(2),
                FullBox("infe", 2, 0, U16(1), U16(0), Ascii("hvc1"), [0]),
                FullBox("infe", 2, 0, U16(2), U16(0), Ascii("Exif"), [0])),
            FullBox("iloc", 1, 0,
                [0x88, 0x40], // offset_size 8, length_size 8, base_offset_size 4, index_size 0
                U16(2),
                // item 1: construction method 0, base offset the 'mdat's data, two extents
                U16(1), U16(0), U16(0), U32((uint)mdatData), U16(2),
                U64(0), U64((ulong)first.Length),
                U64((ulong)(first.Length + between.Length)), U64((ulong)second.Length),
                // item 2: construction method 1, in the 'idat', one extent
                U16(2), U16(1), U16(0), U32(0), U16(1),
                U64(2), U64((ulong)item2.Length)),
            Box("iprp",
                Box("ipco", FullBox("ispe", 0, 0, U32(64), U32(48))),
                FullBox("ipma", 0, 0, U32(2), U16(1), [1, 0x81], U16(2), [1, 0x01])),
            Box("idat", idat));

        // the 'meta' is of the same size whatever the offset in it
        long mdatData = ftyp.Length + Meta(0).Length + 8;
        var file = ftyp.Concat(Meta(mdatData)).Concat(Box("mdat", first, between, second)).ToArray();
        return (file, [.. first, .. second], item2);
    }

    private static ImageReader Parse(byte[] file)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
        Assert.IsFalse(container.IsTruncated);

        // no codec this test is about: any property makes the track
        var reader = new ImageReader();
        reader.TrackFactory.CreateTrack = (id, box, timescale, duration, handler, name, logger) => TrackFactory.CreateGenericTrack(id, box, timescale, duration, handler, name);
        reader.Parse(container);
        return reader;
    }

    /// <summary>
    /// Each item is read whole - all of its extents, of the 'mdat' or of the 'idat' - and after the last there is none,
    /// where the last was read again and again; a file without an 'iref', or a 'pitm', is read.
    /// </summary>
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void ReadsEachItemWholeThenNone(bool primaryItem)
    {
        var (file, item1, item2) = BuildImage(primaryItem);
        var reader = Parse(file);

        Assert.AreEqual(64u, reader.Ispe.ImageWidth);
        CollectionAssert.AreEqual(item1, reader.ReadSample()!.Data, "item 1, of two extents");
        CollectionAssert.AreEqual(item2, reader.ReadSample()!.Data, "item 2, in the 'idat'");
        Assert.IsNull(reader.ReadSample(), "past the last item");
        Assert.IsNull(reader.ReadSample(), "past the last item");
    }
}
