// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="AV2CodecConfigurationBox"/>, 'av2C' of the AV2 Codec ISO Media File Format
/// Binding (working group draft): a count, then OBUs each led by a leb128() length.
/// </summary>
[TestClass]
public class AV2CodecConfigurationBoxTests
{
    // Two OBUs: 3 bytes, then 2. The bytes stand for OBUs; only their lengths matter to the box.
    private static readonly byte[] Obus = [0x03, 0x08, 0x11, 0x22, 0x02, 0x40, 0x55];

    [TestMethod]
    public void SplitsItsOBUsByTheirLengths()
    {
        var box = Read(Build(Obus, countMinus1: 1));

        var obus = box.ConfigObus;
        Assert.AreEqual(2, obus.Count);
        CollectionAssert.AreEqual(new byte[] { 0x08, 0x11, 0x22 }, obus[0]);
        CollectionAssert.AreEqual(new byte[] { 0x40, 0x55 }, obus[1]);
    }

    /// <summary>A length need not be as short as it can be (leb128, AV2 4.11.6): it is kept as it was.</summary>
    [TestMethod]
    public void WritesBackAsItWasRead()
    {
        byte[] padded = [0x83, 0x00, 0x08, 0x11, 0x22]; // 3, in two bytes
        var original = Build(padded, countMinus1: 0);

        var container = new Container();
        using (var input = new MemoryStream(original))
            container.Read(new IsoStream(new StreamWrapper(input)));

        using var written = new MemoryStream();
        container.Write(new IsoStream(new StreamWrapper(written)));

        CollectionAssert.AreEqual(original, written.ToArray());
        CollectionAssert.AreEqual(new byte[] { 0x08, 0x11, 0x22 }, ((AV2CodecConfigurationBox)container.Children.Single()).ConfigObus.Single());
    }

    private static AV2CodecConfigurationBox Read(byte[] bytes)
    {
        var container = new Container();
        using var stream = new MemoryStream(bytes);
        container.Read(new IsoStream(new StreamWrapper(stream)));
        return (AV2CodecConfigurationBox)container.Children.Single();
    }

    private static byte[] Build(byte[] obus, byte countMinus1)
    {
        int size = 8 + 2 + obus.Length;
        return [0, 0, 0, (byte)size, (byte)'a', (byte)'v', (byte)'2', (byte)'C', 0, countMinus1, .. obus];
    }
}
