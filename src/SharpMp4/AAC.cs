using System.Collections.Generic;

namespace SharpMp4
{
    public class AACTrack : TrackBase
    {
        public AACTrack(uint trackID, ushort channelCount, int sampleRate, ushort sampleSize) : base(trackID)
        {
            base.Handler = "soun";
            this.Timescale = (uint)sampleRate;
            this.ChannelCount = channelCount;   
            this.SampleRate = sampleRate;
            this.SampleSize = sampleSize;
            this.SampleDuration = 1000;
        }

        public ushort ChannelCount { get; private set; }
        public int SampleRate { get; private set; }
        public ushort SampleSize { get; private set; }
    }

    public static class AACBoxBuilder
    {
        private static readonly Dictionary<int, int> SamplingFrequencyMap = new Dictionary<int, int>()
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
            audioSampleEntry.SampleRate = aacTrack.SampleRate;
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.SampleSize = aacTrack.SampleSize;

            EsdsBox esds = new EsdsBox(0, audioSampleEntry);
            ESDescriptor descriptor = new ESDescriptor();
            descriptor.EsId = 0;

            DecoderConfigDescriptor decoderConfigDescriptor = new DecoderConfigDescriptor(0x40);
            decoderConfigDescriptor.StreamType = 5;
            decoderConfigDescriptor.MaxBitRate = (uint)aacTrack.SampleRate;
            decoderConfigDescriptor.AvgBitRate = (uint)aacTrack.SampleRate;

            AudioSpecificConfigDescriptor audioSpecificConfig = new AudioSpecificConfigDescriptor();
            audioSpecificConfig.GaSpecificConfig = true; // TODO: should be set automatically?
            audioSpecificConfig.OriginalAudioObjectType = 2; // AAC LC
            audioSpecificConfig.SamplingFrequencyIndex = SamplingFrequencyMap[aacTrack.SampleRate];
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
