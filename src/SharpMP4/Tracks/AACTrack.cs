using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AAC Track. Made to be written, it is AAC-LC (Low Complexity); read from a file, it is whatever AAC the file's
    /// AudioSpecificConfig says - HE-AAC, Main, multichannel - which it writes back as it was. Samples should be provided
    /// with or without ADTS header.
    /// </summary>
    public class AACTrack : TrackBase
    {
        public const int AAC_SAMPLE_SIZE = 1024;
        public const int AAC_AUDIO_OBJECT_TYPE = 2; // AAC LC
        public const int AAC_OBJECT_TYPE_INDICATION = 0x40;
        public const int AAC_STREAM_TYPE = 0x05;

        public byte ChannelCount { get; private set; }
        public uint SamplingRate { get; private set; }
        public ushort SampleSize { get; private set; }
        public byte ChannelConfiguration { get; private set; }
        public AudioSpecificConfig AudioSpecificConfig { get; private set; }

        /// <summary>
        /// The decoder specific info - the AudioSpecificConfig - of the file the track was read from, as its bytes: what the
        /// sample entry is written with, so nothing of it the track does not read - SBR, PS, a program config element - is
        /// lost. Null of a track made to be written, whose AAC-LC config is made of its rate and channels.
        /// </summary>
        public byte[] DecoderSpecificInfo { get; private set; }

        /// <summary>The objectTypeIndication of the decoder config: MPEG-4 Audio (0x40), or one of MPEG-2 AAC's.</summary>
        public byte ObjectTypeIndication { get; private set; } = AAC_OBJECT_TYPE_INDICATION;

        // of the decoder config the track was read with, written back as they were
        private uint _maxBitrate;
        private uint _avgBitrate;
        private uint _bufferSizeDB;

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="channelCount">Number of audio channels.</param>
        /// <param name="samplingRateInHz">Audio sampling rate in HZ. Must be one of the supported sampling rates.</param>
        /// <param name="sampleSizeInBits">Size of 1 sample in bits. </param>
        public AACTrack(byte channelCount, uint samplingRateInHz, ushort sampleSizeInBits) :
            this(channelCount, samplingRateInHz, sampleSizeInBits, channelCount)
        {  }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="channelCount">Number of audio channels.</param>
        /// <param name="samplingRateInHz">
        /// Audio sampling rate in HZ: one of the table's (ISO/IEC 14496-3 Table 1.18) is written as its index, any other as
        /// itself, after the escape index 0xF.
        /// </param>
        /// <param name="sampleSizeInBits">Size of 1 sample in bits. </param>
        /// <param name="channelConfiguration">Channel configuration from the ADTS header.</param>
        public AACTrack(byte channelCount, uint samplingRateInHz, ushort sampleSizeInBits, byte channelConfiguration)
        {
            if (samplingRateInHz == 0 || samplingRateInHz > 0xFFFFFF)
                throw new ArgumentOutOfRangeException(nameof(samplingRateInHz), "A sampling rate of AAC is more than 0 and fits 24 bits");

            if(sampleSizeInBits % 8 != 0)
                throw new ArgumentOutOfRangeException("Invalid sample size!");

            Timescale = samplingRateInHz;
            ChannelCount = channelCount;   
            SamplingRate = samplingRateInHz;
            SampleSize = sampleSizeInBits;
            DefaultSampleDuration = AAC_SAMPLE_SIZE; // hardcoded for AAC-LC
            ChannelConfiguration = channelConfiguration;
        }

        /// <summary>
        /// Ctor with initialization from the <see cref="SampleEntry"/>.
        /// </summary>
        /// <param name="config"><see cref="ESDBox"/>.</param>
        public AACTrack(Box config, uint timescale = 0, int sampleDuration = -1)
        {
            ESDBox esd = config as ESDBox;
            if (esd == null)
            {
                // this box can be nested inside a wave box
                if(config.Children != null && config.Children.Count > 0)
                {
                    esd = config.Children.OfType<ESDBox>().SingleOrDefault();
                }
                
                if(esd == null)
                    throw new ArgumentException($"Invalid ESDBox: {config.FourCC}");
            }

            DefaultSampleDuration = sampleDuration <= 0 ? AAC_SAMPLE_SIZE : sampleDuration;

            // the channels as the entry has them: of 5.1, 6, which the entry is written back with
            if (config.GetParent() is AudioSampleEntry audioSampleEntry)
            {
                Timescale = timescale == 0 ? audioSampleEntry.Samplerate >> 16 : timescale;
                ChannelCount = (byte)audioSampleEntry.Channelcount;
                SamplingRate = Timescale;
                SampleSize = audioSampleEntry.Samplesize;
                ChannelConfiguration = (byte)audioSampleEntry.Channelcount;
            }
            else if(config.GetParent() is AudioSampleEntryV1 audioSampleEntryV1)
            {
                Timescale = timescale == 0 ? audioSampleEntryV1.Samplerate >> 16 : timescale;
                ChannelCount = (byte)audioSampleEntryV1.Channelcount;
                SamplingRate = Timescale;
                SampleSize = audioSampleEntryV1.Samplesize;
                ChannelConfiguration = (byte)audioSampleEntryV1.Channelcount;
            }
            else
            {
                throw new NotSupportedException("Parent box is not AudioSampleEntry!");
            }

            DecoderConfigDescriptor decoderConfigDescriptor = esd._ES.Children.OfType<DecoderConfigDescriptor>().SingleOrDefault();
            if (decoderConfigDescriptor != null)
            {
                ObjectTypeIndication = decoderConfigDescriptor.ObjectTypeIndication;
                _maxBitrate = decoderConfigDescriptor.MaxBitrate;
                _avgBitrate = decoderConfigDescriptor.AvgBitrate;
                _bufferSizeDB = decoderConfigDescriptor.BufferSizeDB;
                DecoderSpecificInfo = DecoderSpecificInfoOf(decoderConfigDescriptor);

                AudioSpecificConfig audioSpecificConfig = null;
                audioSpecificConfig = decoderConfigDescriptor.Children.OfType<AudioSpecificConfig>().SingleOrDefault();
                if (audioSpecificConfig == null)
                {
                    // TODO: Fix demuxer
                    GenericDecoderSpecificInfo genericDecoderSpecificInfo = decoderConfigDescriptor.Children.OfType<GenericDecoderSpecificInfo>().SingleOrDefault();
                    if(genericDecoderSpecificInfo != null)
                    {
                        using(IsoStream isoStream = new IsoStream(new MemoryStream()))
                        {
                            genericDecoderSpecificInfo.Write(isoStream);
                            isoStream.SeekFromBeginning(0);
                            audioSpecificConfig = new AudioSpecificConfig();
                            try
                            {
                                audioSpecificConfig.Read(isoStream, (ulong)isoStream.GetStreamLength() << 3);
                            }
                            catch (EndOfStreamException)
                            {
                                // a config cut short (mutagen's ep7.m4b ends before the coreCoderDelay it says
                                // follows): what was read before its end stands - the object type, the
                                // sampling frequency and the channel configuration come first
                            }
                        }
                    }
                }

                if (audioSpecificConfig != null)
                {
                    ChannelConfiguration = audioSpecificConfig.ChannelConfiguration;
                    this.AudioSpecificConfig = audioSpecificConfig; // store this for later use, the decoder will need it
                }
            }
        }

        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true; // in case of audio it's implied, no need to signal it

            if (buffer == null)
            {
                output = default;
                return;
            }

            // An ADTS header is not stored, so the sample starts after it. Nothing is copied: what
            // comes back is the bytes where they already are.
            int header = AdtsHeader.GetLength(buffer, offset, length);
            output = new ArraySegment<byte>(buffer, offset + header, length - header);
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC("mp4a"));
            audioSampleEntry.Children = new List<Box>();

            audioSampleEntry.Channelcount = ChannelCount;
            audioSampleEntry.Samplerate = SamplingRate << 16; // TODO simplify API
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

            DecoderConfigDescriptor decoderConfigDescriptor = new DecoderConfigDescriptor();
            decoderConfigDescriptor.SetParent(descriptor);
            descriptor.Children.Add(decoderConfigDescriptor);
            decoderConfigDescriptor.Children = new List<Descriptor>();
            decoderConfigDescriptor.ObjectTypeIndication = ObjectTypeIndication;
            decoderConfigDescriptor.StreamType = AAC_STREAM_TYPE;
            decoderConfigDescriptor.MaxBitrate = _maxBitrate;
            decoderConfigDescriptor.AvgBitrate = _avgBitrate;
            decoderConfigDescriptor.BufferSizeDB = _bufferSizeDB;

            if (DecoderSpecificInfo != null)
            {
                // read from a file: its config as it was, whatever of it the track does not read
                var decoderSpecificInfo = new GenericDecoderSpecificInfo { Data = (byte[])DecoderSpecificInfo.Clone() };
                decoderSpecificInfo.SetParent(decoderConfigDescriptor);
                decoderConfigDescriptor.Children.Add(decoderSpecificInfo);
            }
            else
            {
                // a rate the table has goes as its index; any other as itself, after the escape index (ISO/IEC 14496-3 1.6.2.1)
                bool tabled = AudioSpecificConfigDescriptor.SamplingFrequencyMap.TryGetValue(SamplingRate, out uint samplingFrequencyIndex);
                if (!tabled)
                    samplingFrequencyIndex = 0xF;

                AudioSpecificConfig = new AudioSpecificConfig()
                {
                    SamplingFrequencyIndex = (byte)samplingFrequencyIndex,
                    SamplingFrequency = tabled ? 0 : SamplingRate,
                    ChannelConfiguration = ChannelConfiguration // TODO: from the ADTS header
                };
                AudioSpecificConfig.AudioObjectType = new GetAudioObjectType() { AudioObjectType = AAC_AUDIO_OBJECT_TYPE }; // TODO simplify API
                AudioSpecificConfig._GASpecificConfig = new GASpecificConfig((int)samplingFrequencyIndex, ChannelCount, AAC_AUDIO_OBJECT_TYPE);
                AudioSpecificConfig.SetParent(decoderConfigDescriptor);
                decoderConfigDescriptor.Children.Add(AudioSpecificConfig);
            }

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
            return CopySettingsTo(new AACTrack(ChannelCount, SamplingRate, SampleSize, ChannelConfiguration)
            {
                AudioSpecificConfig = AudioSpecificConfig,
                DecoderSpecificInfo = DecoderSpecificInfo,
                ObjectTypeIndication = ObjectTypeIndication,
                _maxBitrate = _maxBitrate,
                _avgBitrate = _avgBitrate,
                _bufferSizeDB = _bufferSizeDB,
            });
        }

        /// <summary>The object types of MPEG-4 Audio of AAC (ISO/IEC 14496-3 Table 1.17), whose samples this track takes.</summary>
        private static readonly HashSet<int> AacObjectTypes = new HashSet<int>
        {
            1,  // AAC Main
            2,  // AAC LC
            3,  // AAC SSR
            4,  // AAC LTP
            5,  // SBR, HE-AAC
            6,  // AAC Scalable
            17, // ER AAC LC
            19, // ER AAC LTP
            20, // ER AAC Scalable
            22, // ER BSAC
            23, // ER AAC LD
            29, // PS, HE-AAC v2
            39, // ER AAC ELD
            42, // USAC
        };

        /// <summary>
        /// Whether an 'esds' - or the 'wave' of QuickTime it is in - is of AAC: of MPEG-2 AAC's object type indications
        /// (0x66 to 0x68), or MPEG-4 Audio's (0x40) of an object type of AAC. Others - MP3 (0x69, 0x6B), and MPEG-4 Audio
        /// of MPEG-1 layers, ALS and the like - are not: their samples are not AAC's, an ADTS header taken off them would
        /// cut a frame of theirs.
        /// </summary>
        public static bool IsAac(Box config)
        {
            ESDBox esd = config as ESDBox ?? config?.Children?.OfType<ESDBox>().FirstOrDefault();
            DecoderConfigDescriptor decoderConfig = esd?._ES?.Children?.OfType<DecoderConfigDescriptor>().FirstOrDefault();
            if (decoderConfig == null)
                return false;

            switch (decoderConfig.ObjectTypeIndication)
            {
                case 0x66: // MPEG-2 AAC Main
                case 0x67: // MPEG-2 AAC LC
                case 0x68: // MPEG-2 AAC SSR
                    return true;
                case AAC_OBJECT_TYPE_INDICATION:
                    // the object type of the AudioSpecificConfig: its first five bits, or after them six more (1.6.2.1)
                    byte[] info = DecoderSpecificInfoOf(decoderConfig);
                    if (info == null || info.Length == 0)
                        return true;
                    int objectType = info[0] >> 3;
                    if (objectType == 31)
                        objectType = info.Length > 1 ? 32 + (((info[0] & 0x7) << 3) | (info[1] >> 5)) : -1;
                    return AacObjectTypes.Contains(objectType);
                default:
                    return false;
            }
        }

        /// <summary>The decoder specific info of a decoder config as its bytes, after its tag and size; null where it has none.</summary>
        internal static byte[] DecoderSpecificInfoOf(DecoderConfigDescriptor decoderConfig)
        {
            Descriptor info = decoderConfig.Children?.FirstOrDefault(x => x.Tag == DescriptorTags.DecSpecificInfoTag);
            if (info == null)
                return null;

            byte[] bytes;
            using (var memory = new MemoryStream())
            {
                new IsoStream(new StreamWrapper(memory)).WriteDescriptor(info, "");
                bytes = memory.ToArray();
            }

            // the tag, then the size, in bytes of seven bits each but the last with its top bit set
            int start = 1;
            while (start < bytes.Length && (bytes[start] & 0x80) != 0)
                start++;
            start++;
            if (start >= bytes.Length)
                return Array.Empty<byte>();
            var payload = new byte[bytes.Length - start];
            Buffer.BlockCopy(bytes, start, payload, 0, payload.Length);
            return payload;
        }
    }

    /// <summary>
    /// AAC ADTS header.
    /// </summary>
    public class AdtsHeader
    {
        public bool MpegVersion { get; set; } // set to 0 for MPEG-4 and 1 for MPEG-2
        public byte Layer { get; set; } // always set to 0
        public bool ProtectionAbsent { get; set; } // 1 if there is no CRC, 0 otherwise
        public byte Profile { get; set; }
        public byte SamplingFrequencyIndex { get; set; } // 15 is forbidden
        public bool PrivateBit { get; set; }
        public byte ChannelConfiguration { get; set; } // in case of 0, it is sent via in-band Program Config Element
        public bool Originality { get; set; }
        public bool Home { get; set; }
        public bool CopyrightIDBit { get; set; }
        public bool CopyrightIDStart { get; set; }
        public ushort FrameLength { get; set; } // lenght of ADTS frame including header and the CRC
        public ushort BufferFullness { get; set; }
        public byte RawDataBlockCountMinus1 { get; set; }
        public ushort CRC { get; set; }

        /// <summary>
        /// Checks whether the AAC sample has ADTS header.
        /// </summary>
        /// <param name="sample">AAC sample bytes.</param>
        /// <returns>true when the ADTS header is present, false otherwise.</returns>
        public static bool HasHeader(byte[] sample) => HasHeader(sample, 0, sample.Length);

        public static bool HasHeader(byte[] buffer, int offset, int length)
        {
            return length > 7 && buffer[offset] == 0xFF && buffer[offset + 1] >> 4 == 0xF;
        }

        /// <summary>
        /// Returns the length of the ADTS header if present, 0 otherwise.
        /// </summary>
        /// <param name="sample">AAC sample bytes.</param>
        /// <returns>Length in bytes of the ADTS header if present, 0 otherwise.</returns>
        public static int GetLength(byte[] sample) => GetLength(sample, 0, sample.Length);

        public static int GetLength(byte[] buffer, int offset, int length)
        {
            if (!HasHeader(buffer, offset, length))
                return 0;

            return ((buffer[offset + 1] >> 4) & 0x1) == 1 ? 9 : 7;
        }

        /// <summary>
        /// Reads the ADTS header.
        /// </summary>
        /// <param name="sample">AAC sample bytes.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the ADTS header is not present.</exception>
        public void Read(byte[] sample)
        {
            if (sample.Length <= 7)
                throw new ArgumentOutOfRangeException(nameof(sample));

            // sync word - 12 bits set to 1
            if (!(sample[0] == 0xFF && sample[1] >> 4 == 0xF))
                throw new ArgumentOutOfRangeException();

            // fixed header:
            // 1 bit ID, 2 bits layer, 1 bit protection absent
            int i = sample[1];
            MpegVersion = ((i >> 3) & 0x1) == 1;
            Layer = (byte)((i >> 1) & 0x3);
            ProtectionAbsent = (i & 0x1) == 1;

            // 2 bits profile, 4 bits sample frequency, 1 bit private bit
            i = sample[2];
            Profile = (byte)(((i >> 6) & 0x3) + 1);
            SamplingFrequencyIndex = (byte)((i >> 2) & 0xF);
            PrivateBit = ((i >> 1) & 0x1) == 1;

            // 3 bits channel configuration, 1 bit copy, 1 bit home
            i = (i << 8) | sample[3];
            ChannelConfiguration = (byte)((i >> 6) & 0x7);
            Originality = ((i >> 5) & 0x1) == 1;
            Home = ((i >> 4) & 0x1) == 1;

            // variable header:
            // 1 bit copyrightIDBit, 1 bit copyrightIDStart, 13 bits frame length,
            // 11 bits adtsBufferFullness, 2 bits rawDataBlockCount
            CopyrightIDBit = ((i >> 3) & 0x1) == 1;
            CopyrightIDStart = ((i >> 2) & 0x1) == 1;
            i = (i << 16) | sample[4] << 8 | sample[5];
            FrameLength = (ushort)((i >> 5) & 0x1FFF);
            i = (i << 8) | sample[6];
            BufferFullness = (ushort)((i >> 2) & 0x7FF);
            RawDataBlockCountMinus1 = (byte)(i & 0x3);

            if(!ProtectionAbsent)
            {
                CRC = (ushort)(sample[7] << 8 | sample[8]);
            }
        }
    }
}
