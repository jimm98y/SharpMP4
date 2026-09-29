using SharpH26X;
using SharpISOBMFF;
using SharpMP4.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Encryption
{
    /// <summary>
    /// Splits a sample of NAL units into what is left in the clear and what is protected (ISO/IEC 23001-7, 9.5.2): of a
    /// VCL NAL unit its length, its NAL unit header and its slice header stay clear, for a decoder to read before the
    /// data is decrypted, and the rest is protected; any other NAL unit stays clear all through. Protected bytes are
    /// whole blocks but for 'cbcs', the bytes left over put in the clear before them, as Shaka Packager does; 'cbcs'
    /// leaves a partial block at the end in the clear of its own.
    /// </summary>
    public abstract class SubsampleSplitter
    {
        private const int BlockSize = 16;

        protected SubsampleSplitter(int lengthSize, IMp4Logger logger)
        {
            LengthSize = lengthSize;
            Logger = logger ?? DefaultMp4Logger.Instance;
        }

        /// <summary>The size of each NAL unit's length before it.</summary>
        public int LengthSize { get; }

        public IMp4Logger Logger { get; set; }

        /// <summary>
        /// The splitter of a sample entry's codec: of H.264, H.265 and H.266. Null for any other, whose samples are
        /// protected whole.
        /// </summary>
        public static SubsampleSplitter For(Box sampleEntry, IMp4Logger logger = null)
        {
            foreach (var box in sampleEntry?.Children ?? Enumerable.Empty<Box>())
            {
                switch (box)
                {
                    case AVCConfigurationBox avcC:
                        return new H264SubsampleSplitter(avcC._AVCConfig, logger);
                    case HEVCConfigurationBox hvcC:
                        return new H265SubsampleSplitter(hvcC._HEVCConfig, logger);
                    case VvcConfigurationBox vvcC:
                        return new H266SubsampleSplitter(vvcC._VvcConfig, logger);
                }
            }
            return null;
        }

        /// <summary>The subsamples of a sample: its NAL units' clear and protected runs, one after another.</summary>
        /// <param name="wholeBlocks">Whether the protected bytes of each NAL unit are to be whole blocks.</param>
        public EncryptionSubsample[] Split(byte[] buffer, int offset, int length, bool wholeBlocks)
        {
            var subsamples = new List<EncryptionSubsample>();
            long clear = 0;

            for (int position = 0; position + LengthSize <= length;)
            {
                int unit = 0;
                for (int i = 0; i < LengthSize; i++)
                    unit = unit << 8 | buffer[offset + position + i];
                int start = position + LengthSize;
                if (unit <= 0 || start + unit > length)
                {
                    // not NAL units as their lengths say: the rest is left clear
                    clear += length - position;
                    position = length;
                    break;
                }

                int header = Math.Min(unit, ClearLength(buffer, offset + start, unit));
                int protectedBytes = unit - header;
                if (wholeBlocks)
                    protectedBytes -= protectedBytes % BlockSize;
                if (protectedBytes < BlockSize)
                    protectedBytes = 0;

                clear += LengthSize + unit - protectedBytes;
                if (protectedBytes > 0)
                {
                    Add(subsamples, ref clear, (uint)protectedBytes);
                }
                position = start + unit;
            }

            // no run of none clear and none protected (9.5.1): an empty sample has no subsample at all
            if (clear > 0)
                Add(subsamples, ref clear, 0);
            return subsamples.ToArray();
        }

        /// <summary>A run of so many clear bytes and so many protected: the clear ones split where there are more than 65535.</summary>
        private static void Add(List<EncryptionSubsample> subsamples, ref long clear, uint protectedBytes)
        {
            while (clear > ushort.MaxValue)
            {
                subsamples.Add(new EncryptionSubsample(ushort.MaxValue, 0));
                clear -= ushort.MaxValue;
            }
            subsamples.Add(new EncryptionSubsample((int)clear, protectedBytes));
            clear = 0;
        }

        /// <summary>
        /// How many of a NAL unit's bytes stay clear: its header and, of a slice, its slice header; all of any other. Its
        /// parameter sets are taken in, for the slice headers after them. Its header's where the parser fails.
        /// </summary>
        protected int ClearLength(byte[] buffer, int offset, int length)
        {
            try
            {
                // one stream, reset to each NAL unit: reading one allocates no stream
                if (_stream == null)
                    _stream = new ItuStream(buffer, offset, length, Logger);
                else
                    _stream.Reset(buffer, offset, length);

                ulong? bits = Read(_stream, length);
                return bits == null ? length : RawLength(buffer, offset, length, bits.Value);
            }
            catch (Exception ex)
            {
                if (Logger.IsWarningEnabled)
                    Logger.LogWarning($"A slice header could not be read, only the NAL unit header is left clear: {ex.Message}");
                return HeaderBytes;
            }
        }

        private ItuStream _stream;

        /// <summary>The size of the codec's NAL unit header.</summary>
        protected abstract int HeaderBytes { get; }

        /// <summary>
        /// Reads a NAL unit's header and, of a slice, its slice header, returning the bits they take; null where it is not
        /// a slice, and so stays clear all through. Its parameter sets are read into the parser's context.
        /// </summary>
        protected abstract ulong? Read(ItuStream stream, int length);

        /// <summary>
        /// The bytes of a NAL unit that hold so many bits of its RBSP: those bits rounded up to bytes, and the emulation
        /// prevention bytes among them (0x03 after two 0x00).
        /// </summary>
        protected static int RawLength(byte[] buffer, int offset, int length, ulong rbspBits)
        {
            long rbspBytes = (long)((rbspBits + 7) / 8);
            int zeros = 0, i = 0;
            for (long counted = 0; i < length && counted < rbspBytes; i++)
            {
                byte b = buffer[offset + i];
                if (zeros >= 2 && b == 3)
                {
                    zeros = 0;
                    continue;
                }
                counted++;
                zeros = b == 0 ? zeros + 1 : 0;
            }
            return i;
        }

    }

    internal sealed class H264SubsampleSplitter : SubsampleSplitter
    {
        private readonly SharpH264.H264Context _context = new SharpH264.H264Context();

        public H264SubsampleSplitter(AVCDecoderConfigurationRecord config, IMp4Logger logger) : base(config.LengthSizeMinusOne + 1, logger)
        {
            foreach (var unit in (config.SequenceParameterSetNALUnit ?? Array.Empty<byte[]>()).Concat(config.PictureParameterSetNALUnit ?? Array.Empty<byte[]>()))
                ClearLength(unit, 0, unit.Length);
        }

        protected override int HeaderBytes => 1;

        protected override ulong? Read(ItuStream stream, int length)
        {
            var header = new SharpH264.NalUnit((uint)length);
            _context.NalHeader = header;
            ulong bits = header.Read(_context, stream);

            switch (header.NalUnitType)
            {
                case SharpH264.H264NALTypes.SPS:
                    (_context.SeqParameterSetRbsp = new SharpH264.SeqParameterSetRbsp()).Read(_context, stream);
                    return null;
                case SharpH264.H264NALTypes.PPS:
                    (_context.PicParameterSetRbsp = new SharpH264.PicParameterSetRbsp()).Read(_context, stream);
                    return null;
                case SharpH264.H264NALTypes.SLICE:
                case SharpH264.H264NALTypes.IDR_SLICE:
                    var slice = new SharpH264.SliceHeader();
                    _context.SliceLayerWithoutPartitioningRbsp = new SharpH264.SliceLayerWithoutPartitioningRbsp { SliceHeader = slice };
                    return bits + slice.Read(_context, stream);
                default:
                    return null;
            }
        }
    }

    internal sealed class H265SubsampleSplitter : SubsampleSplitter
    {
        private readonly SharpH265.H265Context _context = new SharpH265.H265Context();

        public H265SubsampleSplitter(HEVCDecoderConfigurationRecord config, IMp4Logger logger) : base(config.LengthSizeMinusOne + 1, logger)
        {
            foreach (var unit in (config.NalUnit ?? Array.Empty<byte[][]>()).SelectMany(x => x))
                ClearLength(unit, 0, unit.Length);
        }

        protected override int HeaderBytes => 2;

        protected override ulong? Read(ItuStream stream, int length)
        {
            var header = new SharpH265.NalUnit((uint)length);
            _context.NalHeader = header;
            ulong bits = header.Read(_context, stream);
            uint type = header.NalUnitHeader.NalUnitType;

            if (header.NalUnitHeader.NuhLayerId == 63)
                return null;
            switch (type)
            {
                case SharpH265.H265NALTypes.VPS_NUT:
                    (_context.VideoParameterSetRbsp = new SharpH265.VideoParameterSetRbsp()).Read(_context, stream);
                    return null;
                case SharpH265.H265NALTypes.SPS_NUT:
                    (_context.SeqParameterSetRbsp = new SharpH265.SeqParameterSetRbsp()).Read(_context, stream);
                    return null;
                case SharpH265.H265NALTypes.PPS_NUT:
                    (_context.PicParameterSetRbsp = new SharpH265.PicParameterSetRbsp()).Read(_context, stream);
                    return null;
            }

            // the slices: of types 0 to 9 and 16 to 21 (7.4.2.2)
            if (type <= SharpH265.H265NALTypes.RASL_R || (type >= SharpH265.H265NALTypes.BLA_W_LP && type <= SharpH265.H265NALTypes.CRA_NUT))
            {
                var slice = new SharpH265.SliceSegmentHeader();
                _context.SliceSegmentLayerRbsp = new SharpH265.SliceSegmentLayerRbsp { SliceSegmentHeader = slice };
                return bits + slice.Read(_context, stream);
            }
            return null;
        }
    }

    internal sealed class H266SubsampleSplitter : SubsampleSplitter
    {
        private readonly SharpH266.H266Context _context = new SharpH266.H266Context();

        public H266SubsampleSplitter(VvcDecoderConfigurationRecord config, IMp4Logger logger) : base(config._LengthSizeMinusOne + 1, logger)
        {
            foreach (var unit in (config.NalUnit ?? Array.Empty<byte[][]>()).SelectMany(x => x))
                ClearLength(unit, 0, unit.Length);
        }

        protected override int HeaderBytes => 2;

        protected override ulong? Read(ItuStream stream, int length)
        {
            var header = new SharpH266.NalUnit((uint)length);
            _context.NalHeader = header;
            ulong bits = header.Read(_context, stream);
            uint type = header.NalUnitHeader.NalUnitType;

            switch (type)
            {
                case SharpH266.H266NALTypes.VPS_NUT:
                    (_context.VideoParameterSetRbsp = new SharpH266.VideoParameterSetRbsp()).Read(_context, stream);
                    return null;
                case SharpH266.H266NALTypes.SPS_NUT:
                    (_context.SeqParameterSetRbsp = new SharpH266.SeqParameterSetRbsp()).Read(_context, stream);
                    return null;
                case SharpH266.H266NALTypes.PPS_NUT:
                    (_context.PicParameterSetRbsp = new SharpH266.PicParameterSetRbsp()).Read(_context, stream);
                    return null;
                case SharpH266.H266NALTypes.PREFIX_APS_NUT:
                case SharpH266.H266NALTypes.SUFFIX_APS_NUT:
                    (_context.AdaptationParameterSetRbsp = new SharpH266.AdaptationParameterSetRbsp()).Read(_context, stream);
                    return null;
                case SharpH266.H266NALTypes.PH_NUT:
                    // the picture header the slices after it take theirs from
                    (_context.PictureHeaderRbsp = new SharpH266.PictureHeaderRbsp()).Read(_context, stream);
                    return null;
            }

            // the slices: of types 0 to 11 (7.4.2.2)
            if (type <= SharpH266.H266NALTypes.GDR_NUT)
            {
                var slice = new SharpH266.SliceHeader();
                _context.SliceLayerRbsp = new SharpH266.SliceLayerRbsp { SliceHeader = slice };
                return bits + slice.Read(_context, stream);
            }
            return null;
        }
    }
}
