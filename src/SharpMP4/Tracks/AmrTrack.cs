using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AMR track: AMR narrowband (3GPP TS 26.071) of a 'samr' sample entry, or AMR-WB (TS 26.171) of 'sawb', each of an
    /// 'damr' (3GPP TS 26.244 6.7). A sample is <see cref="FramesPerSample"/> speech frames of 20 ms, each as AMR's storage
    /// format has it (RFC 4867 5.3): a byte of its frame type and quality, then its bits, padded to a byte. Mono, of 8000
    /// samples a second - AMR-WB of 16000. A track read from a file writes back the entry it was read with.
    /// </summary>
    public class AmrTrack : TrackBase
    {
        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        // the bytes of a frame's speech bits, of each frame type (RFC 4867 5.3; TS 26.101 and TS 26.201): of AMR modes 0 to
        // 7 and SID; of AMR-WB modes 0 to 8 and SID; then the types of no speech, of no bytes
        private static readonly int[] NarrowbandFrameBytes = { 12, 13, 15, 17, 19, 20, 26, 31, 5, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly int[] WidebandFrameBytes = { 17, 23, 32, 36, 40, 46, 50, 58, 60, 5, 0, 0, 0, 0, 0, 0 };

        /// <summary>Whether it is AMR-WB, of 'sawb', rather than AMR's narrowband 'samr'.</summary>
        public bool IsWideband { get; }

        /// <summary>8000 samples a second of AMR, 16000 of AMR-WB.</summary>
        public uint SampleRate => IsWideband ? 16000u : 8000u;

        /// <summary>AMR is mono.</summary>
        public ushort ChannelCount => 1;

        /// <summary>The samples of a frame: 20 ms of them.</summary>
        public int SamplesPerFrame => (int)SampleRate / 50;

        /// <summary>The 'damr''s vendor of the encoder, a four-character code.</summary>
        public uint Vendor { get; }

        public byte DecoderVersion { get; }

        /// <summary>The modes the frames may be of, a bit each: of AMR, 0x81FF all of them and SID.</summary>
        public ushort ModeSet { get; }

        /// <summary>The frames, at least, between changes of mode: 0 of no such constraint.</summary>
        public byte ModeChangePeriod { get; }

        /// <summary>The frames of a sample, of 1 to 15.</summary>
        public byte FramesPerSample { get; }

        // the entry of a track read from a file, written back as it was
        private SampleEntryCopy _entry;

        /// <summary>A track of AMR or AMR-WB, written as a 'samr' or 'sawb' entry of a 'damr'.</summary>
        /// <param name="framesPerSample">The frames each sample is to be of.</param>
        public AmrTrack(bool wideband = false, byte framesPerSample = 1, ushort modeSet = 0)
        {
            if (framesPerSample is < 1 or > 15)
                throw new ArgumentOutOfRangeException(nameof(framesPerSample), framesPerSample, "A sample is of 1 to 15 frames.");
            IsWideband = wideband;
            FramesPerSample = framesPerSample;
            ModeSet = modeSet;
            Vendor = IsoStream.FromFourCC("SMP4");
            CompatibleBrand = "3gp6";
            Timescale = SampleRate;
            DefaultSampleDuration = SamplesPerFrame * framesPerSample;
        }

        /// <summary>A track of an AMR sample entry, as a file has it, the entry written back as it was.</summary>
        /// <param name="config">The entry's 'damr', or the entry, as the reader gives it.</param>
        public AmrTrack(Box config, uint timescale = 0, int sampleDuration = -1)
        {
            var entry = config as SampleEntry ?? config?.GetParent() as SampleEntry
                ?? throw new ArgumentException($"Not of an AMR sample entry: {IsoStream.ToFourCC(config?.FourCC ?? 0)}", nameof(config));
            string coding = Encryption.SubsampleSplitter.CodingOf(entry);
            if (coding != "samr" && coding != "sawb")
                throw new ArgumentException($"Not an AMR sample entry: {coding}", nameof(config));
            IsWideband = coding == "sawb";

            var damr = config as AmrSpecificBox ?? entry.Children?.OfType<AmrSpecificBox>().FirstOrDefault();
            if (damr != null)
            {
                Vendor = damr.Vendor;
                DecoderVersion = damr.DecoderVersion;
                ModeSet = damr.ModeSet;
                ModeChangePeriod = damr.ModeChangePeriod;
                FramesPerSample = damr.FramesPerSample;
            }
            if (FramesPerSample == 0)
                FramesPerSample = 1;

            _entry = SampleEntryCopy.Of(entry);
            CompatibleBrand = "3gp6";
            Timescale = timescale == 0 ? SampleRate : timescale;
            DefaultSampleDuration = sampleDuration > 0 ? sampleDuration : SamplesPerFrame * FramesPerSample;
        }

        /// <summary>
        /// The bytes of a frame of the storage format whose header byte is <paramref name="header"/>, the header and all: of
        /// its frame type, bits 6 to 3 (RFC 4867 5.3).
        /// </summary>
        public static int FrameLength(byte header, bool wideband) => 1 + (wideband ? WidebandFrameBytes : NarrowbandFrameBytes)[(header >> 3) & 0xF];

        /// <summary>A sample's frames, each its header byte and its bits.</summary>
        public IEnumerable<ArraySegment<byte>> ParseFrames(byte[] buffer, int offset, int length)
        {
            int end = offset + length;
            while (offset < end)
            {
                int frame = Math.Min(FrameLength(buffer[offset], IsWideband), end - offset);
                yield return new ArraySegment<byte>(buffer, offset, frame);
                offset += frame;
            }
        }

        /// <summary>The sample goes to the file as it arrives: its frames, each a sync sample, as every frame is decoded on its own.</summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override Box CreateSampleEntryBox()
        {
            if (_entry != null)
                return _entry.Create();

            // ISO's audio sample entry, as 3GPP's is (TS 26.244 6.5): of 2 channels and 16 bits, whatever AMR is
            var entry = new AudioSampleEntryV1(IsoStream.FromFourCC(IsWideband ? "sawb" : "samr"));
            entry.Children = new List<Box>();
            entry.Channelcount = 2;
            entry.Samplesize = 16;
            entry.Samplerate = SampleRate << 16;
            entry.DataReferenceIndex = 1;
            entry.ReservedSampleEntry = new byte[6];
            entry.Reserved = new ushort[3];

            var damr = new AmrSpecificBox
            {
                Vendor = Vendor,
                DecoderVersion = DecoderVersion,
                ModeSet = ModeSet,
                ModeChangePeriod = ModeChangePeriod,
                FramesPerSample = FramesPerSample,
            };
            damr.SetParent(entry);
            entry.Children.Add(damr);
            return entry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 1 << 8;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new AmrTrack(this));
        }

        // of a clone: the same format, and a copy of the entry of its own
        private AmrTrack(AmrTrack other)
        {
            IsWideband = other.IsWideband;
            Vendor = other.Vendor;
            DecoderVersion = other.DecoderVersion;
            ModeSet = other.ModeSet;
            ModeChangePeriod = other.ModeChangePeriod;
            FramesPerSample = other.FramesPerSample;
            _entry = other._entry?.Clone();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _entry?.Dispose();
            base.Dispose(disposing);
        }
    }
}
