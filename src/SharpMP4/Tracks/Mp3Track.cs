using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// MP3 Track: MPEG-1 or MPEG-2 audio, each sample a frame, in an 'mp4a' sample entry whose 'esds' says the object
    /// type - 0x6B of MPEG-1, 0x69 of MPEG-2 - and has no decoder specific info. Read also of QuickTime's '.mp3' entry,
    /// which has no configuration; written as MP4 has it. Of Layer III, but of Layers I and II alike, which the same entry
    /// carries.
    /// </summary>
    /// <remarks>ISO/IEC 14496-1 7.2.6.6.2; ISO/IEC 11172-3 2.4.2.3; ISO/IEC 13818-3 2.4.2.3</remarks>
    public class Mp3Track : TrackBase
    {
        public const byte MPEG1_AUDIO_OBJECT_TYPE_INDICATION = 0x6B;
        public const byte MPEG2_AUDIO_OBJECT_TYPE_INDICATION = 0x69;
        public const byte MPEG4_AUDIO_OBJECT_TYPE_INDICATION = 0x40;
        public const byte AUDIO_STREAM_TYPE = 5;

        /// <summary>The samples of each channel a Layer III frame holds: of MPEG-1, and of MPEG-2's lower rates.</summary>
        public const int MPEG1_SAMPLES_PER_FRAME = 1152;
        public const int MPEG2_SAMPLES_PER_FRAME = 576;

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        public byte ChannelCount { get; private set; }
        public uint SamplingRate { get; private set; }
        public ushort SampleSize { get; private set; } = 16;

        /// <summary>The 'esds' object type: <see cref="MPEG1_AUDIO_OBJECT_TYPE_INDICATION"/> or <see cref="MPEG2_AUDIO_OBJECT_TYPE_INDICATION"/>.</summary>
        public byte ObjectTypeIndication { get; private set; }

        /// <summary>The 'esds''s peak and average bit rate, in bits a second, and decoder buffer size: as a file has them, else 0.</summary>
        public uint MaxBitrate { get; set; }
        public uint AvgBitrate { get; set; }
        public uint BufferSizeDB { get; set; }

        /// <summary>
        /// Ctor, of the stream's channels and rate: MPEG-1's rates - 32000, 44100, 48000 - are of MPEG-1, the rest of MPEG-2,
        /// and a frame of Layer III of 1152 samples or of 576.
        /// </summary>
        public Mp3Track(byte channelCount, uint samplingRate)
        {
            ChannelCount = channelCount;
            SamplingRate = samplingRate;
            bool mpeg1 = IsMpeg1Rate(samplingRate);
            ObjectTypeIndication = mpeg1 ? MPEG1_AUDIO_OBJECT_TYPE_INDICATION : MPEG2_AUDIO_OBJECT_TYPE_INDICATION;
            Timescale = samplingRate;
            DefaultSampleDuration = mpeg1 ? MPEG1_SAMPLES_PER_FRAME : MPEG2_SAMPLES_PER_FRAME;
        }

        /// <summary>
        /// Ctor with initialization from the <see cref="SampleEntry"/>.
        /// </summary>
        /// <param name="config">The entry's 'esds'; of QuickTime's '.mp3' entry, which has none, whichever box it has, or itself.</param>
        public Mp3Track(Box config, uint timescale = 0, int sampleDuration = -1)
        {
            Box entry = config is SampleEntry ? config : config?.GetParent() as Box;
            switch (entry)
            {
                case AudioSampleEntryV1 v1:
                    ChannelCount = (byte)v1.Channelcount;
                    SamplingRate = v1.Samplerate >> 16;
                    break;
                case AudioSampleEntry v0:
                    ChannelCount = (byte)v0.Channelcount;
                    SamplingRate = v0.Samplerate >> 16;
                    break;
                default:
                    throw new NotSupportedException($"No audio sample entry of the MP3 track: {IsoStream.ToFourCC(config.FourCC)}");
            }

            if (DecoderConfigOf(config) is DecoderConfigDescriptor decoderConfig)
            {
                MaxBitrate = decoderConfig.MaxBitrate;
                AvgBitrate = decoderConfig.AvgBitrate;
                BufferSizeDB = decoderConfig.BufferSizeDB;
            }

            ObjectTypeIndication = ObjectTypeIndicationOf(config) switch
            {
                MPEG2_AUDIO_OBJECT_TYPE_INDICATION => MPEG2_AUDIO_OBJECT_TYPE_INDICATION,
                MPEG1_AUDIO_OBJECT_TYPE_INDICATION => MPEG1_AUDIO_OBJECT_TYPE_INDICATION,
                _ => IsMpeg1Rate(SamplingRate) ? MPEG1_AUDIO_OBJECT_TYPE_INDICATION : MPEG2_AUDIO_OBJECT_TYPE_INDICATION,
            };

            Timescale = timescale == 0 ? SamplingRate : timescale;
            DefaultSampleDuration = sampleDuration > 0 ? sampleDuration
                : ObjectTypeIndication == MPEG1_AUDIO_OBJECT_TYPE_INDICATION ? MPEG1_SAMPLES_PER_FRAME : MPEG2_SAMPLES_PER_FRAME;
        }

        private static bool IsMpeg1Rate(uint rate) => rate == 32000 || rate == 44100 || rate == 48000;

        /// <summary>
        /// Whether a box is of MPEG-1 or MPEG-2 audio: an 'esds' of its object type, or of MPEG-4 Audio's of Layer I, II or
        /// III (audioObjectType 32 to 34); or QuickTime's '.mp3' entry, or a box of it.
        /// </summary>
        public static bool IsMp3(Box config)
        {
            if (config == null)
                return false;
            Box entry = config is SampleEntry ? config : config.GetParent() as Box;
            if (entry != null && IsQuickTimeMp3(Encryption.SubsampleSplitter.CodingOf(entry)))
                return true;

            byte oti = ObjectTypeIndicationOf(config);
            if (oti == MPEG1_AUDIO_OBJECT_TYPE_INDICATION || oti == MPEG2_AUDIO_OBJECT_TYPE_INDICATION)
                return true;
            if (oti == MPEG4_AUDIO_OBJECT_TYPE_INDICATION && DecoderSpecificInfoOf(config) is byte[] info && info.Length > 1 && info[0] >> 3 == 31)
            {
                int objectType = 32 + (((info[0] & 0x7) << 3) | (info[1] >> 5));
                return objectType >= 32 && objectType <= 34;
            }
            return false;
        }

        /// <summary>QuickTime's codings of MP3: '.mp3', and 'ms\0U', of its WAVE format tag 0x55.</summary>
        public static bool IsQuickTimeMp3(string coding) => coding == ".mp3" || coding == "ms\0U";

        private static DecoderConfigDescriptor DecoderConfigOf(Box config)
        {
            ESDBox esd = config as ESDBox ?? config?.Children?.OfType<ESDBox>().FirstOrDefault();
            return esd?._ES?.Children?.OfType<DecoderConfigDescriptor>().FirstOrDefault();
        }

        private static byte ObjectTypeIndicationOf(Box config) => DecoderConfigOf(config)?.ObjectTypeIndication ?? 0;

        private static byte[] DecoderSpecificInfoOf(Box config) =>
            DecoderConfigOf(config) is DecoderConfigDescriptor decoderConfig ? AACTrack.DecoderSpecificInfoOf(decoderConfig) : null;

        /// <summary>
        /// The frames of a run of MPEG audio - what an encoder hands out, of one or more frames - each a sample: a frame is
        /// known by its sync word and the length its header gives. Bytes before the first frame, or of no frame, are skipped.
        /// </summary>
        public IEnumerable<ArraySegment<byte>> ParseFrames(byte[] buffer, int offset, int length)
        {
            int end = offset + length;
            int position = offset;
            while (position + 4 <= end)
            {
                if (!TryParseFrameHeader(buffer, position, out int frameLength, out _, out _, out _) || position + frameLength > end)
                {
                    position++;
                    continue;
                }
                yield return new ArraySegment<byte>(buffer, position, frameLength);
                position += frameLength;
            }
        }

        private static readonly int[,] Bitrates =
        {
            // MPEG-1 Layer I, II, III; MPEG-2 and 2.5 Layer I, Layer II and III - in kbit/s, of index 1 to 14
            { 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448 },
            { 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384 },
            { 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 },
            { 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256 },
            { 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 },
        };

        private static readonly uint[] SampleRates = { 44100, 48000, 32000 };

        /// <summary>
        /// The header of an MPEG audio frame at <paramref name="offset"/> (ISO/IEC 11172-3 2.4.2.3, ISO/IEC 13818-3 2.4.2.3, and MPEG-2.5's): its length in
        /// bytes, padding and all, its rate, its channels and the samples of each it holds. False where the four bytes are
        /// no header - no sync, or a reserved or free bit rate, rate or layer.
        /// </summary>
        public static bool TryParseFrameHeader(byte[] buffer, int offset, out int frameLength, out uint samplingRate, out int channels, out int samplesPerFrame)
        {
            frameLength = 0; samplingRate = 0; channels = 0; samplesPerFrame = 0;
            if (buffer == null || offset < 0 || offset + 4 > buffer.Length || buffer[offset] != 0xFF || (buffer[offset + 1] & 0xE0) != 0xE0)
                return false;

            byte b1 = buffer[offset + 1], b2 = buffer[offset + 2], b3 = buffer[offset + 3];

            int version = (b1 >> 3) & 0x3; // 0: MPEG-2.5, 2: MPEG-2, 3: MPEG-1
            int layer = (b1 >> 1) & 0x3;   // 1: III, 2: II, 3: I
            int bitrateIndex = b2 >> 4;
            int rateIndex = (b2 >> 2) & 0x3;
            int padding = (b2 >> 1) & 0x1;
            if (version == 1 || layer == 0 || bitrateIndex == 0 || bitrateIndex == 15 || rateIndex == 3)
                return false;

            bool mpeg1 = version == 3;
            int table = mpeg1 ? 3 - layer : (layer == 3 ? 3 : 4);
            int bitrate = Bitrates[table, bitrateIndex - 1] * 1000;
            samplingRate = SampleRates[rateIndex] >> (mpeg1 ? 0 : version == 2 ? 1 : 2);
            channels = (b3 >> 6) == 3 ? 1 : 2;

            if (layer == 3)
            {
                samplesPerFrame = 384;
                frameLength = (12 * bitrate / (int)samplingRate + padding) * 4;
            }
            else
            {
                samplesPerFrame = layer == 2 ? 1152 : mpeg1 ? 1152 : 576;
                frameLength = samplesPerFrame / 8 * bitrate / (int)samplingRate + padding;
            }
            return true;
        }

        /// <summary>
        /// The sample goes to the file as it arrives - a frame - so this hands back exactly what it was given, without
        /// copying it anywhere. An encoder's output of several frames is split by <see cref="ParseFrames"/> first.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC("mp4a"));
            audioSampleEntry.Children = new List<Box>();
            audioSampleEntry.Channelcount = ChannelCount;
            audioSampleEntry.Samplerate = SamplingRate << 16; // 16.16 fixed point
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.Samplesize = SampleSize;
            audioSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            audioSampleEntry.Reserved = new ushort[3]; // TODO simplify API

            ESDBox esds = new ESDBox();
            esds.SetParent(audioSampleEntry);
            audioSampleEntry.Children.Add(esds);

            ES_Descriptor descriptor = new ES_Descriptor();
            descriptor.Children = new List<Descriptor>();
            esds._ES = descriptor;
            descriptor.ESID = (ushort)TrackID;

            // of no decoder specific info: MPEG-1 and MPEG-2 audio say all in each frame's header
            DecoderConfigDescriptor decoderConfigDescriptor = new DecoderConfigDescriptor();
            decoderConfigDescriptor.SetParent(descriptor);
            descriptor.Children.Add(decoderConfigDescriptor);
            decoderConfigDescriptor.Children = new List<Descriptor>();
            decoderConfigDescriptor.ObjectTypeIndication = ObjectTypeIndication;
            decoderConfigDescriptor.StreamType = AUDIO_STREAM_TYPE;
            decoderConfigDescriptor.MaxBitrate = MaxBitrate;
            decoderConfigDescriptor.AvgBitrate = AvgBitrate;
            decoderConfigDescriptor.BufferSizeDB = BufferSizeDB;

            SLConfigDescriptor slConfigDescriptor = new SLConfigDescriptor();
            slConfigDescriptor.Predefined = 2;
            slConfigDescriptor.Ocr = new byte[0]; // TODO simplify API
            slConfigDescriptor.UseTimeStampsFlag = true; // TODO simplify API
            slConfigDescriptor.SetParent(descriptor);
            descriptor.Children.Add(slConfigDescriptor);

            return audioSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 256;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new Mp3Track(ChannelCount, SamplingRate)
            {
                ObjectTypeIndication = ObjectTypeIndication,
                SampleSize = SampleSize,
                DefaultSampleDuration = DefaultSampleDuration,
                MaxBitrate = MaxBitrate,
                AvgBitrate = AvgBitrate,
                BufferSizeDB = BufferSizeDB,
            });
        }
    }
}
