using SharpISOBMFF;
using static SharpMP4.Tests.TrackTestSupport;

namespace SharpMP4.Tests;

/// <summary>
/// The boxes of an iPhone's spatial photo that say what its two eyes are, made here byte by byte as one has them: the
/// camera extrinsic matrix, of ISO/IEC 23008-12 ('cmex') and of Apple's uuid, of each flag's fields; and Apple's entity
/// group of the eyes and the primary image. Each is read as it is, and written back the same.
/// </summary>
[TestClass]
public class SpatialPhotoBoxesTests
{
    private const string AppleExtrinsicUuid = "4363e9145b7d4aab97aebea69803b434";
    private const string AppleEntityGroupUuid = "524a52f437b347d09637933d05ac9b68";

    private static byte[] Box(string type, string body)
    {
        byte[] content = Hex(body);
        return Hex((8 + content.Length).ToString("x8") + Convert.ToHexString(System.Text.Encoding.ASCII.GetBytes(type))).Concat(content).ToArray();
    }

    private static byte[] Uuid(string uuid, string body) => Box("uuid", uuid + body);

    /// <summary>The boxes read, and the bytes they write back.</summary>
    private static (Container Container, byte[] Written) ReadAndWrite(byte[] bytes)
    {
        var container = new Container();
        container.Read(new IsoStream(new StreamWrapper(new MemoryStream(bytes))));
        var output = new MemoryStream();
        container.Write(new IsoStream(new StreamWrapper(output)));
        return (container, output.ToArray());
    }

    /// <summary>
    /// The extrinsics of an iPhone's two eyes, as IMG_9009.HEIC has them: of the first nothing but the flag of 32 bit
    /// rotations, the reference; of the second its position along x, -19275 micrometres, the baseline of the two cameras.
    /// </summary>
    [TestMethod]
    public void ReadsApplesExtrinsicsOfTheTwoEyes()
    {
        byte[] bytes = Uuid(AppleExtrinsicUuid, "00000010").Concat(Uuid(AppleExtrinsicUuid, "00000011" + "ffffb4b5")).ToArray();
        var (container, written) = ReadAndWrite(bytes);

        var eyes = container.Children.OfType<UuidCameraExtrinsicMatrixBox>().ToArray();
        Assert.AreEqual(2, eyes.Length);
        Assert.AreEqual(0, eyes[0].Posx);
        Assert.AreEqual(-19275, eyes[1].Posx);
        CollectionAssert.AreEqual(bytes, written);
    }

    /// <summary>
    /// 'cmex' of every field: a position of x, y and z, an orientation of 16 bit components or of 32, and the world
    /// coordinate system's ID, each of its flag.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ReadsACameraExtrinsicMatrixOfEachFlag(bool largeRotation)
    {
        string rotation = largeRotation ? "40000000" + "c0000000" + "00000001" : "4000" + "c000" + "0001";
        byte[] bytes = Box("cmex", (largeRotation ? "0000003f" : "0000002f") + "00000064" + "ffffff9c" + "00000032" + rotation + "00000007");
        var (container, written) = ReadAndWrite(bytes);

        var cmex = container.Children.OfType<CameraExtrinsicMatrixBox>().Single();
        Assert.AreEqual((100, -100, 50), (cmex.Posx, cmex.Posy, cmex.Posz));
        if (largeRotation)
            Assert.AreEqual((0x40000000, unchecked((int)0xc0000000), 1), (cmex.Quatx, cmex.Quaty, cmex.Quatz));
        else
            Assert.AreEqual(((short)0x4000, unchecked((short)0xc000), (short)1), (cmex.QuatxShort, cmex.QuatyShort, cmex.QuatzShort));
        Assert.AreEqual(7u, cmex.WorldCoordinateSystemId);
        CollectionAssert.AreEqual(bytes, written);
    }

    /// <summary>Apple's entity group in a 'grpl', of the layout of an EntityToGroupBox: group 125 of the eyes and the primary image.</summary>
    [TestMethod]
    public void ReadsApplesEntityGroupOfTheEyes()
    {
        byte[] group = Uuid(AppleEntityGroupUuid, "00000002" + "0000007d" + "00000003" + "0000007a" + "0000005f" + "0000002e");
        byte[] stereo = Box("ster", "00000000" + "0000007e" + "00000002" + "0000007a" + "0000005f");
        byte[] bytes = Box("grpl", Convert.ToHexString(group) + Convert.ToHexString(stereo));
        var (container, written) = ReadAndWrite(bytes);

        var grpl = container.Children.OfType<GroupsListBox>().Single();
        var apple = grpl.Children.OfType<UuidEntityToGroupBox>().Single();
        Assert.AreEqual(125u, apple.GroupId);
        CollectionAssert.AreEqual(new uint[] { 122, 95, 46 }, apple.EntityId);
        CollectionAssert.AreEqual(new uint[] { 122, 95 }, grpl.Children.OfType<StereoEntityGroupBox>().Single().EntityId);
        CollectionAssert.AreEqual(bytes, written);
    }
}
