using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SharpMP4.Bench
{
    /// <summary>
    /// Timing and fuzzing harness. It runs against a file committed to the repo, so it needs
    /// nothing from outside; pass a path to point it at something else.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            string command = args.Length > 0 ? args[0].ToLowerInvariant() : "all";
            string path = args.Length > 1 ? args[1] : Sample.DefaultPath;
            int iterations = args.Length > 2 ? int.Parse(args[2]) : 4000;

            if (command == "help" || command == "--help" || command == "-h")
            {
                Console.WriteLine("Usage: SharpMP4.Bench [all|bits|parse|fuzz] [file.mp4] [iterations]");
                return 0;
            }

            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"{path} not found");
                return 2;
            }

            if (command == "bits")
            {
                Bits.Run();
                return 0;
            }

            var file = File.ReadAllBytes(path);
            var nalUnits = Sample.ReadNalUnits(file, out uint trackId);
            Console.WriteLine($"{Path.GetFileName(path)}: {file.Length / 1024} KB, track {trackId}, " +
                $"{nalUnits.Count} NAL units, {nalUnits.Sum(n => (long)n.Length) / 1024} KB of them");

            if (nalUnits.Count == 0)
            {
                Console.Error.WriteLine("no video NAL units found");
                return 2;
            }

            switch (command)
            {
                case "parse":
                    Parse(file);
                    break;

                case "fuzz":
                    Fuzz.Run(file, nalUnits, iterations);
                    break;

                case "all":
                    Parse(file);
                    Bits.Run();
                    Fuzz.Run(file, nalUnits, iterations);
                    break;

                default:
                    Console.Error.WriteLine($"Unknown command '{command}'.");
                    return 1;
            }

            return 0;
        }

        private static void Parse(byte[] file)
        {
            Console.WriteLine("\n--- parsing ---");

            Time("Container.Read (whole box tree)", 5, () =>
            {
                var container = new Container();
                container.Read(new IsoStream(new StreamWrapper(new MemoryStream(file))));
                return container.Children.Count;
            });

            Time("Demux (box tree + every sample)", 5, () => Sample.ReadNalUnits(file, out _).Count);

            long before = GC.GetAllocatedBytesForCurrentThread();
            int count = Sample.ReadNalUnits(file, out _).Count;
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            Console.WriteLine($"  allocation per NAL unit demuxed: {allocated / count:N0} bytes");
        }

        private static void Time(string label, int iterations, Func<int> action)
        {
            action();   // warm up, so JIT and the file cache are not what is being measured

            var timings = new List<double>();
            for (int i = 0; i < iterations; i++)
            {
                var sw = Stopwatch.StartNew();
                action();
                sw.Stop();
                timings.Add(sw.Elapsed.TotalMilliseconds);
            }

            timings.Sort();
            Console.WriteLine($"  {label,-40} {timings[0],7:F1} ms  (median {timings[timings.Count / 2]:F1})");
        }
    }
}
