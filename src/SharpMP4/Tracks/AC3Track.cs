using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AC-3 Track: an 'ac-3' sample entry and its 'dac3', of the fields of the stream's bit stream information - its rate,
    /// its service, its channels and its bit rate. Each sample a sync frame of 1536 samples.
    /// </summary>
    /// <remarks>ETSI TS 102 366 F.3, F.4</remarks>
    public class AC3Track : TrackBase
    {
        /// <summary>The samples of each channel a sync frame holds: six blocks of 256.</summary>
        public const int AC3_SAMPLES_PER_FRAME = 1536;

        /// <summary>The rate of each fscod (ETSI TS 102 366 4.4.1.3): 3 is reserved.</summary>
        public static readonly uint[] SampleRates = { 48000, 44100, 32000 };

        /// <summary>The full-bandwidth channels of each acmod (ETSI TS 102 366 4.4.2.3): 1+1, 1/0, 2/0, 3/0, 2/1, 3/1, 2/2, 3/2.</summary>
        public static readonly byte[] AcmodChannels = { 2, 1, 2, 3, 3, 4, 4, 5 };

        /// <summary>The bit rate of each frmsizecod / 2 - bit_rate_code - in kbit/s (ETSI TS 102 366 4.4.1.4).</summary>
        public static readonly ushort[] BitRates = { 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384, 448, 512, 576, 640 };

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        public byte Fscod { get; private set; }
        public byte Bsid { get; private set; }
        public byte Bsmod { get; private set; }
        public byte Acmod { get; private set; }
        public bool Lfeon { get; private set; }
        public byte BitRateCode { get; private set; }

        public uint SamplingRate => Fscod < SampleRates.Length ? SampleRates[Fscod] : 0;

        /// <summary>The channels, the LFE among them.</summary>
        public byte ChannelCount => (byte)(AcmodChannels[Acmod & 0x7] + (Lfeon ? 1 : 0));

        /// <summary>The stream's bit rate in kbit/s.</summary>
        public ushort BitRate => BitRateCode < BitRates.Length ? BitRates[BitRateCode] : (ushort)0;

        /// <summary>
        /// Ctor, of the fields of the stream's bit stream information, which <see cref="TryParseSyncFrame"/> reads of a frame.
        /// </summary>
        public AC3Track(byte fscod, byte bsid, byte bsmod, byte acmod, bool lfeon, byte bitRateCode)
        {
            Fscod = fscod;
            Bsid = bsid;
            Bsmod = bsmod;
            Acmod = acmod;
            Lfeon = lfeon;
            BitRateCode = bitRateCode;
            Timescale = SamplingRate;
            DefaultSampleDuration = AC3_SAMPLES_PER_FRAME;
        }

        /// <summary>
        /// Ctor with initialization from the <see cref="SampleEntry"/>.
        /// </summary>
        /// <param name="config">The entry's <see cref="AC3SpecificBox"/>.</param>
        public AC3Track(Box config, uint timescale = 0, int sampleDuration = -1)
        {
            if (config is not AC3SpecificBox dac3)
                throw new ArgumentException($"Invalid AC3SpecificBox: {IsoStream.ToFourCC(config.FourCC)}");

            Fscod = dac3.Fscod;
            Bsid = dac3.Bsid;
            Bsmod = dac3.Bsmod;
            Acmod = dac3.Acmod;
            Lfeon = dac3.Lfeon;
            BitRateCode = dac3.BitRateCode;
            Timescale = timescale == 0 ? SamplingRate : timescale;
            DefaultSampleDuration = sampleDuration > 0 ? sampleDuration : AC3_SAMPLES_PER_FRAME;
        }

        /// <summary>
        /// An AC-3 sync frame's fields (ETSI TS 102 366 4.3): its syncinfo - the sync word 0x0B77, crc1, fscod and frmsizecod
        /// - and the first fields of its bsi, bsid, bsmod, acmod and, after the mix levels acmod has, lfeon. False of no
        /// sync frame, or one of E-AC-3 (bsid above 10).
        /// </summary>
        public static bool TryParseSyncFrame(byte[] frame, int offset, out AC3Track track)
        {
            track = null;
            if (frame == null || offset < 0 || offset + 8 > frame.Length || frame[offset] != 0x0B || frame[offset + 1] != 0x77)
                return false;

            byte fscod = (byte)(frame[offset + 4] >> 6);
            byte frmsizecod = (byte)(frame[offset + 4] & 0x3F);
            var bits = new BitReader(frame, offset + 5);
            byte bsid = (byte)bits.Read(5);
            byte bsmod = (byte)bits.Read(3);
            byte acmod = (byte)bits.Read(3);
            if (fscod == 3 || bsid > 10 || frmsizecod / 2 >= BitRates.Length)
                return false;
            if ((acmod & 0x1) != 0 && acmod != 0x1)
                bits.Read(2); // cmixlev
            if ((acmod & 0x4) != 0)
                bits.Read(2); // surmixlev
            if (acmod == 0x2)
                bits.Read(2); // dsurmod
            bool lfeon = bits.Read(1) == 1;

            track = new AC3Track(fscod, bsid, bsmod, acmod, lfeon, (byte)(frmsizecod / 2));
            return true;
        }

        /// <summary>
        /// The sample goes to the file as it arrives - a sync frame - so this hands back exactly what it was given, without
        /// copying it anywhere.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC("ac-3"));
            audioSampleEntry.Children = new List<Box>();
            audioSampleEntry.Channelcount = 2; // ignored: the 'dac3' says the channels (ETSI TS 102 366 F.3)
            audioSampleEntry.Samplerate = SamplingRate << 16; // 16.16 fixed point
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.Samplesize = 16;
            audioSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            audioSampleEntry.Reserved = new ushort[3]; // TODO simplify API

            var dac3 = new AC3SpecificBox
            {
                Fscod = Fscod,
                Bsid = Bsid,
                Bsmod = Bsmod,
                Acmod = Acmod,
                Lfeon = Lfeon,
                BitRateCode = BitRateCode,
            };
            dac3.SetParent(audioSampleEntry);
            audioSampleEntry.Children.Add(dac3);

            return audioSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 256;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new AC3Track(Fscod, Bsid, Bsmod, Acmod, Lfeon, BitRateCode));
        }

        /// <summary>Bits of a run of bytes, the first the most significant.</summary>
        internal struct BitReader
        {
            private readonly byte[] _data;
            private int _bit;

            public BitReader(byte[] data, int offset)
            {
                _data = data;
                _bit = offset * 8;
            }

            public uint Read(int count)
            {
                uint value = 0;
                for (int i = 0; i < count; i++, _bit++)
                {
                    int index = _bit >> 3;
                    int bit = index < _data.Length ? (_data[index] >> (7 - (_bit & 7))) & 1 : 0;
                    value = (value << 1) | (uint)bit;
                }
                return value;
            }
        }
    }

    /// <summary>
    /// E-AC-3 Track: an 'ec-3' sample entry and its 'dec3', of the stream's data rate and each independent substream's
    /// fields - its rate, its service, its channels and its dependent substreams. Each sample a sync frame, or the frames
    /// of the substreams that play at one time.
    /// </summary>
    /// <remarks>ETSI TS 102 366 F.5, F.6</remarks>
    public class EAC3Track : TrackBase
    {
        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        /// <summary>The stream's data rate, in kbit/s.</summary>
        public ushort DataRate { get; private set; }

        /// <summary>The independent substreams, as the 'dec3' says them: of each its fields.</summary>
        public IReadOnlyList<Substream> Substreams { get; }

        /// <summary>The 'dec3''s bytes after its substreams, as they were read: of Dolby Atmos, its extension and complexity index.</summary>
        public byte[] Extension { get; }

        /// <summary>An independent substream of the 'dec3', and of it the channels its dependent substreams add.</summary>
        public sealed class Substream
        {
            public byte Fscod { get; set; }
            public byte Bsid { get; set; }
            public byte Bsmod { get; set; }
            public byte Acmod { get; set; }
            public bool Lfeon { get; set; }
            public byte NumDepSub { get; set; }

            /// <summary>Of a substream with dependent ones: the channel locations they add (ETSI TS 102 366 E.1.3.1.8, Table F.6.1).</summary>
            public ushort ChanLoc { get; set; }
        }

        /// <summary>The rate of the stream: of its first substream's fscod; 3 - a reduced rate - its entry says.</summary>
        public uint SamplingRate { get; private set; }

        /// <summary>The channels of the first substream and its dependents, the LFE among them.</summary>
        public byte ChannelCount
        {
            get
            {
                var first = Substreams[0];
                int channels = AC3Track.AcmodChannels[first.Acmod & 0x7] + (first.Lfeon ? 1 : 0);
                // each location of chan_loc adds one channel, or a pair (Table F.6.1: Lc/Rc, Lrs/Rrs, Cs, Ts, Lsd/Rsd, Lw/Rw, Lvh/Rvh, Cvh, LFE2)
                int[] perLocation = { 2, 2, 1, 1, 2, 2, 2, 1, 1 };
                for (int i = 0; i < 9; i++)
                    if ((first.ChanLoc & (0x100 >> i)) != 0)
                        channels += perLocation[i];
                return (byte)channels;
            }
        }

        /// <summary>Ctor, of the stream's data rate and its independent substreams' fields.</summary>
        public EAC3Track(ushort dataRate, IEnumerable<Substream> substreams, uint samplingRate = 0, byte[] extension = null)
        {
            DataRate = dataRate;
            Substreams = substreams?.ToList() ?? throw new ArgumentNullException(nameof(substreams));
            if (Substreams.Count == 0)
                throw new ArgumentException("An E-AC-3 stream has one independent substream at least.", nameof(substreams));
            Extension = extension ?? Array.Empty<byte>();
            SamplingRate = samplingRate != 0 ? samplingRate : Substreams[0].Fscod < AC3Track.SampleRates.Length ? AC3Track.SampleRates[Substreams[0].Fscod] : 0;
            Timescale = SamplingRate;
            DefaultSampleDuration = AC3Track.AC3_SAMPLES_PER_FRAME;
        }

        /// <summary>
        /// Ctor with initialization from the <see cref="SampleEntry"/>.
        /// </summary>
        /// <param name="config">The entry's <see cref="EC3SpecificBox"/>.</param>
        public EAC3Track(Box config, uint timescale = 0, int sampleDuration = -1)
            : this(DataRateOf(config), SubstreamsOf(config), RateOf(config), ExtensionOf(config))
        {
            Timescale = timescale == 0 ? SamplingRate : timescale;
            if (sampleDuration > 0)
                DefaultSampleDuration = sampleDuration;
        }

        private static EC3SpecificBox Dec3(Box config) =>
            config as EC3SpecificBox ?? throw new ArgumentException($"Invalid EC3SpecificBox: {IsoStream.ToFourCC(config.FourCC)}");

        private static ushort DataRateOf(Box config) => Dec3(config).DataRate;

        private static IEnumerable<Substream> SubstreamsOf(Box config) =>
            (Dec3(config).Entries ?? Array.Empty<EC3SpecificEntry>()).Select(e => new Substream
            {
                Fscod = e.Fscod,
                Bsid = e.Bsid,
                Bsmod = e.Bsmod,
                Acmod = e.Acmod,
                Lfeon = e.Lfeon,
                NumDepSub = e.NumDepSub,
                ChanLoc = e.ChanLoc,
            });

        /// <summary>The rate of the entry the 'dec3' is in, which is the stream's where fscod cannot say it.</summary>
        private static uint RateOf(Box config) => config.GetParent() switch
        {
            AudioSampleEntryV1 v1 => v1.Samplerate >> 16,
            AudioSampleEntry v0 => v0.Samplerate >> 16,
            _ => 0,
        };

        /// <summary>The 'dec3''s bytes past its substreams: the box's size less what its fields take.</summary>
        private static byte[] ExtensionOf(Box config)
        {
            var padding = Dec3(config).Padding;
            if (padding == null || padding.Length <= 0)
                return Array.Empty<byte>();
            using var memory = new System.IO.MemoryStream();
            new IsoStream(new StreamWrapper(memory)).WriteUInt8ArrayTillEnd(padding, "");
            return memory.ToArray();
        }

        /// <summary>
        /// The sample goes to the file as it arrives - a sync frame, or those of one time - so this hands back exactly what
        /// it was given, without copying it anywhere.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC("ec-3"));
            audioSampleEntry.Children = new List<Box>();
            audioSampleEntry.Channelcount = 2; // ignored: the 'dec3' says the channels (ETSI TS 102 366 F.5)
            audioSampleEntry.Samplerate = SamplingRate << 16; // 16.16 fixed point
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.Samplesize = 16;
            audioSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            audioSampleEntry.Reserved = new ushort[3]; // TODO simplify API

            var dec3 = new EC3SpecificBox
            {
                DataRate = DataRate,
                NumIndSub = (byte)(Substreams.Count - 1),
            };
            dec3.Entries = Substreams.Select(s =>
            {
                var entry = new EC3SpecificEntry
                {
                    Fscod = s.Fscod,
                    Bsid = s.Bsid,
                    Bsmod = s.Bsmod,
                    Acmod = s.Acmod,
                    Lfeon = s.Lfeon,
                    NumDepSub = s.NumDepSub,
                    ChanLoc = s.ChanLoc,
                };
                entry.SetParent(dec3);
                return entry;
            }).ToArray();
            if (Extension.Length > 0)
                dec3.Padding = new StreamMarker(0, Extension.Length, new IsoStream(new StreamWrapper(new System.IO.MemoryStream((byte[])Extension.Clone()))));
            dec3.SetParent(audioSampleEntry);
            audioSampleEntry.Children.Add(dec3);

            return audioSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 256;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new EAC3Track(DataRate, Substreams.Select(s => new Substream
            {
                Fscod = s.Fscod,
                Bsid = s.Bsid,
                Bsmod = s.Bsmod,
                Acmod = s.Acmod,
                Lfeon = s.Lfeon,
                NumDepSub = s.NumDepSub,
                ChanLoc = s.ChanLoc,
            }), SamplingRate, (byte[])Extension.Clone()));
        }
    }
}
