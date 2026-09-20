using SharpH264;
using SharpH26X;
using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SharpMP4.Bench
{
    /// <summary>
    /// Feeds corrupted input to the parsers and records what comes back. A parser facing a
    /// malformed file should raise a defined error quickly; anything else - running out of memory,
    /// running away, or an exception that says nothing about the input - is a problem when the file
    /// comes from somewhere untrusted.
    /// </summary>
    internal static class Fuzz
    {
        /// <summary>An allocation this large out of one NAL unit is worth printing on its own.</summary>
        private const long ReportAbove = 200L * 1024 * 1024;

        public static void Run(byte[] file, List<byte[]> nalUnits, int iterations)
        {
            Console.WriteLine($"\n--- fuzzing NAL units ({nalUnits.Count} seeds, {iterations} iterations) ---");
            FuzzNalUnits(nalUnits, iterations);

            Console.WriteLine($"\n--- fuzzing the container ({iterations / 2} iterations) ---");
            FuzzContainer(file, iterations / 2);
        }

        private static void FuzzNalUnits(List<byte[]> nalUnits, int iterations)
        {
            var random = new Random(12345);
            var outcomes = new Dictionary<string, int>();
            long worstAllocation = 0;
            double worstMs = 0;
            string worstCase = null;
            byte[] worstInput = null;
            int reported = 0;

            for (int i = 0; i < iterations; i++)
            {
                var data = Corrupt(random, nalUnits[random.Next(nalUnits.Count)]);

                // A slice header is read against the parameter sets in force, so a context
                // without them fails for its own reason and tests nothing about the input.
                var context = new H264Context();
                Prime(context, nalUnits);

                long before = GC.GetAllocatedBytesForCurrentThread();
                var sw = Stopwatch.StartNew();
                string outcome;
                try
                {
                    Parse(context, data);
                    outcome = "parsed (no error)";
                }
                catch (Exception ex)
                {
                    outcome = ex.GetType().Name;
                }
                sw.Stop();
                long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

                Bump(outcomes, outcome);
                if (allocated > ReportAbove && reported < 3)
                {
                    reported++;
                    Console.WriteLine($"    >>> {allocated / (1024.0 * 1024.0):F0} MB allocated out of a " +
                        $"{data.Length} byte NAL unit, {outcome}, {sw.Elapsed.TotalMilliseconds:F0} ms");
                }
                if (allocated > worstAllocation)
                {
                    worstAllocation = allocated;
                    worstCase = outcome;
                    worstInput = data;
                }
                worstMs = Math.Max(worstMs, sw.Elapsed.TotalMilliseconds);
            }

            Report(outcomes, iterations);
            Console.WriteLine($"  worst single-parse allocation: {worstAllocation / (1024.0 * 1024.0):F1} MB ({worstCase})");
            Console.WriteLine($"  slowest single parse:          {worstMs:F0} ms");
            Save(worstInput, "worst-nalu.bin");
        }

        /// <summary>
        /// Keeps the input that provoked the worst allocation, so it can be looked at rather than
        /// guessed at. The seeds are fixed, so a saved case reproduces.
        /// </summary>
        private static void Save(byte[] data, string name)
        {
            if (data == null)
                return;

            string path = Path.Combine(Path.GetTempPath(), name);
            File.WriteAllBytes(path, data);
            Console.WriteLine($"  worst input saved to {path}");
        }

        /// <summary>
        /// Two corruption shapes. Bit flips explore the syntax; runs of zeroes are what truncation
        /// and zero fill look like in practice, and they are also what makes an Exp-Golomb value
        /// enormous - a long run of leading zero bits encodes a huge number, and several of those
        /// values are used directly as array lengths.
        /// </summary>
        private static byte[] Corrupt(Random random, byte[] source)
        {
            var data = (byte[])source.Clone();
            if (data.Length < 4)
                return data;

            if (random.Next(2) == 0)
            {
                int flips = 1 + random.Next(3);
                for (int f = 0; f < flips; f++)
                {
                    int index = 1 + random.Next(Math.Min(48, data.Length - 1));
                    data[index] ^= (byte)(1 << random.Next(8));
                }
            }
            else
            {
                int start = 1 + random.Next(Math.Min(32, data.Length - 1));
                int run = 1 + random.Next(8);
                for (int z = 0; z < run && start + z < data.Length; z++)
                    data[start + z] = 0x00;
            }

            return data;
        }

        /// <summary>
        /// Parses the real parameter sets into a fresh context, the way a player would have them
        /// by the time a slice arrives. Each iteration gets its own, since parsing mutates it.
        /// </summary>
        private static void Prime(H264Context context, List<byte[]> nalUnits)
        {
            foreach (var nalUnit in nalUnits)
            {
                if (nalUnit.Length == 0)
                    continue;

                uint type = (uint)(nalUnit[0] & 0x1F);
                if (type != H264NALTypes.SPS && type != H264NALTypes.PPS)
                    continue;

                try { Parse(context, nalUnit); }
                catch (Exception) { }   // a parameter set that will not parse simply is not used
            }
        }

        /// <summary>
        /// Reads one NAL unit the way a player would: the header, then whichever payload the type
        /// calls for. Only the types that carry parsed syntax are worth the trouble.
        /// </summary>
        private static void Parse(H264Context context, byte[] data)
        {
            using var stream = new ItuStream(new MemoryStream(data));

            var nalUnit = new NalUnit((uint)data.Length);
            context.NalHeader = nalUnit;
            nalUnit.Read(context, stream);

            switch (nalUnit.NalUnitType)
            {
                case H264NALTypes.SPS:
                    context.SeqParameterSetRbsp = new SeqParameterSetRbsp();
                    context.SeqParameterSetRbsp.Read(context, stream);
                    break;

                case H264NALTypes.PPS:
                    context.PicParameterSetRbsp = new PicParameterSetRbsp();
                    context.PicParameterSetRbsp.Read(context, stream);
                    break;

                case H264NALTypes.SEI:
                    context.SeiRbsp = new SeiRbsp();
                    context.SeiRbsp.Read(context, stream);
                    break;

                case H264NALTypes.SLICE:
                case H264NALTypes.IDR_SLICE:
                    context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                    context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                    break;
            }
        }

        private static void FuzzContainer(byte[] original, int iterations)
        {
            var random = new Random(999);
            var outcomes = new Dictionary<string, int>();
            long worstAllocation = 0;
            double worstMs = 0;
            byte[] worstInput = null;

            // What an untouched parse of this file costs, so the fuzzed figures have something to
            // be read against.
            long cleanBefore = GC.GetAllocatedBytesForCurrentThread();
            var clean = new Container();
            clean.Read(new IsoStream(new StreamWrapper(new MemoryStream(original))));
            Console.WriteLine($"  the file as it stands parses with " +
                $"{(GC.GetAllocatedBytesForCurrentThread() - cleanBefore) / (1024.0 * 1024.0):F1} MB allocated");

            // Aim at structure rather than at the media: the front of the file, and the tail where
            // the moov usually sits.
            int tail = Math.Min(original.Length, 32 * 1024);
            int tailStart = original.Length - tail;

            for (int i = 0; i < iterations; i++)
            {
                var data = (byte[])original.Clone();
                int flips = 1 + random.Next(4);
                for (int f = 0; f < flips; f++)
                {
                    int index = random.Next(2) == 0
                        ? random.Next(Math.Min(4096, data.Length))
                        : tailStart + random.Next(tail);
                    data[index] ^= (byte)(1 << random.Next(8));
                }

                long before = GC.GetAllocatedBytesForCurrentThread();
                var sw = Stopwatch.StartNew();
                string outcome;
                try
                {
                    var container = new Container();
                    container.Read(new IsoStream(new StreamWrapper(new MemoryStream(data))));
                    outcome = "parsed (no error)";
                }
                catch (Exception ex)
                {
                    outcome = ex.GetType().Name;
                }
                sw.Stop();

                long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

                Bump(outcomes, outcome);
                if (allocated > worstAllocation)
                {
                    worstAllocation = allocated;
                    worstInput = data;
                }
                worstMs = Math.Max(worstMs, sw.Elapsed.TotalMilliseconds);
            }

            Report(outcomes, iterations);
            Console.WriteLine($"  worst single-parse allocation: {worstAllocation / (1024.0 * 1024.0):F1} MB");
            Console.WriteLine($"  slowest single parse:          {worstMs:F0} ms");
            Save(worstInput, "worst-container.mp4");
        }

        private static void Bump(Dictionary<string, int> map, string key)
        {
            map.TryGetValue(key, out int count);
            map[key] = count + 1;
        }

        private static void Report(Dictionary<string, int> outcomes, int total)
        {
            foreach (var entry in outcomes.OrderByDescending(e => e.Value))
                Console.WriteLine($"    {entry.Value,6} ({100.0 * entry.Value / total,5:F1}%)  {entry.Key}");
        }
    }
}
