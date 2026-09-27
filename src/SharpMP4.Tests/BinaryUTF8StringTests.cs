using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="BinaryUTF8String.Text"/>, which reads a handler name in whichever form a
/// writer left it: zero terminated as ISOBMFF has it, or counted as QuickTime has it.
/// </summary>
[TestClass]
public class BinaryUTF8StringTests
{
    [TestMethod]
    public void ReadsAZeroTerminatedName()
    {
        Assert.AreEqual("VideoHandler", Text("VideoHandler\0"));
    }

    [TestMethod]
    public void ReadsACountedName()
    {
        Assert.AreEqual("Apple Alias Data Handler", Text("\x18" + "Apple Alias Data Handler"));
    }

    /// <summary>As mp4-with-mov-in24-ver.mp4 has it: a counted name, then zeros.</summary>
    [TestMethod]
    public void ReadsACountedNameWithZerosAfterIt()
    {
        Assert.AreEqual("Apple Alias Data Handler", Text("\x18" + "Apple Alias Data Handler\0\0\0"));
    }

    /// <summary>As Apple's afconvert writes it: nothing, and a zero more than it needs.</summary>
    [TestMethod]
    public void ReadsAnEmptyNameWithAZeroAfterIt()
    {
        Assert.AreEqual("", Text("\0\0"));
    }

    /// <summary>
    /// A zero terminated name whose first letter, read as a count, would fit: 'A' is 65, and the name is
    /// longer than that. It has no zero where a count of 65 would end it, so it is not counted.
    /// </summary>
    [TestMethod]
    public void DoesNotTakeALongNameForACountedOne()
    {
        string name = "A" + new string('x', 80);
        Assert.AreEqual(name, Text(name + "\0"));
    }

    [TestMethod]
    public void KeepsTheBytesAsTheyWere()
    {
        byte[] bytes = [0x00, 0xB0];
        Assert.AreSame(bytes, new BinaryUTF8String(bytes).Bytes);
    }

    private static string Text(string raw)
    {
        return new BinaryUTF8String(raw.Select(c => (byte)c).ToArray()).Text;
    }
}
