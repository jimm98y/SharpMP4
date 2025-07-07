using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AAC Track. Supports AAC-LC (Low Complexity) only. Samples should be provided with or without ADTS header.
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

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "eng";

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
        /// <param name="samplingRateInHz">Audio sampling rate in HZ. Must be one of the supported sampling rates.</param>
        /// <param name="sampleSizeInBits">Size of 1 sample in bits. </param>
        /// <param name="channelConfiguration">Channel configuration from the ADTS header.</param>
        public AACTrack(byte channelCount, uint samplingRateInHz, ushort sampleSizeInBits, byte channelConfiguration)
        {
            if (!AudioSpecificConfigDescriptor.SamplingFrequencyMap.ContainsKey(samplingRateInHz))
                throw new ArgumentOutOfRangeException("Invalid sampling rate!");

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
        /// <param name="sampleEntry"><see cref="SampleEntry"/>.</param>
        public AACTrack(Box sampleEntry, uint timescale = 0, int sampleDuration = -1)
        {
            DefaultSampleDuration = sampleDuration <= 0 ? AAC_SAMPLE_SIZE : sampleDuration;

            ESDBox esd = null;

            if (sampleEntry is AudioSampleEntry audioSampleEntry)
            {
                Timescale = timescale == 0 ? audioSampleEntry.Samplerate >> 16 : timescale; 
                ChannelCount = (byte)audioSampleEntry.Channelcount;
                SamplingRate = audioSampleEntry.Samplerate >> 16;
                SampleSize = audioSampleEntry.Samplesize;
                ChannelConfiguration = (byte)audioSampleEntry.Channelcount;
                esd = sampleEntry.Children.OfType<ESDBox>().Single();                       
            }
            else if(sampleEntry is AudioSampleEntryV1 audioSampleEntryV1)
            {
                Timescale = audioSampleEntryV1.Samplerate >> 16;
                ChannelCount = (byte)audioSampleEntryV1.Channelcount;
                SamplingRate = audioSampleEntryV1.Samplerate >> 16;
                SampleSize = audioSampleEntryV1.Samplesize;
                ChannelConfiguration = (byte)audioSampleEntryV1.Channelcount;
                esd = sampleEntry.Children.OfType<ESDBox>().Single();
            }
            else
            {
                throw new NotSupportedException();
            }

            if (esd != null)
            {
                DecoderConfigDescriptor decoderConfigDescriptor = esd._ES.Children.OfType<DecoderConfigDescriptor>().SingleOrDefault();
                if (decoderConfigDescriptor != null)
                {
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
                                audioSpecificConfig.Read(isoStream, (ulong)isoStream.GetStreamLength() << 3);
                            }
                        }
                    }

                    if (audioSpecificConfig != null)
                    {
                        ChannelConfiguration = audioSpecificConfig.ChannelConfiguration;
                    }
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public override void ProcessSample(byte[] sample, out byte[] output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true; // in case of audio it's implied, no need to signal it

            if(sample == null)
            {
                output = null;
                return;
            }

            if (AdtsHeader.HasHeader(sample))
            {
                //var header = new AdtsHeader();
                //header.Read(sample);

                // strip ADTS header
                output = sample.Skip(AdtsHeader.GetLength(sample)).ToArray();
            }
            else
            {
                output = sample;
            }
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
            decoderConfigDescriptor.ObjectTypeIndication = AAC_OBJECT_TYPE_INDICATION; // AAC LC
            decoderConfigDescriptor.StreamType = AAC_STREAM_TYPE;
            decoderConfigDescriptor.MaxBitrate = 0; // this.SamplingRate;
            decoderConfigDescriptor.AvgBitrate = 0; // TODO: this.SamplingRate;
            decoderConfigDescriptor.BufferSizeDB = 0; // TODO: ???

            AudioSpecificConfig audioSpecificConfig = new AudioSpecificConfig() 
            {
                SamplingFrequencyIndex = (byte)AudioSpecificConfigDescriptor.SamplingFrequencyMap[SamplingRate],
                ChannelConfiguration = ChannelConfiguration // TODO: from the ADTS header
            };
            audioSpecificConfig.AudioObjectType = new GetAudioObjectType() { AudioObjectType = AAC_AUDIO_OBJECT_TYPE }; // TODO simplify API
            audioSpecificConfig._GASpecificConfig = new GASpecificConfig((int)AudioSpecificConfigDescriptor.SamplingFrequencyMap[SamplingRate], ChannelCount, AAC_AUDIO_OBJECT_TYPE);
            audioSpecificConfig.SetParent(decoderConfigDescriptor);
            decoderConfigDescriptor.Children.Add(audioSpecificConfig);

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
        public static bool HasHeader(byte[] sample)
        {
            return sample.Length > 7 && sample[0] == 0xFF && sample[1] >> 4 == 0xF;
        }

        /// <summary>
        /// Returns the length of the ADTS header if present, 0 otherwise.
        /// </summary>
        /// <param name="sample">AAC sample bytes.</param>
        /// <returns>Length in bytes of the ADTS header if present, 0 otherwise.</returns>
        public static int GetLength(byte[] sample)
        {
            if (!HasHeader(sample))
                return 0;

            return ((sample[1] >> 4) & 0x1) == 1 ? 9 : 7;
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
