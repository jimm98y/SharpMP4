using SharpISOBMFF;
using System;
using System.Collections.Generic;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// Motion JPEG track: its samples JPEG pictures (ISO/IEC 10918-1), each a sync sample, coded on its own. Of QuickTime's
    /// 'jpeg' entry - Photo JPEG, a picture a sample - and 'mjpa', Motion JPEG format A, of a field or two a sample, each a
    /// JPEG picture with an APP1 'mjpg' marker; and of MP4's 'mp4v' entry whose 'esds' says JPEG, object type 0x6C (ISO/IEC
    /// 14496-1 Table 5). 'mjpb', Motion JPEG format B, is read as such too, though its pictures are of no markers but its
    /// own, which a JPEG decoder does not read. Neither entry has a configuration: each picture's frame header (SOF) is read
    /// for its size and its chroma subsampling. A track read from a file writes back the entry it was read with; one made
    /// anew writes QuickTime's 'jpeg'.
    /// </summary>
    /// <remarks>https://developer.apple.com/documentation/quicktime-file-format/video_sample_description</remarks>
    public class MjpegTrack : TrackBase
    {
        /// <summary>The object type of JPEG in an 'esds' (ISO/IEC 14496-1 Table 5): Visual ISO/IEC 10918-1.</summary>
        public const byte JPEG_OBJECT_TYPE_INDICATION = 0x6C;

        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";

        // QuickTime's entries of Motion JPEG
        private static readonly string[] Entries = { "jpeg", "mjpa", "mjpb" };

        /// <summary>Whether a sample entry's type is one of QuickTime's of Motion JPEG: 'jpeg', 'mjpa' or 'mjpb'.</summary>
        public static bool IsMjpeg(string coding) => Array.IndexOf(Entries, coding) >= 0;

        /// <summary>The coding of the sample entry: 'jpeg', 'mjpa', 'mjpb', or MP4's 'mp4v'.</summary>
        public string Coding { get; }

        /// <summary>The size of the pictures: the entry's, and of the last read, its frame header's.</summary>
        public int Width => _width;
        public int Height => _height;

        /// <summary>
        /// The chroma subsampling of the pictures, as their frame header's components' sampling factors are, of the last read:
        /// 0 of one component - grey - 1 of 4:2:0, 2 of 4:2:2, 3 of 4:4:4; -1 where none is read yet, or of another.
        /// </summary>
        public int ChromaFormat => _chromaFormat;

        // the entry of a track read from a file, written back as it was
        private SampleEntryCopy _entry;

        private int _width, _height;
        private int _chromaFormat = -1;

        public MjpegTrack()
        {
            Coding = "jpeg";
            CompatibleBrand = QuickTimeBrand;
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 2 };
            TimescaleFallback = 30000;
            FrameTickFallback = 1001;
        }

        public MjpegTrack(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        /// <summary>A track of a Motion JPEG sample entry, as a file has it: its size the entry's, the entry written back as it was.</summary>
        /// <param name="coding">The entry's coding: its type, or of a protected entry what it was before ('frma').</param>
        public MjpegTrack(VisualSampleEntry entry, string coding, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            Coding = coding;
            if (coding == "mp4v")
                CompatibleBrand = null;
            _entry = SampleEntryCopy.Of(entry);
            _width = entry.Width;
            _height = entry.Height;
        }

        /// <summary>
        /// A picture - or of 'mjpa' a field or two: a sample of its own and a sync sample. Its frame header is read for its
        /// size and chroma subsampling.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            output = default;
            isRandomAccessPoint = false;
            if (buffer == null || length == 0)
                return;

            output = new ArraySegment<byte>(buffer, offset, length);
            isRandomAccessPoint = true;
            if (TryReadFrameHeader(buffer, offset, length, out int width, out int height, out int chromaFormat))
            {
                // of 'mjpa' of two fields, the picture is of both: each field's height doubled is the frame's, as the entry says
                if (_height == 0 || height > _height / 2 + 1 || Coding != "mjpa")
                {
                    _width = width;
                    _height = height;
                }
                _chromaFormat = chromaFormat;
            }
            else if (Coding != "mjpb" && Logger.IsWarningEnabled)
            {
                Logger.LogWarning("A JPEG picture's frame header could not be read.");
            }
        }

        /// <summary>
        /// The frame header (SOF, ISO/IEC 10918-1 B.2.2) of the JPEG picture in the buffer, past its markers before it: its
        /// size, and its chroma subsampling - <see cref="ChromaFormat"/>. False where there is no picture, or no frame header
        /// before its scan.
        /// </summary>
        public static bool TryReadFrameHeader(byte[] buffer, int offset, int length, out int width, out int height, out int chromaFormat)
        {
            width = height = 0;
            chromaFormat = -1;
            int end = Math.Min(buffer.Length, offset + length);
            if (offset + 4 > end || buffer[offset] != 0xFF || buffer[offset + 1] != 0xD8)
                return false; // no SOI

            int at = offset + 2;
            while (at + 4 <= end)
            {
                if (buffer[at] != 0xFF)
                    return false;
                byte marker = buffer[at + 1];
                if (marker == 0xFF)
                {
                    at++; // fill bytes
                    continue;
                }
                if (marker == 0xD8 || marker == 0x01 || (marker >= 0xD0 && marker <= 0xD7))
                {
                    at += 2; // markers of no segment
                    continue;
                }
                int segment = (buffer[at + 2] << 8) | buffer[at + 3];
                if (segment < 2 || at + 2 + segment > end)
                    return false;

                // SOF0 to SOF15, but DHT (C4), JPG (C8) and DAC (CC)
                if (marker >= 0xC0 && marker <= 0xCF && marker != 0xC4 && marker != 0xC8 && marker != 0xCC)
                {
                    if (segment < 8)
                        return false;
                    height = (buffer[at + 5] << 8) | buffer[at + 6];
                    width = (buffer[at + 7] << 8) | buffer[at + 8];
                    int components = buffer[at + 9];
                    chromaFormat = ChromaFormatOf(buffer, at + 10, components, at + 2 + segment);
                    return width > 0 && height > 0;
                }
                if (marker == 0xDA || marker == 0xD9)
                    return false; // a scan, or the end, before any frame header
                at += 2 + segment;
            }
            return false;
        }

        /// <summary>Of a frame header's components - an id, the sampling factors H and V, a table each - the subsampling of the chroma.</summary>
        private static int ChromaFormatOf(byte[] buffer, int at, int components, int end)
        {
            if (components == 1)
                return 0;
            if (components != 3 || at + 9 > end)
                return -1;
            int h0 = buffer[at + 1] >> 4, v0 = buffer[at + 1] & 0xF;
            int h1 = buffer[at + 4] >> 4, v1 = buffer[at + 4] & 0xF;
            int h2 = buffer[at + 7] >> 4, v2 = buffer[at + 7] & 0xF;
            if (h1 != h2 || v1 != v2 || h1 == 0 || v1 == 0)
                return -1;
            return (h0 / h1, v0 / v1) switch
            {
                (2, 2) => 1,
                (2, 1) => 2,
                (1, 1) => 3,
                _ => -1,
            };
        }

        public override Box CreateSampleEntryBox()
        {
            if (_entry != null)
                return _entry.Create();

            var entry = new VisualSampleEntry(IsoStream.FromFourCC("jpeg"));
            entry.Children = new List<Box>();
            entry.ReservedSampleEntry = new byte[6];
            entry.PreDefined0 = new uint[3];
            entry.DataReferenceIndex = 1;
            entry.Depth = 24;
            entry.FrameCount = 1;
            entry.Horizresolution = 72 << 16;
            entry.Vertresolution = 72 << 16;
            entry.Width = (ushort)_width;
            entry.Height = (ushort)_height;
            entry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
            return entry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Width = (uint)_width << 16;
            tkhd.Height = (uint)_height << 16;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new MjpegTrack(this));
        }

        // of a clone: the same coding and size, and a copy of the entry of its own
        private MjpegTrack(MjpegTrack other) : this(other.Timescale, other.DefaultSampleDuration)
        {
            Coding = other.Coding;
            CompatibleBrand = other.CompatibleBrand;
            _entry = other._entry?.Clone();
            _width = other._width;
            _height = other._height;
            _chromaFormat = other._chromaFormat;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _entry?.Dispose();
            base.Dispose(disposing);
        }
    }
}
