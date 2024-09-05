using System;

namespace SharpMp4
{
    /// <summary>
    /// AAC Track. Supports AAC-LC (Low Complexity) only. Samples should be provided without ADTS header.
    /// </summary>
    public class AACTrack : TrackBase
    {
        public const int AAC_SAMPLE_SIZE = 1024;

        public byte ChannelCount { get; private set; }
        public int SamplingRate { get; private set; }
        public ushort SampleSize { get; private set; }

        public override string HdlrName => HdlrNames.Sound;
        public override string HdlrType => HdlrTypes.Sound;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="channelCount">Number of audio channels.</param>
        /// <param name="samplingRateInHz">Audio sampling rate in HZ. Must be one of the supported sampling rates.</param>
        /// <param name="sampleSizeInBits">Size of 1 sample in bits. </param>
        public AACTrack(byte channelCount, int samplingRateInHz, ushort sampleSizeInBits)
        {
            if (!AudioSpecificConfigDescriptor.SamplingFrequencyMap.ContainsKey(samplingRateInHz))
                throw new ArgumentOutOfRangeException("Invalid sampling rate!");

            if(sampleSizeInBits % 8 != 0) 
                throw new ArgumentOutOfRangeException("Invalid sample size!");

            this.Timescale = (uint)samplingRateInHz;
            this.ChannelCount = channelCount;   
            this.SamplingRate = samplingRateInHz;
            this.SampleSize = sampleSizeInBits;
            this.SampleDuration = AAC_SAMPLE_SIZE; // hardcoded for AAC-LC
        }

        public override Mp4Box CreateSampleEntryBox(Mp4Box parent)
        {
            return CreateAudioSampleEntryBox(parent, this);
        }

        private static AudioSampleEntryBox CreateAudioSampleEntryBox(Mp4Box parent, AACTrack aacTrack)
        {
            AudioSampleEntryBox audioSampleEntry = new AudioSampleEntryBox(0, 0, parent, "mp4a");

            audioSampleEntry.ChannelCount = aacTrack.ChannelCount;
            audioSampleEntry.SampleRate = aacTrack.SamplingRate;
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.SampleSize = aacTrack.SampleSize;

            EsdsBox esds = new EsdsBox(0, 0, audioSampleEntry);
            ESDescriptor descriptor = new ESDescriptor();
            descriptor.EsId = 0;

            DecoderConfigDescriptor decoderConfigDescriptor = new DecoderConfigDescriptor(0x40);
            decoderConfigDescriptor.StreamType = 5;
            decoderConfigDescriptor.MaxBitRate = (uint)aacTrack.SamplingRate;
            decoderConfigDescriptor.AvgBitRate = (uint)aacTrack.SamplingRate;

            AudioSpecificConfigDescriptor audioSpecificConfig = new AudioSpecificConfigDescriptor();
            audioSpecificConfig.GaSpecificConfig = true; 
            audioSpecificConfig.OriginalAudioObjectType = 2; // AAC LC
            audioSpecificConfig.SamplingFrequencyIndex = AudioSpecificConfigDescriptor.SamplingFrequencyMap[aacTrack.SamplingRate];
            audioSpecificConfig.ChannelConfiguration = aacTrack.ChannelCount;
            decoderConfigDescriptor.AudioSpecificConfig = audioSpecificConfig;
            descriptor.Descriptors.Add(decoderConfigDescriptor);

            SLConfigDescriptor slConfigDescriptor = new SLConfigDescriptor();
            slConfigDescriptor.Predefined = 2;
            descriptor.Descriptors.Add(slConfigDescriptor);

            esds.ESDescriptor = descriptor;

            audioSampleEntry.Children.Add(esds);
            return audioSampleEntry;
        }

        public override void FillTkhdBox(TkhdBox tkhd)
        {
            tkhd.Volume = 1;
        }
    }
}
