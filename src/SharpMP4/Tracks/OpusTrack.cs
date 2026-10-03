using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// Opus Track.
    /// </summary>
    /// <remarks>https://opus-codec.org/docs/opus_in_isobmff.html</remarks>
    public class OpusTrack : TrackBase
    {
        public const int OPUS_SAMPLE_SIZE = 960; // TODO

        /// <summary>The rate Opus is decoded at, which its sample entry and its media timescale are (Opus in ISOBMFF 4.3).</summary>
        public const uint OPUS_SAMPLE_RATE = 48000;

        public byte ChannelCount { get; private set; }
        public uint SamplingRate { get; private set; }
        public ushort SampleSize { get; private set; }

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        public ushort PreSkip { get; set; } = 3840;
        public short OutputGain { get; set; }
        public byte ChannelMappingFamily { get; set; }
        public byte StreamCount { get; set; }
        public byte CoupledCount { get; set; }
        public byte[] ChannelMapping { get; set; }

        /// <summary>
        /// The rate of the audio the encoder was given, which the 'dOps' box says as the Ogg Opus header does: what a
        /// decoder may resample to, not the rate of the samples, which is 48000 whatever it is.
        /// </summary>
        public uint InputSampleRate { get; set; } = OPUS_SAMPLE_RATE;

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
            SamplingRate = OPUS_SAMPLE_RATE;
            SampleSize = 16;

            // the media timescale is 48000, and a sample of 20 ms the most common (Opus in ISOBMFF 4.3)
            Timescale = OPUS_SAMPLE_RATE;
            DefaultSampleDuration = OPUS_SAMPLE_SIZE;

            PreSkip = preSkip;
            OutputGain = outputGain;
            StreamCount = streamCount;
            CoupledCount = coupledCount;
            ChannelMappingFamily = channelMappingFamily;
            ChannelMapping = channelMapping;
        }

        /// <summary>
        /// Ctor with initialization from the <see cref="SampleEntry"/>.
        /// </summary>
        /// <param name="config"><see cref="SampleEntry"/>.</param>
        public OpusTrack(Box config, uint timescale = 0, int sampleDuration = -1)
        {
            OpusSpecificBox opusSpecificBox = config as OpusSpecificBox;
            if (opusSpecificBox == null)
                throw new ArgumentException($"Invalid OpusSpecificBox: {config.FourCC}");

            DefaultSampleDuration = sampleDuration <= 0 ? OPUS_SAMPLE_SIZE : sampleDuration;

            if (config.GetParent() is AudioSampleEntry audioSampleEntry)
            {
                Timescale = timescale == 0 ? audioSampleEntry.Samplerate >> 16 : timescale;
                ChannelCount = (byte)audioSampleEntry.Channelcount;
                SamplingRate = audioSampleEntry.Samplerate >> 16;
                SampleSize = audioSampleEntry.Samplesize;
                opusSpecificBox = audioSampleEntry.Children.OfType<OpusSpecificBox>().SingleOrDefault();
            }
            else if (config.GetParent() is AudioSampleEntryV1 audioSampleEntryV1)
            {
                Timescale = timescale == 0 ? audioSampleEntryV1.Samplerate >> 16 : timescale;
                ChannelCount = (byte)audioSampleEntryV1.Channelcount;
                SamplingRate = audioSampleEntryV1.Samplerate >> 16;
                SampleSize = audioSampleEntryV1.Samplesize;
                opusSpecificBox = audioSampleEntryV1.Children.OfType<OpusSpecificBox>().SingleOrDefault();
            }
            else
            {
                throw new NotSupportedException();
            }

            PreSkip = opusSpecificBox._PreSkip;
            OutputGain = opusSpecificBox._OutputGain;
            InputSampleRate = opusSpecificBox._InputSampleRate;
            ChannelMappingFamily = opusSpecificBox._ChannelMappingFamily;
            if (opusSpecificBox._ChannelMappingTable != null)
            {
                StreamCount = opusSpecificBox._ChannelMappingTable._StreamCount;
                CoupledCount = opusSpecificBox._ChannelMappingTable._CoupledCount;
                ChannelMapping = opusSpecificBox._ChannelMappingTable._ChannelMapping;
            }
        }

        /// <summary>
        /// The sample goes to the file as it arrives, so this hands back exactly what it was
        /// given, without copying it anywhere.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC("Opus"));
            audioSampleEntry.Children = new List<Box>();
            audioSampleEntry.Channelcount = ChannelCount;
            audioSampleEntry.Samplerate = SamplingRate << 16; // 16.16 fixed point, as a reader takes it
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.Samplesize = SampleSize;
            audioSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            audioSampleEntry.Reserved = new ushort[3]; // TODO simplify API

            // TODO: It should be possible to parse the Opus payload and get these parameters
            OpusSpecificBox opusSpecificBox = new OpusSpecificBox();
            opusSpecificBox.SetParent(audioSampleEntry);
            audioSampleEntry.Children.Add(opusSpecificBox);

            opusSpecificBox._Version = 0; // Opus Specific Box version 0
            opusSpecificBox._OutputChannelCount = ChannelCount;
            opusSpecificBox._PreSkip = PreSkip;
            opusSpecificBox._InputSampleRate = InputSampleRate;
            opusSpecificBox._OutputGain = OutputGain; // 8.8 fixed-point
            opusSpecificBox._ChannelMappingFamily = ChannelMappingFamily;

            // of a family other than 0 only: of family 0, mono or stereo, the box has no table
            if (ChannelMappingFamily != 0)
            {
                opusSpecificBox._ChannelMappingTable = new ChannelMappingTable(ChannelCount)
                {
                    _StreamCount = StreamCount,
                    _CoupledCount = CoupledCount,
                    _ChannelMapping = ChannelMapping
                };
                opusSpecificBox._ChannelMappingTable.SetParent(opusSpecificBox);
            }

            return audioSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 256;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new OpusTrack(ChannelCount, PreSkip, OutputGain, ChannelMappingFamily, StreamCount, CoupledCount, ChannelMapping)
            {
                SamplingRate = SamplingRate,
                SampleSize = SampleSize,
                InputSampleRate = InputSampleRate,
            });
        }
    }
}
