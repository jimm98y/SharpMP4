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

using SharpMP4.Tracks;

namespace SharpMP4.Tests;

/// <summary>
/// Tests on <see cref="TrackBase.ParseSample(byte[], int, int, List{ArraySegment{byte}})"/>: the units of a sample added to
/// the caller's list - those the enumerable has - of nothing allocated of each sample, where a track parses as the base does
/// or as H.264 and H.265 do.
/// </summary>
[TestClass]
public class TrackParseIntoListTests
{
    // two NAL units of 4 byte lengths, of 3 and 2 bytes, at an offset of 5 into the buffer
    private static readonly byte[] TwoNalUnits = [9, 9, 9, 9, 9, 0, 0, 0, 3, 0x65, 1, 2, 0, 0, 0, 2, 0x41, 3];

    private static void AssertSameUnits(TrackBase track, byte[] buffer, int offset, int length)
    {
        var expected = track.ParseSample(buffer, offset, length).ToList();
        var units = new List<ArraySegment<byte>>();
        track.ParseSample(buffer, offset, length, units);
        Assert.AreEqual(expected.Count, units.Count, track.GetType().Name);
        for (int i = 0; i < expected.Count; i++)
            Assert.AreEqual((expected[i].Array, expected[i].Offset, expected[i].Count), (units[i].Array, units[i].Offset, units[i].Count), $"{track.GetType().Name} unit {i}");
    }

    [TestMethod]
    public void TheUnitsAreThoseOfTheEnumerable()
    {
        AssertSameUnits(new H264Track(), TwoNalUnits, 5, TwoNalUnits.Length - 5);
        AssertSameUnits(new AACTrack(2, 48000, 16), TwoNalUnits, 5, TwoNalUnits.Length - 5);
        // of a track parsing of its own enumerable alone: two temporal delimiters of size fields
        AssertSameUnits(new AV1Track(), [0x12, 0x00, 0x12, 0x00], 0, 4);
    }

#if NET
    [TestMethod]
    public void NothingIsAllocatedOfEachSample()
    {
        foreach (TrackBase track in new TrackBase[] { new H264Track(), new AACTrack(2, 48000, 16) })
        {
            var units = new List<ArraySegment<byte>>();
            track.ParseSample(TwoNalUnits, 5, TwoNalUnits.Length - 5, units);
            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 1000; i++)
            {
                units.Clear();
                track.ParseSample(TwoNalUnits, 5, TwoNalUnits.Length - 5, units);
            }
            Assert.AreEqual(0L, GC.GetAllocatedBytesForCurrentThread() - before, track.GetType().Name);
        }
    }
#endif
}
