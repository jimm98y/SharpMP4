namespace SharpH26X.Tests;

/// <summary>
/// Tests against <see cref="ItuStream"/>.
/// </summary>
public class ItuStreamTests
{
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
}
