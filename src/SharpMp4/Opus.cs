using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SharpMp4
{
    /// <summary>
    /// Opus Track. 
    /// </summary>
    /// <remarks>https://opus-codec.org/docs/opus_in_isobmff.html</remarks>
    public class OpusTrack : TrackBase
    {
        public byte ChannelCount { get; private set; }
        public int SamplingRate { get; private set; }
        public ushort SampleSize { get; private set; }

        public override string HdlrName => HdlrNames.Sound;
        public override string HdlrType => HdlrTypes.Sound;

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

        public override Mp4Box CreateSampleEntryBox(Mp4Box parent)
        {
            return CreateAudioSampleEntryBox(parent, this);
        }

        // TODO: Not tested
        private static AudioSampleEntryBox CreateAudioSampleEntryBox(Mp4Box parent, OpusTrack opusTrack)
        {
            AudioSampleEntryBox audioSampleEntry = new AudioSampleEntryBox(0, 0, parent, "Opus");

            audioSampleEntry.ChannelCount = opusTrack.ChannelCount;
            audioSampleEntry.SampleRate = opusTrack.SamplingRate;
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.SampleSize = opusTrack.SampleSize;

            // TODO: It should be possible to parse the Opus payload and get these parameters
            OpusSpecificBox opusSpecificBox = new OpusSpecificBox(
                0,
                0,
                audioSampleEntry,
                0, // default version is 0
                opusTrack.ChannelCount,
                opusTrack.PreSkip,
                (uint)opusTrack.SamplingRate,
                opusTrack.OutputGain,
                opusTrack.ChannelMappingFamily,
                opusTrack.StreamCount,
                opusTrack.CoupledCount,
                opusTrack.ChannelMapping);

            audioSampleEntry.Children.Add(opusSpecificBox);
            return audioSampleEntry;
        }

        public override void FillTkhdBox(TkhdBox tkhd)
        {
            tkhd.Volume = 1;
        }
    }

    public class OpusSpecificBox : Mp4Box
    {
        public const string TYPE = "dOps";

        public byte Version { get; set; } = 0;
        public byte OutputChannelCount { get; set; }
        public ushort PreSkip { get; set; } = 3840;
        public uint InputSampleRate { get; set; } 
        public short OutputGain { get; set; }
        public byte ChannelMappingFamily { get; set; }

        // ChannelMappingTable
        public byte StreamCount { get; set; }
        public byte CoupledCount { get; set; }
        public byte[] ChannelMapping { get; set; } = null;

        public OpusSpecificBox(uint size, ulong largeSize, Mp4Box parent) : base(size, largeSize, TYPE, parent)
        { }

        public OpusSpecificBox(
            uint size, 
            ulong largeSize,
            Mp4Box parent,
            byte version, 
            byte outputChannelCount,
            ushort preSkip,
            uint inputSampleRate, 
            short outputGain,
            byte channelMappingFamily,
            byte streamCount, 
            byte coupledCount, 
            byte[] channelMapping) : this(size, largeSize, parent)
        {
            this.Version = version;
            this.OutputChannelCount = outputChannelCount;
            this.PreSkip = preSkip;
            this.InputSampleRate = inputSampleRate;
            this.OutputGain = outputGain;
            this.ChannelMappingFamily = channelMappingFamily;
            this.StreamCount = streamCount;
            this.CoupledCount = coupledCount;
            this.ChannelMapping = channelMapping;
        }

        public static Task<Mp4Box> ParseAsync(uint size, ulong largeSize, string type, Mp4Box parent, Stream stream)
        {
            byte version = IsoReaderWriter.ReadByte(stream);
            byte outputChannelCount = IsoReaderWriter.ReadByte(stream);
            ushort preSkip = IsoReaderWriter.ReadUInt16(stream);
            uint inputSampleRate = IsoReaderWriter.ReadUInt32(stream);
            short outputGain = IsoReaderWriter.ReadInt16(stream);
            byte channelMappingFamily = IsoReaderWriter.ReadByte(stream);
            byte streamCount = 0;
            byte coupledCount = 0;
            List<byte> channelMapping = null;

            if(channelMappingFamily != 0)
            {
                channelMapping = new List<byte>();
                streamCount = IsoReaderWriter.ReadByte(stream);
                coupledCount = IsoReaderWriter.ReadByte(stream);
                
                for(int i = 0; i < 8 * outputChannelCount; i++)
                {
                    channelMapping.Add(IsoReaderWriter.ReadByte(stream));
                }
            }

            return Task.FromResult((Mp4Box)new OpusSpecificBox(
                size,
                largeSize,
                parent,
                version,
                outputChannelCount,
                preSkip,
                inputSampleRate,
                outputGain,
                channelMappingFamily,
                streamCount,
                coupledCount,
                channelMapping?.ToArray()
                ));
        }

        public static Task<ulong> BuildAsync(Mp4Box box, Stream stream)
        {
            OpusSpecificBox b = (OpusSpecificBox)box;
            ulong size = 0;

            size += IsoReaderWriter.WriteByte(stream, b.Version);
            size += IsoReaderWriter.WriteByte(stream, b.OutputChannelCount);
            size += IsoReaderWriter.WriteUInt16(stream, b.PreSkip);
            size += IsoReaderWriter.WriteUInt32(stream, b.InputSampleRate);
            size += IsoReaderWriter.WriteInt16(stream, b.OutputGain);
            size += IsoReaderWriter.WriteByte(stream, b.ChannelMappingFamily);

            if (b.ChannelMappingFamily != 0)
            {
                size += IsoReaderWriter.WriteByte(stream, b.StreamCount);
                size += IsoReaderWriter.WriteByte(stream, b.CoupledCount);

                for (int i = 0; i < 8 * b.OutputChannelCount; i++)
                {
                    size += IsoReaderWriter.WriteByte(stream, b.ChannelMapping[i]);
                }
            }

            return Task.FromResult(size);
        }

        public override ulong CalculateSize()
        {
            return (ulong)((long)base.CalculateSize() + 11 + (ChannelMappingFamily != 0 ? (2 + 8 * OutputChannelCount) : 0));
        }
    }
}
