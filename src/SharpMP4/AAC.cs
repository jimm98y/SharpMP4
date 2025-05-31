using SharpISOBMFF;
using System;
using System.Collections.Generic;

namespace SharpMP4
{
    /// <summary>
    /// AAC Track. Supports AAC-LC (Low Complexity) only. Samples should be provided without ADTS header.
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

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "eng";

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="channelCount">Number of audio channels.</param>
        /// <param name="samplingRateInHz">Audio sampling rate in HZ. Must be one of the supported sampling rates.</param>
        /// <param name="sampleSizeInBits">Size of 1 sample in bits. </param>
        public AACTrack(byte channelCount, uint samplingRateInHz, ushort sampleSizeInBits)
        {
            if (!AudioSpecificConfigDescriptor.SamplingFrequencyMap.ContainsKey(samplingRateInHz))
                throw new ArgumentOutOfRangeException("Invalid sampling rate!");

            if(sampleSizeInBits % 8 != 0) 
                throw new ArgumentOutOfRangeException("Invalid sample size!");

            this.Timescale = samplingRateInHz;
            this.ChannelCount = channelCount;   
            this.SamplingRate = samplingRateInHz;
            this.SampleSize = sampleSizeInBits;
            this.SampleDuration = AAC_SAMPLE_SIZE; // hardcoded for AAC-LC
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC("mp4a"));
            audioSampleEntry.Children = new List<Box>();

            audioSampleEntry.Channelcount = this.ChannelCount;
            audioSampleEntry.Samplerate = this.SamplingRate << 16; // TODO simplify API
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.Samplesize = this.SampleSize;
            audioSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            audioSampleEntry.Reserved = new ushort[3]; // TODO simplify API

            ESDBox esds = new ESDBox();
            esds.SetParent(audioSampleEntry);
            audioSampleEntry.Children.Add(esds);

            ES_Descriptor descriptor = new ES_Descriptor();
            descriptor.Children = new List<Descriptor>();
            esds._ES = descriptor;
            descriptor.ESID = 0;

            DecoderConfigDescriptor decoderConfigDescriptor = new DecoderConfigDescriptor();
            decoderConfigDescriptor.SetParent(descriptor);
            descriptor.Children.Add(decoderConfigDescriptor);
            decoderConfigDescriptor.Children = new List<Descriptor>();
            decoderConfigDescriptor.ObjectTypeIndication = AAC_OBJECT_TYPE_INDICATION; // AAC LC
            decoderConfigDescriptor.StreamType = AAC_STREAM_TYPE;
            decoderConfigDescriptor.MaxBitrate = 0; // this.SamplingRate;
            decoderConfigDescriptor.AvgBitrate = 64000; // TODO: this.SamplingRate;
            decoderConfigDescriptor.BufferSizeDB = 6144; // TODO: ???

            AudioSpecificConfig audioSpecificConfig = new AudioSpecificConfig() 
            {
                SamplingFrequencyIndex = (byte)AudioSpecificConfigDescriptor.SamplingFrequencyMap[this.SamplingRate],
                ChannelConfiguration = this.ChannelCount
            };
            audioSpecificConfig.AudioObjectType = new GetAudioObjectType() { AudioObjectType = AAC_AUDIO_OBJECT_TYPE }; // TODO simplify API
            audioSpecificConfig._GASpecificConfig = new GASpecificConfig((int)AudioSpecificConfigDescriptor.SamplingFrequencyMap[this.SamplingRate], this.ChannelCount, AAC_AUDIO_OBJECT_TYPE);
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

        public override void FillTkhdBox(TrackHeaderBox box)
        {
            box.Volume = 256;
        }
    }
}
