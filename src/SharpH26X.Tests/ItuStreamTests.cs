namespace SharpH26X.Tests;

/// <summary>
/// Tests against <see cref="ItuStream"/>.
/// </summary>
public class ItuStreamTests
{
    class BogusSerializable : IItuSerializable
    {
        public int HasMoreRbspData { get; set; } = 1;
        public int[] ReadNextBits { get; set; } = [];

        public ulong Read(IItuContext context, ItuStream stream)
        {
            throw new NotImplementedException();
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void TestReadWriteBits()
    {
        using var memoryStream = new MemoryStream();
        var ituStream = new ItuStream(memoryStream);

        ituStream.WriteBits(8, 120, "peep");
        ituStream.WriteBits(4, 12, "peep4");
        ituStream.WriteBits(5, 0, "filler");

        memoryStream.Seek(0, SeekOrigin.Begin);
        
        ituStream = new ItuStream(memoryStream);

        ituStream.ReadUnsignedIntVariable(0uL, 8uL, out uint peep, "peep");
        ituStream.ReadUnsignedIntVariable(0uL, 4uL, out uint peep4, "peep4");
        
        Assert.Equal(120u, peep);
        Assert.Equal(12u, peep4);
    }

    [Fact]
    public void TestReadNextBits()
    {
        using var memoryStream = new MemoryStream();
        var ituStream = new ItuStream(memoryStream);

        ituStream.WriteBits(8, 120, "peep");
        ituStream.WriteBits(4, 12, "peep4");
        ituStream.WriteBits(5, 0, "filler");

        memoryStream.Seek(0, SeekOrigin.Begin);

        ituStream = new ItuStream(memoryStream);

        var mySerializable = new BogusSerializable();

        ituStream.ReadUnsignedIntVariable(0uL, 8uL, out uint peep, "peep");
        
        ituStream.ReadNextBits(mySerializable, 4);
        ituStream.ReadNextBits(mySerializable, 4);
        ituStream.ReadNextBits(mySerializable, 4);
        ituStream.ReadNextBits(mySerializable, 4);
        ituStream.ReadNextBits(mySerializable, 4);
        int lastSerializable = ituStream.ReadNextBits(mySerializable, 4);

        Assert.Equal(120u, peep);
        Assert.Equal(12, lastSerializable);
    }
}
