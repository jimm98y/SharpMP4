using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// Uncompressed PCM track: QuickTime's - 'lpcm' of a version 2 sound description, of the format its fields say, and of
    /// version 0 or 1 'raw ', 'twos', 'sowt', 'in24', 'in32', 'fl32' and 'fl64', of their coding and the 'enda' of their
    /// 'wave' - and ISO's 'ipcm' and 'fpcm' (ISO/IEC 23003-5), of their 'pcmC'. A sample is one frame, a sample of each
    /// channel, as QuickTime has it: <see cref="Readers.VideoReader.ReadSamples"/> reads a run of them at once, and the
    /// builders take a block of frames as a sample each. A track read from a file writes back the entry it was read with;
    /// one made of its format writes QuickTime's 'lpcm', as Apple's devices do.
    /// </summary>
    /// <remarks>https://developer.apple.com/documentation/quicktime-file-format/sound_sample_descriptions</remarks>
    public class PcmTrack : TrackBase
    {
        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        // the codings of PCM: QuickTime's, then ISO's
        private static readonly HashSet<string> Codings = new HashSet<string> { "lpcm", "raw ", "twos", "sowt", "in24", "in32", "fl32", "fl64", "ipcm", "fpcm" };

        // the formatSpecificFlags of a version 2 sound description: CoreAudio's kAudioFormatFlag
        private const uint FlagIsFloat = 1, FlagIsBigEndian = 2, FlagIsSignedInteger = 4, FlagIsPacked = 8;

        // the size of a version 2 sound description's fields, its sizeOfStructOnly: of its header to its last field
        private const uint SoundDescriptionV2Size = 72;

        /// <summary>The coding of the sample entry: 'lpcm', 'sowt', 'ipcm', ...</summary>
        public string Coding { get; }

        public uint SampleRate { get; }
        public ushort ChannelCount { get; }

        /// <summary>The bits of a sample of one channel: 8, 16, 24 or 32 of an integer, 32 or 64 of a float.</summary>
        public ushort BitsPerSample { get; }

        public bool IsFloat { get; }

        /// <summary>Whether the samples are signed: all but QuickTime's 8-bit 'raw ', of offset binary.</summary>
        public bool IsSigned { get; }

        public bool IsLittleEndian { get; }

        /// <summary>The bytes of a frame, a sample of each channel: the bytes of a sample of the track.</summary>
        public int BytesPerFrame { get; }

        // the entry of a track read from a file, written back as it was
        private SampleEntryCopy _entry;

        /// <summary>A track of PCM of a format, written as QuickTime's 'lpcm' - of a QuickTime file.</summary>
        /// <param name="bitsPerSample">8, 16, 24 or 32 of integers; 32 or 64 of floats.</param>
        public PcmTrack(uint sampleRate, ushort channelCount, ushort bitsPerSample, bool isFloat = false, bool isLittleEndian = true)
        {
            if (sampleRate == 0)
                throw new ArgumentOutOfRangeException(nameof(sampleRate), "A sample rate is more than 0.");
            if (channelCount == 0)
                throw new ArgumentOutOfRangeException(nameof(channelCount), "A track has a channel at least.");
            if (isFloat ? bitsPerSample is not (32 or 64) : bitsPerSample is not (8 or 16 or 24 or 32))
                throw new ArgumentOutOfRangeException(nameof(bitsPerSample), bitsPerSample, isFloat ? "Floats are of 32 or 64 bits." : "Integers are of 8, 16, 24 or 32 bits.");

            Coding = "lpcm";
            SampleRate = sampleRate;
            ChannelCount = channelCount;
            BitsPerSample = bitsPerSample;
            IsFloat = isFloat;
            IsSigned = true;
            IsLittleEndian = isLittleEndian;
            BytesPerFrame = channelCount * bitsPerSample / 8;

            CompatibleBrand = QuickTimeBrand;
            Timescale = sampleRate;
            DefaultSampleDuration = 1;
        }

        /// <summary>A track of a PCM sample entry, as a file has it, the entry written back as it was.</summary>
        /// <param name="config">The entry, or a box of it - its 'wave', 'pcmC' or 'chan' - as the reader gives it.</param>
        public PcmTrack(Box config, uint timescale = 0, int sampleDuration = -1)
        {
            var entry = EntryOf(config) ?? throw new ArgumentException($"Not a sample entry of PCM: {IsoStream.ToFourCC(config?.FourCC ?? 0)}", nameof(config));
            Coding = Encryption.SubsampleSplitter.CodingOf(entry);

            if (entry.Soundversion == 2)
            {
                // version 2: audioSampleRate a double over what version 1 has as bytesPerPacket and bytesPerFrame,
                // numAudioChannels its bytesPerSample; then always7F000000, constBitsPerChannel, formatSpecificFlags,
                // constBytesPerAudioPacket and constLPCMFramesPerAudioPacket
                byte[] v2 = entry.SoundVersion2Data ?? new byte[20];
                double rate = BitConverter.Int64BitsToDouble((long)(((ulong)entry.BytesPerPacket << 32) | entry.BytesPerFrame));
                uint flags = ReadUInt32(v2, 8);
                uint bytesPerPacket = ReadUInt32(v2, 12), framesPerPacket = ReadUInt32(v2, 16);

                SampleRate = (uint)Math.Round(rate);
                ChannelCount = (ushort)entry.BytesPerSample;
                BitsPerSample = (ushort)ReadUInt32(v2, 4);
                IsFloat = (flags & FlagIsFloat) != 0;
                IsSigned = IsFloat || (flags & FlagIsSignedInteger) != 0;
                IsLittleEndian = (flags & FlagIsBigEndian) == 0;
                BytesPerFrame = framesPerPacket > 0 && bytesPerPacket > 0 ? (int)(bytesPerPacket / framesPerPacket) : ChannelCount * ((BitsPerSample + 7) / 8);
            }
            else
            {
                // versions 0 and 1, and ISO's: the rate 16.16, unless an 'srat' says it, of a rate past 16 bits
                SampleRate = entry.Children?.OfType<SamplingRateBox>().FirstOrDefault()?.SamplingRate ?? entry.Samplerate >> 16;
                ChannelCount = entry.Channelcount;

                var pcmC = entry.Children?.OfType<PcmCBox>().FirstOrDefault();
                bool enda = FindBox<AppleEndiannessBox>(entry)?.LittleEndian is ushort little && little != 0;
                switch (Coding)
                {
                    case "ipcm":
                    case "fpcm":
                        // format_flags' lowest bit: little-endian
                        BitsPerSample = pcmC?.PcmSampleSize ?? entry.Samplesize;
                        IsFloat = Coding == "fpcm";
                        IsSigned = true;
                        IsLittleEndian = pcmC != null && (pcmC.FormatFlags & 1) != 0;
                        break;
                    case "raw ":
                        // offset binary, of 8 bits
                        BitsPerSample = 8;
                        IsSigned = false;
                        break;
                    case "twos":
                    case "sowt":
                        // of 8 or 16 bits, as the entry's sample size says: big-endian 'twos', little-endian 'sowt'
                        BitsPerSample = entry.Samplesize == 8 ? (ushort)8 : (ushort)16;
                        IsSigned = true;
                        IsLittleEndian = Coding == "sowt";
                        break;
                    default:
                        // 'in24', 'in32', 'fl32' and 'fl64': big-endian, unless their 'wave' has an 'enda' that says otherwise
                        BitsPerSample = Coding switch { "in24" => 24, "fl64" => 64, _ => 32 };
                        IsFloat = Coding.StartsWith("fl", StringComparison.Ordinal);
                        IsSigned = true;
                        IsLittleEndian = enda;
                        break;
                }
                BytesPerFrame = ChannelCount * ((BitsPerSample + 7) / 8);
            }

            if (ChannelCount == 0 || BitsPerSample == 0 || BytesPerFrame == 0)
                throw new ArgumentException($"A PCM sample entry '{Coding}' of {ChannelCount} channels of {BitsPerSample} bits.");

            _entry = SampleEntryCopy.Of(entry);
            CompatibleBrand = Coding is "ipcm" or "fpcm" ? null : QuickTimeBrand;
            Timescale = timescale == 0 ? SampleRate : timescale;
            DefaultSampleDuration = sampleDuration > 0 ? sampleDuration : 1;
        }

        /// <summary>Whether a sample entry, or a box of one, is of PCM.</summary>
        public static bool IsPcm(Box config) => EntryOf(config) != null;

        // the audio sample entry of PCM a box is, or is in
        private static AudioSampleEntry EntryOf(Box config)
        {
            var entry = config as AudioSampleEntry ?? config?.GetParent() as AudioSampleEntry;
            return entry != null && Codings.Contains(Encryption.SubsampleSplitter.CodingOf(entry)) ? entry : null;
        }

        // a box of a type among the entry's, or among those of the boxes in it - an 'enda' is in the 'wave'
        private static T FindBox<T>(Box box) where T : Box
        {
            foreach (var child in box.Children ?? Enumerable.Empty<Box>())
            {
                if (child is T found)
                    return found;
                if (child.Children != null && FindBox<T>(child) is T inner)
                    return inner;
            }
            return null;
        }

        private static uint ReadUInt32(byte[] data, int offset) =>
            (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);

        private static void WriteUInt32(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)(value >> 24);
            data[offset + 1] = (byte)(value >> 16);
            data[offset + 2] = (byte)(value >> 8);
            data[offset + 3] = (byte)value;
        }

        /// <summary>
        /// A frame, or frames: what goes to the file as it arrives, each a sync sample. The builders write a block of frames
        /// as a sample of each.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        /// <summary>
        /// The sample entry: of a track read from a file, the one it was read with; else QuickTime's 'lpcm' of a version 2
        /// sound description, its format in its fields, a frame to a packet.
        /// </summary>
        public override Box CreateSampleEntryBox()
        {
            if (_entry != null)
                return _entry.Create();

            ulong rate = (ulong)BitConverter.DoubleToInt64Bits(SampleRate);
            uint flags = (IsFloat ? FlagIsFloat : FlagIsSignedInteger) | FlagIsPacked | (IsLittleEndian ? 0 : FlagIsBigEndian);
            var v2 = new byte[20];
            WriteUInt32(v2, 0, 0x7F000000);
            WriteUInt32(v2, 4, BitsPerSample);
            WriteUInt32(v2, 8, flags);
            WriteUInt32(v2, 12, (uint)BytesPerFrame);
            WriteUInt32(v2, 16, 1);

            return new AudioSampleEntry(IsoStream.FromFourCC("lpcm"))
            {
                Children = new List<Box>(),
                ReservedSampleEntry = new byte[6],
                DataReferenceIndex = 1,
                Soundversion = 2,
                // always3, always16, alwaysMinus2, always0 and always65536
                Channelcount = 3,
                Samplesize = 16,
                PreDefined = 0xFFFE,
                Reserved = 0,
                Samplerate = 0x00010000,
                // sizeOfStructOnly, audioSampleRate and numAudioChannels
                SamplesPerPacket = SoundDescriptionV2Size,
                BytesPerPacket = (uint)(rate >> 32),
                BytesPerFrame = (uint)rate,
                BytesPerSample = ChannelCount,
                SoundVersion2Data = v2,
            };
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 256;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new PcmTrack(this));
        }

        // of a clone: the same format, and a copy of the entry of its own
        private PcmTrack(PcmTrack other)
        {
            Coding = other.Coding;
            SampleRate = other.SampleRate;
            ChannelCount = other.ChannelCount;
            BitsPerSample = other.BitsPerSample;
            IsFloat = other.IsFloat;
            IsSigned = other.IsSigned;
            IsLittleEndian = other.IsLittleEndian;
            BytesPerFrame = other.BytesPerFrame;
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
