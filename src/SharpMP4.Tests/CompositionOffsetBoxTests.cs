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
/// Tests against <see cref="CompositionOffsetBox"/>, the box that records how far a sample's
/// composition time sits from its decode time.
/// </summary>
[TestClass]
public class CompositionOffsetBoxTests
{
    /// <summary>
    /// Version 1 stores the offsets signed. Reading one used to throw, because only the offsets
    /// were allocated and not the counts beside them, so any file carrying a version 1 ctts - what
    /// ffmpeg writes with -movflags +negative_cts_offsets - could not be opened at all.
    /// </summary>
    [TestMethod]
    public void ReadsVersion1WithNegativeOffsets()
    {
        var box = ReadBox(BuildCtts(version: 1, entries: [(3u, 0), (1u, -40), (2u, 120)]));

        Assert.AreEqual<byte>(1, box.Version);
        Assert.AreEqual(3u, box.EntryCount);
        CollectionAssert.AreEqual(new uint[] { 3, 1, 2 }, box.SampleCount);
        CollectionAssert.AreEqual(new[] { 0, -40, 120 }, box.SampleOffset0);
    }

    /// <summary>Version 0 stores the offsets unsigned, and was always readable.</summary>
    [TestMethod]
    public void ReadsVersion0()
    {
        var box = ReadBox(BuildCtts(version: 0, entries: [(5u, 0), (2u, 60)]));

        Assert.AreEqual<byte>(0, box.Version);
        CollectionAssert.AreEqual(new uint[] { 5, 2 }, box.SampleCount);
        CollectionAssert.AreEqual(new uint[] { 0, 60 }, box.SampleOffset);
    }

    [TestMethod]
    [DataRow((byte)0)]
    [DataRow((byte)1)]
    public void RoundTripsThroughWrite(byte version)
    {
        var original = BuildCtts(version, entries: [(4u, 0), (1u, version == 1 ? -20 : 20)]);

        var box = ReadBox(original);
        using var written = new MemoryStream();
        box.Write(new IsoStream(new StreamWrapper(written)));

        // The box is read from just past its header, so writing it back produces the body alone.
        CollectionAssert.AreEqual(original.Skip(8).ToArray(), written.ToArray());
    }

    private static CompositionOffsetBox ReadBox(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        var iso = new IsoStream(new StreamWrapper(stream));

        // Skip the size and type the box header carries, and read what follows.
        var box = new CompositionOffsetBox();
        iso.SeekFromBeginning(8);
        box.Read(iso, (ulong)(bytes.Length - 8) * 8);
        return box;
    }

    /// <summary>Builds a ctts by hand, so the test does not depend on the writer it is checking.</summary>
    private static byte[] BuildCtts(byte version, (uint Count, int Offset)[] entries)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);

        WriteBigEndian(writer, (uint)(16 + entries.Length * 8));
        writer.Write(new[] { (byte)'c', (byte)'t', (byte)'t', (byte)'s' });
        writer.Write(version);
        writer.Write(new byte[3]); // flags
        WriteBigEndian(writer, (uint)entries.Length);

        foreach (var (count, offset) in entries)
        {
            WriteBigEndian(writer, count);
            WriteBigEndian(writer, unchecked((uint)offset));
        }

        writer.Flush();
        return memory.ToArray();
    }

    private static void WriteBigEndian(BinaryWriter writer, uint value)
    {
        writer.Write((byte)(value >> 24));
        writer.Write((byte)(value >> 16));
        writer.Write((byte)(value >> 8));
        writer.Write((byte)value);
    }
}
