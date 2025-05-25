using SharpISOBMFF;

namespace SharpMP4
{
    /// <summary>
    /// Opus Track. 
    /// </summary>
    /// <remarks>https://opus-codec.org/docs/opus_in_isobmff.html</remarks>
    public class OpusTrack : TrackBase
    {
        public byte ChannelCount { get; private set; }
        public uint SamplingRate { get; private set; }
        public ushort SampleSize { get; private set; }

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "eng";

        public ushort PreSkip { get; set; } = 3840;
        public short OutputGain { get; set; }
        public byte ChannelMappingFamily { get; set; }
        public byte StreamCount { get; set; }
        public byte CoupledCount { get; set; }
        public byte[] ChannelMapping { get; set; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="channelCount">Number of audio channels.</param>
        /// <param name="preSkip">The PreSkip field indicates the number of the priming samples, that is, the number of samples at 48000 Hz to discard from the decoder output when starting playback.The value of the PreSkip field shall be at least 80 milliseconds' worth of PCM samples even when removing any number of Opus samples which may or may not contain the priming samples.The PreSkip field is not used for discarding the priming samples at the whole playback at all since it is informative only, and that task falls on the Edit List Box.</param>
        /// <param name="outputGain">The OutputGain field shall be set to the same value as the *Output Gain* field in the identification header define in Ogg Opus[3]. Note that the value is stored as 8.8 fixed-point.</param>
        /// <param name="channelMappingFamily">The ChannelMappingFamily field shall be set to the same value as the *Channel Mapping Family* field in the identification header defined in Ogg Opus.</param>
        /// <param name="streamCount">The StreamCount field shall be set to the same value as the *Stream Count* field in the identification header defined in Ogg Opus.</param>
        /// <param name="coupledCount">The CoupledCount field shall be set to the same value as the *Coupled Count* field in the identification header defined in Ogg Opus.</param>
        /// <param name="channelMapping">The ChannelMapping field shall be set to the same octet string as *Channel Mapping* field in the identification header defined in Ogg Opus.</param>
        public OpusTrack(byte channelCount, ushort preSkip, short outputGain, byte channelMappingFamily, byte streamCount = 0, byte coupledCount = 0, byte[] channelMapping = null)
        {
            ChannelCount = channelCount;
            SamplingRate = 48000;
            SampleSize = 16;

            PreSkip = preSkip;
            OutputGain = outputGain;
            StreamCount = streamCount;
            CoupledCount = coupledCount;
            ChannelMappingFamily = channelMappingFamily;
            ChannelMapping = channelMapping;
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC("Opus"));
            audioSampleEntry.Channelcount = this.ChannelCount;
            audioSampleEntry.Samplerate = this.SamplingRate;
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.Samplesize = this.SampleSize;

            // TODO: It should be possible to parse the Opus payload and get these parameters
            OpusSpecificBox opusSpecificBox = new OpusSpecificBox();
            opusSpecificBox.SetParent(audioSampleEntry);
            audioSampleEntry.Children.Add(opusSpecificBox);

            opusSpecificBox._Version = 0; // Opus Specific Box version 0
            opusSpecificBox._OutputChannelCount = this.ChannelCount;
            opusSpecificBox._PreSkip = this.PreSkip;
            opusSpecificBox._InputSampleRate = this.SamplingRate;
            opusSpecificBox._OutputGain = this.OutputGain; // 8.8 fixed-point
            opusSpecificBox._ChannelMappingFamily = this.ChannelMappingFamily;
            opusSpecificBox._ChannelMappingTable = new ChannelMappingTable(this.ChannelCount)
            {
                _StreamCount = this.StreamCount,
                _CoupledCount = this.CoupledCount,
                _ChannelMapping = this.ChannelMapping
            };

            return audioSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 1;
        }
    }
}
