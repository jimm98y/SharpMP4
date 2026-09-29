using SharpISOBMFF;
using SharpMP4.Encryption;
using SharpMP4.Readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SharpMP4.Bench
{
    /// <summary>
    /// What reading slice headers costs with SharpH26X, as protecting a track does to leave each slice header in the
    /// clear (ISO/IEC 23001-7, 9.5.2): every sample of the file's video track split into its subsamples, in time and in
    /// allocation per sample and per NAL unit. The floor is walking the samples' NAL unit lengths and nothing more.
    /// </summary>
    internal static class Slices
    {
        public static void Run(string path)
        {
            Console.WriteLine("\n--- slice headers ---");

            var (entry, samples) = Read(path);
            if (entry == null)
            {
                Console.WriteLine("  no H.264, H.265 or H.266 track");
                return;
            }

            int lengthSize = SubsampleSplitter.For(entry).LengthSize;
            int units = samples.Sum(s => Units(s, lengthSize));
            Console.WriteLine($"  {IsoStream.ToFourCC(entry.FourCC)}: {samples.Count} samples, {units} NAL units");

            // the floor: the NAL unit lengths walked, nothing read
            Time("NAL unit lengths only", samples.Count, units, () =>
            {
                int total = 0;
                foreach (var sample in samples)
                    total += Units(sample, lengthSize);
                return total;
            });

            // a splitter made once, as a track makes it, the parameter sets of the sample entry taken in
            var splitter = SubsampleSplitter.For(entry);
            Time("SubsampleSplitter.Split (SharpH26X)", samples.Count, units, () =>
            {
                int total = 0;
                foreach (var sample in samples)
                    total += splitter.Split(sample, 0, sample.Length, wholeBlocks: false).Length;
                return total;
            });
        }

        private static (Box Entry, List<byte[]> Samples) Read(string path)
        {
            using var stream = File.OpenRead(path);
            var container = new Container();
            container.Read(new IsoStream(new StreamWrapper(stream)));
            var reader = new VideoReader();
            reader.Parse(container);

            foreach (var (trackID, track) in reader.Tracks)
            {
                var entry = track.Stbl.Children.OfType<SampleDescriptionBox>().Single().Children.First();
                if (SubsampleSplitter.For(entry) == null)
                    continue;

                var samples = new List<byte[]>();
                for (var sample = reader.ReadSample(trackID); sample != null; sample = reader.ReadSample(trackID))
                    samples.Add(sample.Data.ToArray());
                return (entry, samples);
            }
            return (null, null);
        }

        private static int Units(byte[] sample, int lengthSize)
        {
            int count = 0;
            for (int position = 0; position + lengthSize <= sample.Length; count++)
            {
                int length = 0;
                for (int i = 0; i < lengthSize; i++)
                    length = length << 8 | sample[position + i];
                position += lengthSize + length;
            }
            return count;
        }

        private static void Time(string label, int samples, int units, Func<int> action)
        {
            action(); // warm up

            long before = GC.GetAllocatedBytesForCurrentThread();
            action();
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            var timings = new List<double>();
            for (int i = 0; i < 20; i++)
            {
                var sw = Stopwatch.StartNew();
                action();
                timings.Add(sw.Elapsed.TotalMilliseconds);
            }
            timings.Sort();

            double best = timings[0];
            Console.WriteLine($"  {label,-38} {best,8:F2} ms  {best * 1e6 / units,7:F0} ns/NAL  {allocated / units,6:N0} B/NAL  {allocated / samples,7:N0} B/sample");
        }
    }
}
