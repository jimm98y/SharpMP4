using SharpH26X;
using System;
using System.Diagnostics;
using System.IO;

namespace SharpMP4.Bench
{
    /// <summary>
    /// Measures the bit reader, which every H.26x parser sits on top of. The reference reader at
    /// the bottom says how much of the cost is inherent and how much is the stream access around
    /// it - without it, a number on its own says nothing about whether it can be improved.
    /// </summary>
    internal static class Bits
    {
        private const int Passes = 4;
        private const int ElementsPerPass = 100_000;

        public static void Run()
        {
            Console.WriteLine("\n--- bit reader ---");

            var buffer = new byte[1 << 20];
            new Random(1).NextBytes(buffer);

            // The emulation prevention scan looks at every byte for a 00 00 03 sequence, so the
            // difference between these two is what that scan costs.
            foreach (bool skipPrevention in new[] { true, false })
            {
                double megabits = Measure(buffer, skipPrevention, logging: false, out double nsPerBit);
                Console.WriteLine($"  ReadUnsignedInt(8), skipPrevention={skipPrevention,-5}: " +
                    $"{megabits,6:F0} Mbit/s ({nsPerBit:F2} ns per bit)");
            }

            // Both log helpers used to build their message before asking whether it would be
            // logged, and that formatting, not the reading, was where the time went. The logger
            // discards what it is given, so this is the cost of building the message alone.
            foreach (bool logging in new[] { true, false })
            {
                double megabits = Measure(buffer, skipPrevention: true, logging: logging, out double nsPerBit);
                Console.WriteLine($"  logging={logging,-5} (messages discarded either way):   " +
                    $"{megabits,6:F0} Mbit/s ({nsPerBit:F2} ns per bit)");
            }

            double reference = Reference(buffer, out double referenceNs);
            Console.WriteLine($"  reference accumulator reader:            " +
                $"{reference,6:F0} Mbit/s ({referenceNs:F2} ns per bit)");
        }

        private static double Measure(byte[] buffer, bool skipPrevention, bool logging, out double nsPerBit)
        {
            var logger = new DiscardingLogger(logging);

            var sw = Stopwatch.StartNew();
            long bits = 0;
            for (int pass = 0; pass < Passes; pass++)
            {
                using var stream = new ItuStream(new MemoryStream(buffer), logger);
                stream.Bitstream.SkipPreventionBytes = skipPrevention;
                ulong size = 0;
                for (int i = 0; i < ElementsPerPass; i++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out byte _, "sample_name");
                    bits += 8;
                }
            }
            sw.Stop();

            nsPerBit = sw.Elapsed.TotalNanoseconds / bits;
            return bits / sw.Elapsed.TotalSeconds / 1e6;
        }

        /// <summary>
        /// A minimal reader that pulls bits out of a 64-bit accumulator instead of going to the
        /// stream for each one. It has none of the behaviour the real reader needs, and is here
        /// only as a floor to compare against.
        /// </summary>
        private static double Reference(byte[] buffer, out double nsPerBit)
        {
            var sw = Stopwatch.StartNew();
            long bits = 0;
            uint sink = 0;
            for (int pass = 0; pass < Passes; pass++)
            {
                var reader = new AccumulatorReader(buffer);
                for (int i = 0; i < ElementsPerPass; i++)
                {
                    sink += reader.Read(8);
                    bits += 8;
                }
            }
            sw.Stop();

            if (sink == uint.MaxValue)
                Console.Write("");   // keeps the reads from being optimised away

            nsPerBit = sw.Elapsed.TotalNanoseconds / bits;
            return bits / sw.Elapsed.TotalSeconds / 1e6;
        }

        private sealed class AccumulatorReader
        {
            private readonly byte[] _data;
            private int _bytePos;
            private ulong _accumulator;
            private int _bitsAvailable;

            public AccumulatorReader(byte[] data) => _data = data;

            public uint Read(int count)
            {
                if (_bitsAvailable < count)
                    Fill();

                uint value = (uint)(_accumulator >> (64 - count));
                _accumulator <<= count;
                _bitsAvailable -= count;
                return value;
            }

            private void Fill()
            {
                while (_bitsAvailable <= 56 && _bytePos < _data.Length)
                {
                    _accumulator |= (ulong)_data[_bytePos++] << (56 - _bitsAvailable);
                    _bitsAvailable += 8;
                }
            }
        }
    }
}
