using System;
using System.Collections.Generic;

namespace SharpMp4
{
    /// <summary>
    /// AAC Track. Supports AAC-LC (Low Complexity) only. Samples should be provided without ADTS header.
    /// </summary>
    public class AACTrack : TrackBase
    {
        public byte ChannelCount { get; private set; }
        public int SamplingRate { get; private set; }
        public ushort SampleSize { get; private set; }

        public override string HdlrName => HdlrNames.Sound;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="channelCount">Number of audio channels.</param>
        /// <param name="samplingRateInHz">Audio sampling rate in HZ. Must be one of the supported sampling rates.</param>
        /// <param name="sampleSizeInBits">Size of 1 sample in bits. </param>
        public AACTrack(byte channelCount, int samplingRateInHz, ushort sampleSizeInBits)
        {
            if (!AACBoxBuilder.SamplingFrequencyMap.ContainsKey(samplingRateInHz))
                throw new ArgumentOutOfRangeException("Invalid sampling rate!");

            if(sampleSizeInBits % 8 != 0) 
                throw new ArgumentOutOfRangeException("Invalid sample size!");

            base.Handler = "soun";
            this.Timescale = (uint)samplingRateInHz;
            this.ChannelCount = channelCount;   
            this.SamplingRate = samplingRateInHz;
            this.SampleSize = sampleSizeInBits;
            this.SampleDuration = 1024; // hardcoded for AAC-LC
        }

        public override Mp4Box CreateSampleEntryBox(Mp4Box parent)
        {
            return AACBoxBuilder.CreateAudioSampleEntryBox(parent, this);
        }

        public override void FillTkhdBox(TkhdBox tkhd)
        {
            tkhd.Volume = 1;
        }
    }

    public static class AACBoxBuilder
    {
        public static readonly Dictionary<int, int> SamplingFrequencyMap = new Dictionary<int, int>()
        {
            {  0, 96000 },
            {  1, 88200 },
            {  2, 64000 },
            {  3, 48000 },
            {  4, 44100 },
            {  5, 32000 },
            {  6, 24000 },
            {  7, 22050 },
            {  8, 16000 },
            {  9, 12000 },
            { 10, 11025 },
            { 11,  8000 },
            { 96000,  0 },
            { 88200,  1 },
            { 64000,  2 },
            { 48000,  3 },
            { 44100,  4 },
            { 32000,  5 },
            { 24000,  6 },
            { 22050,  7 },
            { 16000,  8 },
            { 12000,  9 },
            { 11025, 10 },
            {  8000, 11 },
        };

        public static AudioSampleEntryBox CreateAudioSampleEntryBox(Mp4Box parent, AACTrack aacTrack)
        {
            AudioSampleEntryBox audioSampleEntry = new AudioSampleEntryBox(0, parent, "mp4a");

            audioSampleEntry.ChannelCount = aacTrack.ChannelCount;
            audioSampleEntry.SampleRate = aacTrack.SamplingRate;
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.SampleSize = aacTrack.SampleSize;

            EsdsBox esds = new EsdsBox(0, audioSampleEntry);
            ESDescriptor descriptor = new ESDescriptor();
            descriptor.EsId = 0;

            DecoderConfigDescriptor decoderConfigDescriptor = new DecoderConfigDescriptor(0x40);
            decoderConfigDescriptor.StreamType = 5;
            decoderConfigDescriptor.MaxBitRate = (uint)aacTrack.SamplingRate;
            decoderConfigDescriptor.AvgBitRate = (uint)aacTrack.SamplingRate;

            AudioSpecificConfigDescriptor audioSpecificConfig = new AudioSpecificConfigDescriptor();
            audioSpecificConfig.GaSpecificConfig = true; // TODO: should be set automatically?
            audioSpecificConfig.OriginalAudioObjectType = 2; // AAC LC
            audioSpecificConfig.SamplingFrequencyIndex = SamplingFrequencyMap[aacTrack.SamplingRate];
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
    }
}
