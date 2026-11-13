using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// DTS Track: DTS Coherent Acoustics and DTS-HD, each sample an access unit - a core frame, the extension substreams
    /// after it, or extension substreams alone - in a 'dtsc', 'dtsh', 'dtsl' or 'dtse' sample entry, as its stream is
    /// made, and its 'ddts'. Read also of an 'mp4a' entry whose 'esds' is of DTS's object types, 0xA9 to 0xAC, as ffmpeg
    /// writes it; written back as it was read.
    /// </summary>
    /// <remarks>ETSI TS 102 114 5.3, 7.4, 7.5, Annex E; MP4RA's object types</remarks>
    public class DtsTrack : TrackBase
    {
        /// <summary>The 'esds' object types of DTS (MP4RA): of DTS Coherent Acoustics, of DTS-HD High Resolution Audio, of DTS-HD Master Audio and of DTS Express.</summary>
        public const byte DTS_OBJECT_TYPE_INDICATION = 0xA9;
        public const byte DTSHD_HRA_OBJECT_TYPE_INDICATION = 0xAA;
        public const byte DTSHD_MA_OBJECT_TYPE_INDICATION = 0xAB;
        public const byte DTS_EXPRESS_OBJECT_TYPE_INDICATION = 0xAC;

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        /// <summary>The sample entry's coding: 'dtsc', 'dtsh', 'dtsl' or 'dtse'; or 'mp4a', of a track read of an 'esds'.</summary>
        public string Coding { get; private set; }

        /// <summary>The rate of the stream, of its asset of the highest: what the track's timescale is.</summary>
        public uint SamplingRate { get; private set; }

        /// <summary>The samples of each channel an access unit holds, at <see cref="SamplingRate"/>.</summary>
        public int SamplesPerFrame { get; private set; }

        /// <summary>The bits of a sample of the original audio: 16 or 24.</summary>
        public byte PcmSampleDepth { get; private set; }

        /// <summary>The speakers, as ETSI TS 102 114 masks them: 0x1 the centre, 0x2 left and right, 0x4 the surrounds, 0x8 the LFE, ...</summary>
        public ushort ChannelLayout { get; private set; }

        /// <summary>The channels: of <see cref="ChannelLayout"/>, a bit of a pair two.</summary>
        public byte ChannelCount { get; private set; }

        /// <summary>The 'ddts''s construction of the stream - which of the core, its extensions and the substream's components it is of - of 0 to 21.</summary>
        public byte StreamConstruction { get; private set; }

        /// <summary>The core's channels, its AMODE, of a stream that has a core; 31 of one that has none.</summary>
        public byte CoreLayout { get; private set; }

        /// <summary>Whether the core has the LFE channel.</summary>
        public bool CoreLfePresent { get; private set; }

        /// <summary>The bytes of the core frame of each access unit; 0 of a stream of no core.</summary>
        public ushort CoreSize { get; private set; }

        /// <summary>Whether a stereo downmix is embedded in the stream.</summary>
        public bool StereoDownmix { get; private set; }

        /// <summary>Whether the extension substream has more than one asset.</summary>
        public bool MultiAsset { get; private set; }

        /// <summary>Of DTS Express, the 'ddts''s LBRDurationMod, as a file has it: 0 when written.</summary>
        public bool LbrDurationMod { get; private set; }

        /// <summary>The peak and average bit rate, in bits a second: of the first access unit, its bytes over its duration, unless set.</summary>
        public uint MaxBitRate { get; set; }
        public uint AvgBitRate { get; set; }

        /// <summary>Of a track read of an 'esds': its object type, and its decoder buffer size.</summary>
        public byte ObjectTypeIndication { get; private set; }
        public uint BufferSizeDB { get; set; }

        private DtsTrack()
        {
        }

        /// <summary>
        /// Ctor, of an access unit of the stream: see <see cref="DtsFrame.Parse(byte[], int, int)"/>. What the sample entry
        /// and its 'ddts' say is of its headers - of the core's, and of the extension substream's static fields and asset
        /// descriptors - and its bit rate of its length.
        /// </summary>
        public DtsTrack(DtsFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));
            if (frame.SamplingRate == 0 || frame.SamplesPerFrame == 0)
                throw new ArgumentException("A DTS access unit whose rate and duration are known: of its core, or of its extension substream's static fields.", nameof(frame));
            if (DtsFrame.FrameDurationCodeOf(frame.SamplesPerFrame) < 0)
                throw new ArgumentException($"A DTS access unit of 512, 1024, 2048 or 4096 samples, as the 'ddts' can say: not of {frame.SamplesPerFrame}.", nameof(frame));

            Coding = frame.Coding;
            SamplingRate = frame.SamplingRate;
            SamplesPerFrame = frame.SamplesPerFrame;
            PcmSampleDepth = frame.BitDepth > 16 ? (byte)24 : (byte)16;
            ChannelLayout = frame.ChannelLayout;
            ChannelCount = frame.ChannelCount;
            StreamConstruction = frame.StreamConstruction;
            CoreLayout = frame.HasCore && Array.IndexOf(DtsFrame.CoreLayouts, frame.Amode) >= 0 ? frame.Amode : (byte)31;
            CoreLfePresent = frame.HasCore && frame.Lff != 0;
            CoreSize = frame.HasCore ? (ushort)Math.Min(frame.CoreLength, 0x3FFF) : (ushort)0;
            StereoDownmix = frame.EmbeddedStereo;
            MultiAsset = frame.AssetCount > 1;
            MaxBitRate = AvgBitRate = (uint)((ulong)frame.Length * 8 * frame.SamplingRate / (ulong)frame.SamplesPerFrame);
            Timescale = SamplingRate;
            DefaultSampleDuration = SamplesPerFrame;
        }

        /// <summary>
        /// Ctor with initialization from the <see cref="SampleEntry"/>.
        /// </summary>
        /// <param name="config">The entry's <see cref="DTSSpecificBox"/>, or its 'esds' of a DTS object type.</param>
        public DtsTrack(Box config, uint timescale = 0, int sampleDuration = -1)
        {
            Box entry = config is SampleEntry ? config : config?.GetParent() as Box;
            ushort sampleSize;
            switch (entry)
            {
                case AudioSampleEntryV1 v1:
                    ChannelCount = (byte)v1.Channelcount;
                    SamplingRate = v1.Samplerate >> 16;
                    sampleSize = v1.Samplesize;
                    break;
                case AudioSampleEntry v0:
                    ChannelCount = (byte)v0.Channelcount;
                    SamplingRate = v0.Samplerate >> 16;
                    sampleSize = v0.Samplesize;
                    break;
                default:
                    throw new NotSupportedException($"No audio sample entry of the DTS track: {(config == null ? "none" : IsoStream.ToFourCC(config.FourCC))}");
            }
            Coding = Encryption.SubsampleSplitter.CodingOf(entry);

            if (config is DTSSpecificBox ddts)
            {
                if (ddts.DtsSamplingFrequency != 0)
                    SamplingRate = ddts.DtsSamplingFrequency;
                MaxBitRate = ddts.MaxBitRate;
                AvgBitRate = ddts.AvgBitRate;
                PcmSampleDepth = ddts.PcmSampleDepth;
                SamplesPerFrame = 512 << (ddts.FrameDuration & 0x3);
                StreamConstruction = ddts.StreamConstruction;
                CoreLfePresent = ddts.CoreLFEPresent;
                CoreLayout = ddts.CoreLayout;
                CoreSize = ddts.CoreSize;
                StereoDownmix = ddts.StereoDownmix;
                ChannelLayout = ddts.ChannelLayout;
                MultiAsset = ddts.MultiAssetFlag;
                LbrDurationMod = ddts.LbrDurationMod;
                if (ChannelLayout != 0)
                    ChannelCount = DtsFrame.ChannelsOf(ChannelLayout);
            }
            else if (IsDts(config) && DecoderConfigOf(config) is DecoderConfigDescriptor decoderConfig)
            {
                Coding = "mp4a";
                ObjectTypeIndication = decoderConfig.ObjectTypeIndication;
                MaxBitRate = decoderConfig.MaxBitrate;
                AvgBitRate = decoderConfig.AvgBitrate;
                BufferSizeDB = decoderConfig.BufferSizeDB;
                PcmSampleDepth = sampleSize > 16 ? (byte)24 : (byte)16;
                // the 'esds' says nothing more: of a core of 48 kHz, as most are, an access unit of 512 samples
                SamplesPerFrame = 512;
            }
            else
            {
                throw new ArgumentException($"Invalid DTSSpecificBox: {IsoStream.ToFourCC(config.FourCC)}");
            }

            Timescale = timescale == 0 ? SamplingRate : timescale;
            DefaultSampleDuration = sampleDuration > 0 ? sampleDuration : SamplesPerFrame;
        }

        /// <summary>Whether a box is of DTS: a 'ddts', or an 'esds' of DTS's object types.</summary>
        public static bool IsDts(Box config)
        {
            if (config is DTSSpecificBox)
                return true;
            byte oti = DecoderConfigOf(config)?.ObjectTypeIndication ?? 0;
            return oti >= DTS_OBJECT_TYPE_INDICATION && oti <= DTS_EXPRESS_OBJECT_TYPE_INDICATION;
        }

        private static DecoderConfigDescriptor DecoderConfigOf(Box config)
        {
            ESDBox esd = config as ESDBox ?? config?.Children?.OfType<ESDBox>().FirstOrDefault();
            return esd?._ES?.Children?.OfType<DecoderConfigDescriptor>().FirstOrDefault();
        }

        /// <summary>
        /// The access units of a run of DTS - what an encoder hands out, of one or more - each a sample: a core frame and
        /// the extension substreams after it, or the extension substreams of a stream of none. Known by their sync words and
        /// the lengths their headers give. Bytes before the first, or of none, are skipped.
        /// </summary>
        public static IEnumerable<ArraySegment<byte>> ParseFrames(byte[] buffer, int offset, int length)
        {
            int end = offset + length;
            int position = offset;
            while (position + DtsFrame.MinimumLength <= end)
            {
                int unit = DtsFrame.AccessUnitLength(buffer, position, end - position);
                if (unit <= 0)
                {
                    position++;
                    continue;
                }
                yield return new ArraySegment<byte>(buffer, position, unit);
                position += unit;
            }
        }

        /// <summary>
        /// The sample goes to the file as it arrives - an access unit - so this hands back exactly what it was given,
        /// without copying it anywhere. An encoder's output of several is split by <see cref="ParseFrames"/> first.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC(Coding));
            audioSampleEntry.Children = new List<Box>();
            audioSampleEntry.Channelcount = ChannelCount;
            // 16.16 fixed point, of a rate it can hold; of one above 65535, 0, the stream's own rate in its 'ddts'
            audioSampleEntry.Samplerate = SamplingRate <= 0xFFFF ? SamplingRate << 16 : 0;
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.Samplesize = 16;
            audioSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            audioSampleEntry.Reserved = new ushort[3]; // TODO simplify API

            Box config = Coding == "mp4a" ? CreateEsds() : new DTSSpecificBox
            {
                DtsSamplingFrequency = SamplingRate,
                MaxBitRate = MaxBitRate,
                AvgBitRate = AvgBitRate,
                PcmSampleDepth = PcmSampleDepth,
                FrameDuration = (byte)Math.Max(0, DtsFrame.FrameDurationCodeOf(SamplesPerFrame)),
                StreamConstruction = StreamConstruction,
                CoreLFEPresent = CoreLfePresent,
                CoreLayout = CoreLayout,
                CoreSize = CoreSize,
                StereoDownmix = StereoDownmix,
                RepresentationType = 4, // not applicable: as DTS's own tools, ffmpeg and mp4parser write it
                ChannelLayout = ChannelLayout,
                MultiAssetFlag = MultiAsset,
                LbrDurationMod = LbrDurationMod,
            };
            config.SetParent(audioSampleEntry);
            audioSampleEntry.Children.Add(config);

            return audioSampleEntry;
        }

        // of a track read of an 'esds': the 'esds' as ffmpeg writes it, of no decoder specific info
        private ESDBox CreateEsds()
        {
            ESDBox esds = new ESDBox();
            ES_Descriptor descriptor = new ES_Descriptor();
            descriptor.Children = new List<Descriptor>();
            esds._ES = descriptor;
            descriptor.ESID = (ushort)TrackID;

            DecoderConfigDescriptor decoderConfigDescriptor = new DecoderConfigDescriptor();
            decoderConfigDescriptor.SetParent(descriptor);
            descriptor.Children.Add(decoderConfigDescriptor);
            decoderConfigDescriptor.Children = new List<Descriptor>();
            decoderConfigDescriptor.ObjectTypeIndication = ObjectTypeIndication;
            decoderConfigDescriptor.StreamType = Mp3Track.AUDIO_STREAM_TYPE;
            decoderConfigDescriptor.MaxBitrate = MaxBitRate;
            decoderConfigDescriptor.AvgBitrate = AvgBitRate;
            decoderConfigDescriptor.BufferSizeDB = BufferSizeDB;

            SLConfigDescriptor slConfigDescriptor = new SLConfigDescriptor();
            slConfigDescriptor.Predefined = 2;
            slConfigDescriptor.Ocr = new byte[0]; // TODO simplify API
            slConfigDescriptor.UseTimeStampsFlag = true; // TODO simplify API
            slConfigDescriptor.SetParent(descriptor);
            descriptor.Children.Add(slConfigDescriptor);
            return esds;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 256;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new DtsTrack
            {
                Coding = Coding,
                SamplingRate = SamplingRate,
                SamplesPerFrame = SamplesPerFrame,
                PcmSampleDepth = PcmSampleDepth,
                ChannelLayout = ChannelLayout,
                ChannelCount = ChannelCount,
                StreamConstruction = StreamConstruction,
                CoreLayout = CoreLayout,
                CoreLfePresent = CoreLfePresent,
                CoreSize = CoreSize,
                StereoDownmix = StereoDownmix,
                MultiAsset = MultiAsset,
                LbrDurationMod = LbrDurationMod,
                MaxBitRate = MaxBitRate,
                AvgBitRate = AvgBitRate,
                ObjectTypeIndication = ObjectTypeIndication,
                BufferSizeDB = BufferSizeDB,
            });
        }
    }

    /// <summary>The coding components of a DTS stream: of its core substream, and of its extension substream's assets.</summary>
    [Flags]
    public enum DtsComponents
    {
        None = 0,
        /// <summary>The core, in the core substream.</summary>
        Core = 0x001,
        /// <summary>The core's extensions, in its frame: of 6.1 channels, and of 96 kHz; and XXCH, of more channels.</summary>
        CoreXxch = 0x002,
        CoreX96 = 0x004,
        CoreXch = 0x008,
        /// <summary>The components of the extension substream: a core, more bit rate, more channels, 96 kHz, DTS Express and lossless.</summary>
        SubstreamCore = 0x010,
        Xbr = 0x020,
        Xxch = 0x040,
        X96 = 0x080,
        Lbr = 0x100,
        Xll = 0x200,
    }

    /// <summary>
    /// A DTS access unit's headers: the core frame's (ETSI TS 102 114 5.3.1) - its sync word 0x7FFE8001, its length, its
    /// samples, its channels and rate, its extension - and of the extension substreams after it, or alone, those of the
    /// first (7.4.1, 7.5): the sync word 0x64582025, its length, and of its static fields and asset descriptors the
    /// duration, rate, bits, speakers and coding components. Of the 16-bit big-endian stream MP4 holds.
    /// </summary>
    public class DtsFrame
    {
        public const uint CoreSyncWord = 0x7FFE8001;
        public const uint SubstreamSyncWord = 0x64582025;

        /// <summary>The bytes of a core frame's header up to its PCMR, or of an extension substream's up to its length.</summary>
        public const int MinimumLength = 11;

        /// <summary>The core's rate of each SFREQ (ETSI TS 102 114 5.3.1, Table 5-5): 0 is invalid.</summary>
        public static readonly uint[] CoreSampleRates = { 0, 8000, 16000, 32000, 0, 0, 11025, 22050, 44100, 0, 0, 12000, 24000, 48000, 0, 0 };

        /// <summary>An extension substream asset's rate of each code (Table 7-5).</summary>
        public static readonly uint[] SubstreamSampleRates = { 8000, 16000, 32000, 64000, 128000, 22050, 44100, 88200, 176400, 352800, 12000, 24000, 48000, 96000, 192000, 384000 };

        /// <summary>The reference clock of each code of the extension substream's static fields: 3 is reserved.</summary>
        private static readonly uint[] ReferenceClocks = { 32000, 44100, 48000, 0 };

        /// <summary>The speakers of each AMODE of 0 to 9 (Table 5-4), as the masks of <see cref="DtsTrack.ChannelLayout"/>.</summary>
        private static readonly ushort[] AmodeLayouts = { 0x1, 0x2, 0x2, 0x2, 0x2, 0x3, 0x12, 0x13, 0x6, 0x7 };

        /// <summary>The AMODEs a 'ddts' says as its CoreLayout, as ETSI TS 102 114 E.2 has them; of any other, 31.</summary>
        internal static readonly byte[] CoreLayouts = { 0, 2, 4, 5, 6, 7, 8, 9 };

        // the sync words of the extension substream's components, of an asset whose descriptor is not read through
        private static readonly (uint Sync, DtsComponents Component)[] ComponentSyncWords =
        {
            (0x02B09261, DtsComponents.SubstreamCore), (0x655E315E, DtsComponents.Xbr), (0x47004A03, DtsComponents.Xxch),
            (0x1D95F262, DtsComponents.X96), (0x0A801921, DtsComponents.Lbr), (0x41A29547, DtsComponents.Xll),
        };

        /// <summary>The bytes of the whole access unit: its core frame and its extension substreams.</summary>
        public int Length { get; set; }

        // the core frame
        public bool HasCore { get; set; }
        /// <summary>The core frame's bytes: its FSIZE + 1.</summary>
        public int CoreLength { get; set; }
        /// <summary>The core's samples: 32 × (NBLKS + 1).</summary>
        public int CoreSamples { get; set; }
        public byte Amode { get; set; }
        public byte Sfreq { get; set; }
        public byte Rate { get; set; }
        public bool ExtAudio { get; set; }
        public byte ExtAudioId { get; set; }
        public byte Lff { get; set; }
        public byte Pcmr { get; set; }

        public uint CoreSamplingRate => CoreSampleRates[Sfreq & 0xF];

        // the extension substreams
        public bool HasSubstream { get; set; }
        /// <summary>The bytes of the extension substreams together.</summary>
        public int SubstreamLength { get; set; }
        /// <summary>Whether the first has its static fields: what the rest of these are of.</summary>
        public bool HasStaticFields { get; set; }
        /// <summary>Its duration in samples at <see cref="SubstreamSamplingRate"/>: of its static fields' duration of the reference clock.</summary>
        public int SubstreamSamples { get; set; }
        public uint SubstreamSamplingRate { get; set; }
        public byte SubstreamBitDepth { get; set; }
        public ushort SpeakerMask { get; set; }
        public int SubstreamChannels { get; set; }
        public bool EmbeddedStereo { get; set; }
        public int AssetCount { get; set; }

        /// <summary>The coding components of the core frame and of the extension substream's assets.</summary>
        public DtsComponents Components { get; set; }

        /// <summary>The rate of the stream: of its extension substream's asset, else of its core.</summary>
        public uint SamplingRate => SubstreamSamplingRate != 0 ? SubstreamSamplingRate : HasCore ? CoreSamplingRate : 0;

        /// <summary>The samples of each channel the access unit holds, at <see cref="SamplingRate"/>.</summary>
        public int SamplesPerFrame
        {
            get
            {
                if (SubstreamSamples != 0)
                    return SubstreamSamples;
                uint core = CoreSamplingRate;
                return HasCore && core != 0 ? (int)((long)CoreSamples * SamplingRate / core) : 0;
            }
        }

        /// <summary>The bits of a sample: of the extension substream's asset, else of the core's PCMR.</summary>
        public int BitDepth => SubstreamBitDepth != 0 ? SubstreamBitDepth : Pcmr switch { 0 or 1 => 16, 2 or 3 => 20, 5 or 6 => 24, _ => 16 };

        /// <summary>The speakers: of the extension substream's asset, else of the core's AMODE, its LFE and its 6.1 extension.</summary>
        public ushort ChannelLayout
        {
            get
            {
                if (SpeakerMask != 0)
                    return SpeakerMask;
                if (!HasCore || Amode >= AmodeLayouts.Length)
                    return 0;
                int layout = AmodeLayouts[Amode];
                if (Lff != 0)
                    layout |= 0x8; // LFE
                if ((Components & DtsComponents.CoreXch) != 0)
                    layout |= 0x10; // the rear centre of XCh
                return (ushort)layout;
            }
        }

        /// <summary>The channels: of <see cref="ChannelLayout"/>, else the asset's own count.</summary>
        public byte ChannelCount => ChannelLayout != 0 ? ChannelsOf(ChannelLayout) : (byte)Math.Max(0, SubstreamChannels);

        /// <summary>The channels of a speaker mask: of a bit of one speaker one, of a pair two (ETSI TS 102 114 Table 7-10).</summary>
        public static byte ChannelsOf(ushort layout)
        {
            int count = 0;
            for (int bit = 0; bit < 16; bit++)
            {
                if (((layout >> bit) & 1) != 0)
                    count += bit is 0 or 3 or 4 or 7 or 8 or 12 or 14 ? 1 : 2;
            }
            return (byte)count;
        }

        /// <summary>The 'ddts''s FrameDuration of samples: 0 to 3 of 512 to 4096, -1 of any other.</summary>
        public static int FrameDurationCodeOf(int samples) => samples switch { 512 => 0, 1024 => 1, 2048 => 2, 4096 => 3, _ => -1 };

        /// <summary>
        /// The sample entry's coding, as the stream is made (ETSI TS 102 114 E.1): 'dtsc' of a core alone, or of its XCh or
        /// X96; 'dtsl' of lossless alone; 'dtse' of DTS Express; 'dtsh' of any other DTS-HD.
        /// </summary>
        public string Coding => Construction().Coding;

        /// <summary>The 'ddts''s StreamConstruction (ETSI TS 102 114 Table E-2): 0 of a stream the table has not.</summary>
        public byte StreamConstruction => Construction().Construction;

        private (string Coding, byte Construction) Construction()
        {
            DtsComponents ext = Components & SubstreamComponents;
            if (!HasCore)
            {
                if ((ext & DtsComponents.Lbr) != 0)
                    return ("dtse", 18);
                if (ext == DtsComponents.Xll)
                    return ("dtsl", 17);
                return ext switch
                {
                    DtsComponents.SubstreamCore => ("dtsh", 19),
                    DtsComponents.SubstreamCore | DtsComponents.Xxch => ("dtsh", 20),
                    DtsComponents.SubstreamCore | DtsComponents.Xll => ("dtsh", 21),
                    _ => ("dtsh", 0),
                };
            }

            // of the core's own extension: XCh, X96, XXCH, of its EXT_AUDIO_ID 0, 2, 6
            int coreExt = !ExtAudio ? -1 : ExtAudioId;
            if (ext == DtsComponents.None)
            {
                return coreExt switch
                {
                    -1 => ("dtsc", 1),
                    0 => ("dtsc", 2),
                    2 => ("dtsc", 4),
                    6 => ("dtsh", 3),
                    _ => ("dtsh", 0),
                };
            }

            byte construction = (coreExt, ext) switch
            {
                (-1, DtsComponents.Xxch) => 5,
                (-1, DtsComponents.Xbr) => 6,
                (-1, DtsComponents.Xxch | DtsComponents.Xbr) => 9,
                (-1, DtsComponents.X96) => 10,
                (-1, DtsComponents.Xxch | DtsComponents.X96) => 13,
                (-1, DtsComponents.Xll) => 14,
                (0, DtsComponents.Xbr) => 7,
                (6, DtsComponents.Xbr) => 8,
                (0, DtsComponents.X96) => 11,
                (6, DtsComponents.X96) => 12,
                (0, DtsComponents.Xll) => 15,
                (2, DtsComponents.Xll) => 16,
                _ => 0,
            };
            return ("dtsh", construction);
        }

        /// <summary>Reads the headers of the access unit at the start of the buffer: see <see cref="TryParse(byte[], int, int, out DtsFrame)"/>.</summary>
        public static bool TryParse(byte[] frame, out DtsFrame dtsFrame) => TryParse(frame, 0, frame?.Length ?? 0, out dtsFrame);

        /// <summary>
        /// Reads the headers of the access unit at the offset: of a core frame and the extension substreams after it, or of
        /// extension substreams alone. False of neither sync word, of a header cut short, or of a core frame of reserved
        /// fields.
        /// </summary>
        public static bool TryParse(byte[] buffer, int offset, int length, out DtsFrame dtsFrame) => Read(buffer, offset, length, out dtsFrame) == null;

        /// <summary>Reads the headers of the access unit at the start of the buffer: see <see cref="Parse(byte[], int, int)"/>.</summary>
        public static DtsFrame Parse(byte[] frame) => Parse(frame, 0, frame?.Length ?? 0);

        /// <summary>Reads the headers of the access unit at the offset.</summary>
        /// <exception cref="ArgumentException">Of neither sync word, of a header cut short, or of reserved fields.</exception>
        public static DtsFrame Parse(byte[] buffer, int offset, int length)
        {
            string error = Read(buffer, offset, length, out DtsFrame frame);
            if (error != null)
                throw new ArgumentException(error, nameof(buffer));
            return frame;
        }

        /// <summary>
        /// The bytes of the access unit at the offset: of its core frame and the extension substreams that follow it, or of
        /// the extension substreams of a stream of no core - up to the next of index 0. 0 of none there, or of one cut short.
        /// </summary>
        internal static int AccessUnitLength(byte[] buffer, int offset, int length)
        {
            if (Read(buffer, offset, length, out DtsFrame frame, headersOnly: true) != null)
                return 0;
            return frame.Length;
        }

        private static uint SyncAt(byte[] buffer, int offset, int end) =>
            offset + 4 <= end ? (uint)(buffer[offset] << 24 | buffer[offset + 1] << 16 | buffer[offset + 2] << 8 | buffer[offset + 3]) : 0;

        // the headers, or why there are none
        private static string Read(byte[] buffer, int offset, int length, out DtsFrame dtsFrame, bool headersOnly = false)
        {
            dtsFrame = null;
            if (buffer == null || offset < 0 || length < 0 || offset + length > buffer.Length)
                return "A DTS access unit within its buffer.";

            int end = offset + length;
            var frame = new DtsFrame();
            int position = offset;
            uint sync = SyncAt(buffer, position, end);

            if (sync == CoreSyncWord)
            {
                string error = ReadCore(buffer, position, end, frame);
                if (error != null)
                    return error;
                position += frame.CoreLength;
            }
            else if (sync != SubstreamSyncWord)
            {
                return "A DTS access unit begins with the sync word of a core frame, 0x7FFE8001, or of an extension substream, 0x64582025 - of the 16-bit big-endian stream MP4 holds.";
            }

            // the extension substreams: after a core, until the next core; alone, the first and those of a higher index
            bool first = true;
            while (SyncAt(buffer, position, end) == SubstreamSyncWord)
            {
                if (position + MinimumLength > end)
                    return "An extension substream's header is cut short.";

                var bits = new AC3Track.BitReader(buffer, position + 4);
                bits.Read(8); // user defined
                int index = (int)bits.Read(2);
                bool wide = bits.Read(1) == 1;
                int headerSize = (int)bits.Read(wide ? 12 : 8) + 1;
                int size = (int)bits.Read(wide ? 20 : 16) + 1;
                if (!first && !frame.HasCore && index == 0)
                    break; // the next access unit's
                if (size < headerSize || position + size > end)
                    return $"An extension substream of {size} bytes, past the {end - position} there are.";

                if (first && !headersOnly)
                    ReadSubstream(buffer, position, headerSize, size, wide, index, ref bits, frame);
                frame.HasSubstream = true;
                frame.SubstreamLength += size;
                position += size;
                first = false;
            }

            frame.Length = position - offset;
            dtsFrame = frame;
            return null;
        }

        private static string ReadCore(byte[] buffer, int position, int end, DtsFrame frame)
        {
            if (position + MinimumLength > end)
                return "A DTS core frame's header is cut short.";

            var bits = new AC3Track.BitReader(buffer, position + 4);
            bool normal = bits.Read(1) == 1; // FTYPE: 1 of a normal frame, 0 of a termination frame
            uint deficit = bits.Read(5);     // SHORT: 31 of a normal frame
            bool crc = bits.Read(1) == 1;    // CPF
            uint nblks = bits.Read(7);
            uint fsize = bits.Read(14);
            frame.Amode = (byte)bits.Read(6);
            frame.Sfreq = (byte)bits.Read(4);
            frame.Rate = (byte)bits.Read(5);
            if (bits.Read(1) != 0) // fixed to 0
                return "A DTS core frame's reserved bit is set.";
            bits.Read(4); // DYNF, TIMEF, AUXF, HDCD
            frame.ExtAudioId = (byte)bits.Read(3);
            frame.ExtAudio = bits.Read(1) == 1;
            bits.Read(1); // ASPF
            frame.Lff = (byte)bits.Read(2);
            bits.Read(1); // HFLAG
            if (crc)
                bits.Read(16); // HCRC
            bits.Read(1 + 4 + 2); // FILTS, VERNUM, CHIST
            frame.Pcmr = (byte)bits.Read(3);

            // a termination frame - the last, shorter - says by SHORT how many of its samples are not of the stream
            if (normal && deficit != 31)
                return $"A normal DTS core frame of SHORT {deficit}: it is 31 of all but a termination frame.";
            if (nblks < 5 || fsize < 95)
                return $"A DTS core frame of NBLKS {nblks} or FSIZE {fsize} is invalid.";
            if (frame.CoreSamplingRate == 0)
                return $"A DTS core frame's SFREQ {frame.Sfreq} is invalid.";
            if (frame.Lff == 3)
                return "A DTS core frame's LFF 3 is invalid.";

            frame.HasCore = true;
            frame.CoreLength = (int)fsize + 1;
            frame.CoreSamples = 32 * ((int)nblks + 1);
            if (position + frame.CoreLength > end)
                return $"A DTS core frame of {frame.CoreLength} bytes, past the {end - position} there are.";
            if (frame.ExtAudio)
            {
                frame.Components |= frame.ExtAudioId switch
                {
                    0 => DtsComponents.CoreXch,
                    2 => DtsComponents.CoreX96,
                    6 => DtsComponents.CoreXxch,
                    _ => DtsComponents.None,
                };
            }
            frame.Components |= DtsComponents.Core;
            return null;
        }

        /// <summary>
        /// The first extension substream's static fields and asset descriptors (ETSI TS 102 114 7.4.1, 7.5), of the bits
        /// after its length. Of an asset whose descriptor does not read through to its coding components, or reads to ones
        /// other than its data begins with, its components are known by the sync word its data begins with.
        /// </summary>
        private static void ReadSubstream(byte[] buffer, int position, int headerSize, int size, bool wide, int index, ref AC3Track.BitReader bits, DtsFrame frame)
        {
            int sizeBits = wide ? 20 : 16;
            int assets = 1;
            int[] mixOutChannels = Array.Empty<int>();
            uint clock = 0;
            int duration = 0;
            frame.HasStaticFields = bits.Read(1) == 1;
            if (frame.HasStaticFields)
            {
                clock = ReferenceClocks[bits.Read(2)];
                duration = 512 * ((int)bits.Read(3) + 1); // of the reference clock
                if (bits.Read(1) == 1)
                    bits.Skip(36); // the timecode
                int presentations = (int)bits.Read(3) + 1;
                assets = (int)bits.Read(3) + 1;
                var masks = new uint[presentations];
                for (int i = 0; i < presentations; i++)
                    masks[i] = bits.Read(index + 1);
                for (int i = 0; i < presentations; i++)
                {
                    for (int j = 0; j <= index; j++)
                    {
                        if (((masks[i] >> j) & 1) != 0)
                            bits.Read(8); // the active assets
                    }
                }
                if (bits.Read(1) == 1) // mixing metadata
                {
                    bits.Read(2);
                    int maskBits = ((int)bits.Read(2) + 1) << 2;
                    mixOutChannels = new int[bits.Read(2) + 1];
                    for (int i = 0; i < mixOutChannels.Length; i++)
                        mixOutChannels[i] = ChannelsOf((ushort)bits.Read(maskBits));
                }
            }
            frame.AssetCount = assets;

            var assetSizes = new int[assets];
            for (int i = 0; i < assets; i++)
                assetSizes[i] = (int)bits.Read(sizeBits) + 1;

            int assetPosition = position + headerSize;
            for (int i = 0; i < assets; i++)
            {
                if (assetPosition + 4 > position + size)
                    break;
                var asset = ReadAssetDescriptor(ref bits, frame.HasStaticFields, mixOutChannels, out int descriptorBits);
                DtsComponents byDescriptor = asset.Components;
                DtsComponents bySync = ComponentAt(buffer, assetPosition, position + size);
                // a descriptor read wrong - or of what this does not read - is caught by the component its data begins with:
                // the first of the substream's components it says, which are in the order of their bits
                bool agrees = byDescriptor != DtsComponents.None && (bySync == DtsComponents.None || LowestOf(byDescriptor & SubstreamComponents) == bySync);
                frame.Components |= agrees ? byDescriptor : bySync;
                if (i == 0 && frame.HasStaticFields)
                {
                    frame.SubstreamBitDepth = asset.BitDepth;
                    frame.SubstreamSamplingRate = asset.SamplingRate;
                    frame.SubstreamChannels = asset.Channels;
                    frame.SpeakerMask = asset.SpeakerMask;
                    frame.EmbeddedStereo = asset.EmbeddedStereo;
                }
                assetPosition += assetSizes[i];
                // to the next asset's descriptor, of the size this one says
                bits.Skip(descriptorBits);
            }

            // the duration of the reference clock, at the asset's rate
            if (clock != 0 && frame.SubstreamSamplingRate != 0)
                frame.SubstreamSamples = (int)((long)duration * frame.SubstreamSamplingRate / clock);
        }

        private const DtsComponents SubstreamComponents = DtsComponents.SubstreamCore | DtsComponents.Xbr | DtsComponents.Xxch | DtsComponents.X96 | DtsComponents.Lbr | DtsComponents.Xll;

        private static DtsComponents LowestOf(DtsComponents components) => (DtsComponents)((int)components & -(int)components);

        private static DtsComponents ComponentAt(byte[] buffer, int position, int end)
        {
            uint sync = SyncAt(buffer, position, end);
            foreach (var (word, component) in ComponentSyncWords)
            {
                if (word == sync)
                    return component;
            }
            return DtsComponents.None;
        }

        private struct Asset
        {
            public byte BitDepth;
            public uint SamplingRate;
            public int Channels;
            public ushort SpeakerMask;
            public bool EmbeddedStereo;
            public DtsComponents Components;
        }

        /// <summary>
        /// An asset descriptor (ETSI TS 102 114 7.5.1): of its static fields its bits, rate, channels and speakers, and
        /// past its dynamic metadata its coding mode and components. <paramref name="remainingBits"/> is what is left of the
        /// descriptor, of its size, after what was read; its components none, where it reads past its size.
        /// </summary>
        private static Asset ReadAssetDescriptor(ref AC3Track.BitReader bits, bool staticFields, int[] mixOutChannels, out int remainingBits)
        {
            var asset = new Asset();
            int start = bits.Position;
            int descriptorBits = ((int)bits.Read(9) + 1) * 8;
            bits.Read(3); // the asset's index
            bool stereo = false, sixChannels = false;
            int maskBits = 0;
            if (staticFields)
            {
                if (bits.Read(1) == 1)
                    bits.Read(4); // the asset type
                if (bits.Read(1) == 1)
                    bits.Read(24); // the language
                if (bits.Read(1) == 1)
                    bits.Skip(((int)bits.Read(10) + 1) * 8); // its info text
                asset.BitDepth = (byte)(bits.Read(5) + 1);
                asset.SamplingRate = SubstreamSampleRates[bits.Read(4)];
                asset.Channels = (int)bits.Read(8) + 1;
                if (bits.Read(1) == 1) // one to one of channels to speakers
                {
                    if (asset.Channels > 2)
                        stereo = bits.Read(1) == 1;
                    if (asset.Channels > 6)
                        sixChannels = bits.Read(1) == 1;
                    if (bits.Read(1) == 1)
                    {
                        maskBits = ((int)bits.Read(2) + 1) << 2;
                        asset.SpeakerMask = (ushort)bits.Read(maskBits);
                    }
                    int remaps = (int)bits.Read(3);
                    if (remaps > 0)
                    {
                        if (maskBits == 0)
                            return Unread(ref bits, start, descriptorBits, asset, out remainingBits);
                        var speakers = new int[remaps];
                        for (int i = 0; i < remaps; i++)
                            speakers[i] = ChannelsOf((ushort)bits.Read(maskBits));
                        for (int i = 0; i < remaps; i++)
                        {
                            int decoded = (int)bits.Read(5) + 1;
                            for (int j = 0; j < speakers[i]; j++)
                                bits.Skip(CountOf(bits.Read(decoded)) * 5);
                        }
                    }
                }
                else
                {
                    bits.Read(3); // the representation type
                }
                asset.EmbeddedStereo = stereo;
            }

            // the dynamic metadata
            bool drc = bits.Read(1) == 1;
            if (drc)
                bits.Read(8);
            if (bits.Read(1) == 1)
                bits.Read(5); // dialog normalization
            if (drc && stereo)
                bits.Read(8);
            if (mixOutChannels.Length > 0 && bits.Read(1) == 1)
            {
                bits.Read(1 + 6); // external mixing, post mixing gain
                if (bits.Read(2) == 3)
                    bits.Read(8);
                else
                    bits.Read(3);
                if (bits.Read(1) == 1)
                {
                    foreach (int channels in mixOutChannels)
                        bits.Skip(6 * channels);
                }
                else
                {
                    bits.Skip(6 * mixOutChannels.Length);
                }
                int downmixed = asset.Channels + (sixChannels ? 6 : 0) + (stereo ? 2 : 0);
                foreach (int channels in mixOutChannels)
                {
                    if (channels == 0)
                        return Unread(ref bits, start, descriptorBits, asset, out remainingBits);
                    for (int j = 0; j < downmixed; j++)
                        bits.Skip(CountOf(bits.Read(channels)) * 6);
                }
            }

            // the coding mode, and of mode 0 the components
            asset.Components = bits.Read(2) switch
            {
                0 => (DtsComponents)(bits.Read(12) & 0x3FF),
                1 => DtsComponents.Xll,
                2 => DtsComponents.Lbr,
                _ => DtsComponents.None, // auxiliary coding
            };

            int read = bits.Position - start;
            if (read > descriptorBits)
            {
                asset.Components = DtsComponents.None;
                bits.Skip(-(read - descriptorBits));
                read = descriptorBits;
            }
            remainingBits = descriptorBits - read;
            return asset;
        }

        private static Asset Unread(ref AC3Track.BitReader bits, int start, int descriptorBits, Asset asset, out int remainingBits)
        {
            asset.Components = DtsComponents.None;
            remainingBits = descriptorBits - (bits.Position - start);
            return asset;
        }

        private static int CountOf(uint mask)
        {
            int count = 0;
            for (; mask != 0; mask &= mask - 1)
                count++;
            return count;
        }
    }
}
