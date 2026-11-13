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
/// Tests against <see cref="PixelInformationProperty"/>, 'pixi' as ISO/IEC 23008-12:2024/CDAM 2:2025 6.5.6.3
/// has it: with the least significant bit of its flags set, each channel's kind, format, subsampling and
/// label follow the bits of each.
/// </summary>
[TestClass]
public class PixelInformationPropertyTests
{
    // 'pixi', version 0, flags 1: 2 channels of 10 bits; the first colour, 4:4:4 at location 0 and no label;
    // the second alpha (channel_idc 1), not subsampled, labelled "a"
    private static readonly byte[] Extended =
    [
        0x00, 0x00, 0x00, 0x14, (byte)'p', (byte)'i', (byte)'x', (byte)'i', 0x00, 0x00, 0x00, 0x01,
        0x02, 0x0A, 0x0A,
        0b000_0_00_1_0, 0x00,              // channel_idc 0, reserved, component_format 0, subsampling_flag 1, channel_label_flag 0; type 0, location 0
        0b001_0_00_0_1, (byte)'a', 0x00,   // channel_idc 1, subsampling_flag 0, channel_label_flag 1; "a"
    ];

    [TestMethod]
    public void ReadsEachChannelWhereItsFlagsSaySo()
    {
        var pixi = Read(Extended);

        Assert.AreEqual(1u, pixi.Flags);
        CollectionAssert.AreEqual(new byte[] { 10, 10 }, pixi.BitsPerChannel);
        CollectionAssert.AreEqual(new byte[] { 0, 1 }, pixi.ChannelIdc);
        CollectionAssert.AreEqual(new[] { true, false }, pixi.SubsamplingFlag);
        CollectionAssert.AreEqual(new[] { false, true }, pixi.ChannelLabelFlag);
    }

    [TestMethod]
    public void WritesBackAsItWasRead()
    {
        var container = new Container();
        using (var input = new MemoryStream(Extended))
            container.Read(new IsoStream(new StreamWrapper(input)));

        using var written = new MemoryStream();
        container.Write(new IsoStream(new StreamWrapper(written)));

        CollectionAssert.AreEqual(Extended, written.ToArray());
    }

    /// <summary>Its size, as a box made rather than read is written with it, counts each channel's fields.</summary>
    [TestMethod]
    public void CalculatesItsSize()
    {
        var pixi = Read(Extended);

        Assert.AreEqual((ulong)Extended.Length * 8, pixi.CalculateSize());
    }

    private static PixelInformationProperty Read(byte[] bytes)
    {
        var container = new Container();
        using var stream = new MemoryStream(bytes);
        container.Read(new IsoStream(new StreamWrapper(stream)));
        return (PixelInformationProperty)container.Children.Single();
    }
}
