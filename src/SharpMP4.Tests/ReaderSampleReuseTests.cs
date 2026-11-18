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

using SharpMP4.Readers;

namespace SharpMP4.Tests;

/// <summary>
/// Tests on reading samples into one of the caller's - <see cref="VideoReader.ReadSample(uint, MediaSample)"/> and
/// <see cref="VideoReader.ReadSamples(uint, int, MediaSample)"/> - which a player reads sample after sample into, rather
/// than allocating one for each.
/// </summary>
[TestClass]
public class ReaderSampleReuseTests
{
    private static byte[][] Data(int count) =>
        Enumerable.Range(0, count).Select(i => Enumerable.Range(0, 50 + i * 13).Select(b => (byte)(b * 7 + i)).ToArray()).ToArray();

    private static byte[] Build(string kind) => kind switch
    {
        "plain" => ReaderSampleTableTests.BuildFile(12, i => 50 + i * 13, i => i % 3 == 1 ? 40 : 0, i => i % 4 == 0).File,
        "fragmented" => ReaderSampleTableTests.BuildFragmentedFile(Data(12), 100), // a fragment of every few samples
        _ => ReaderSampleTableTests.BuildFileWithSamplesInMoovAndFragments(4, 8).File,
    };

    private static VideoReader Open(byte[] file, out uint trackID)
    {
        var reader = new VideoReader();
        reader.Parse(ReaderSampleTableTests.Read(file));
        trackID = reader.Tracks.Keys.Single();
        return reader;
    }

    private static void AssertSameSample(MediaSample expected, MediaSample actual, string what)
    {
        CollectionAssert.AreEqual(expected.Data.ToArray(), actual.Data.ToArray(), $"{what}: data");
        Assert.AreEqual((expected.PTS, expected.DTS, expected.LongDuration, expected.IsRandomAccessPoint, expected.Offset),
            (actual.PTS, actual.DTS, actual.LongDuration, actual.IsRandomAccessPoint, actual.Offset), what);
        Assert.AreSame(expected.Encryption, actual.Encryption, $"{what}: encryption");
    }

    /// <summary>
    /// Read into one sample, the samples of a file are those read into new ones, every property of each: of a file not
    /// fragmented, of one fragmented, and of one of both - the samples of its 'moov', then of its fragments, into the same.
    /// After the last, nothing is read, and the sample is as it was.
    /// </summary>
    [TestMethod]
    [DataRow("plain")]
    [DataRow("fragmented")]
    [DataRow("both")]
    public void ReadsIntoOneSampleWhatItReadsIntoNewOnes(string kind)
    {
        byte[] file = Build(kind);
        var fresh = Open(file, out uint trackID);
        var reused = Open(file, out _);
        var sample = new MediaSample();

        int count = 0;
        for (var expected = fresh.ReadSample(trackID); expected != null; expected = fresh.ReadSample(trackID), count++)
        {
            Assert.IsTrue(reused.ReadSample(trackID, sample), $"sample {count}");
            AssertSameSample(expected, sample, $"sample {count}");
        }
        Assert.IsTrue(count >= 8, "samples");

        var last = (sample.PTS, sample.DTS, sample.Data.Count);
        Assert.IsFalse(reused.ReadSample(trackID, sample), "a sample after the last");
        Assert.AreEqual(last, (sample.PTS, sample.DTS, sample.Data.Count), "the sample after the last");
    }

    /// <summary>
    /// Runs read as one and samples read alone, in turn, into the same sample, are those read into new ones: what a sample
    /// of the one had - the offset of a sample alone in the file, which a run has none of - is not left in the other.
    /// </summary>
    [TestMethod]
    public void ReadsRunsAndSamplesInTurnIntoOneSample()
    {
        byte[] file = ReaderSampleTableTests.BuildFile(30, _ => 64, _ => 0, _ => true).File;
        var fresh = Open(file, out uint trackID);
        var reused = Open(file, out _);
        var sample = new MediaSample();

        for (int i = 0; ; i++)
        {
            bool run = i % 2 == 0;
            var expected = run ? fresh.ReadSamples(trackID, 4) : fresh.ReadSample(trackID);
            bool read = run ? reused.ReadSamples(trackID, 4, sample) : reused.ReadSample(trackID, sample);
            Assert.AreEqual(expected != null, read, $"read {i}");
            if (expected == null)
                break;
            AssertSameSample(expected, sample, $"read {i}");
            if (run && i < 6)
                Assert.AreEqual(-1, sample.Offset, $"read {i}: a run's offset");
        }
    }

    /// <summary>A sample to read into is null: it is not read into, it is refused.</summary>
    [TestMethod]
    public void RefusesNoSampleToReadInto()
    {
        var reader = Open(Build("plain"), out uint trackID);
        Assert.ThrowsExactly<ArgumentNullException>(() => reader.ReadSample(trackID, null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => reader.ReadSamples(trackID, 4, null!));
    }

#if NET
    /// <summary>
    /// Read into one sample, the samples of a file not fragmented allocate nothing, once the track's buffer is made: a
    /// player reads sample after sample without garbage.
    /// </summary>
    [TestMethod]
    public void AllocatesNothingASampleReadIntoOne()
    {
        byte[] file = ReaderSampleTableTests.BuildFile(200, i => 100 + i % 7, i => i % 3 == 1 ? 40 : 0, i => i % 10 == 0).File;
        var reader = Open(file, out uint trackID);
        var sample = new MediaSample();
        Assert.IsTrue(reader.ReadSample(trackID, sample), "the first sample, of the buffer made for them all");

        int count = 1;
        long before = GC.GetAllocatedBytesForCurrentThread();
        while (reader.ReadSample(trackID, sample))
            count++;
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.AreEqual(200, count, "samples");
        Assert.AreEqual(0, allocated, $"bytes allocated reading {count - 1} samples");
    }
#endif
}
