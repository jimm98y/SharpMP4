using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// ALAC Track: an 'alac' sample entry and its 'alac' box, of the ALACSpecificConfig - the magic cookie - after a
    /// version and flags. QuickTime's entry has the box in a 'wave'; it is read from either, and written as MP4 has it.
    /// </summary>
    /// <remarks>https://github.com/macosforge/alac/blob/master/ALACMagicCookieDescription.txt</remarks>
    public class AlacTrack : TrackBase
    {
        /// <summary>The size of an ALACSpecificConfig.</summary>
        public const int CONFIG_SIZE = 24;

        /// <summary>The samples of each channel a frame holds, where the config does not say: what Apple's encoder writes.</summary>
        public const uint DEFAULT_FRAME_LENGTH = 4096;

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        /// <summary>
        /// The ALACSpecificConfig - the decoder's magic cookie - and whatever follows it in the box: the channel layout,
        /// of a stream of more than two channels.
        /// </summary>
        public byte[] Config { get; }

        /// <summary>The samples of each channel a frame holds: of every frame but the last.</summary>
        public uint FrameLength { get; private set; }

        public byte BitDepth { get; private set; }
        public byte ChannelCount { get; private set; }
        public uint SamplingRate { get; private set; }
        public uint MaxFrameBytes { get; private set; }
        public uint AvgBitRate { get; private set; }

        /// <summary>
        /// Ctor, of an ALACSpecificConfig: what an encoder hands out as its magic cookie.
        /// </summary>
        /// <param name="config">The ALACSpecificConfig, 24 bytes, and the channel layout after it, if any.</param>
        public AlacTrack(byte[] config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            ReadConfig();
        }

        /// <summary>
        /// Ctor with initialization from the <see cref="SampleEntry"/>.
        /// </summary>
        /// <param name="config">The entry's 'alac' box, or QuickTime's 'wave' it is in.</param>
        public AlacTrack(Box config, uint timescale = 0, int sampleDuration = -1)
        {
            Config = ConfigOf(config) ?? throw new ArgumentException($"Invalid ALAC configuration: {IsoStream.ToFourCC(config.FourCC)}");
            ReadConfig();

            Timescale = timescale == 0 ? SamplingRate : timescale;
            if (sampleDuration > 0)
                DefaultSampleDuration = sampleDuration;
        }

        /// <summary>Whether a box is ALAC's 'alac' box, or a 'wave' with one in it.</summary>
        public static bool IsAlac(Box config) => ConfigOf(config) != null;

        /// <summary>
        /// The ALACSpecificConfig of an 'alac' box - past its version and flags - or of the one in a 'wave'; null of any
        /// other box.
        /// </summary>
        private static byte[] ConfigOf(Box config)
        {
            Box alac = config?.FourCC == IsoStream.FromFourCC("alac") && config is not SampleEntry
                ? config
                : config?.FourCC == IsoStream.FromFourCC("wave") ? config.Children?.FirstOrDefault(x => x.FourCC == IsoStream.FromFourCC("alac")) : null;
            if (alac is not CodecConfigurationBox box || box.Data == null || box.Data.Length < 4 + CONFIG_SIZE)
                return null;
            var data = new byte[box.Data.Length - 4];
            Buffer.BlockCopy(box.Data, 4, data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// The stream's parameters, of its ALACSpecificConfig, big-endian: frameLength (32), compatibleVersion, bitDepth, pb,
        /// mb, kb, numChannels (8 each), maxRun (16), maxFrameBytes, avgBitRate and sampleRate (32 each).
        /// </summary>
        private void ReadConfig()
        {
            if (Config.Length < CONFIG_SIZE)
                throw new ArgumentException($"An ALACSpecificConfig is of {CONFIG_SIZE} bytes.");

            FrameLength = ReadUInt32(0);
            BitDepth = Config[5];
            ChannelCount = Config[9];
            MaxFrameBytes = ReadUInt32(12);
            AvgBitRate = ReadUInt32(16);
            SamplingRate = ReadUInt32(20);

            Timescale = SamplingRate;
            DefaultSampleDuration = (int)(FrameLength == 0 ? DEFAULT_FRAME_LENGTH : FrameLength);
        }

        private uint ReadUInt32(int offset) =>
            (uint)((Config[offset] << 24) | (Config[offset + 1] << 16) | (Config[offset + 2] << 8) | Config[offset + 3]);

        /// <summary>
        /// The sample goes to the file as it arrives - a frame, each of which a decoder can start at - so this hands back
        /// exactly what it was given, without copying it anywhere.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC("alac"));
            audioSampleEntry.Children = new List<Box>();
            audioSampleEntry.Channelcount = ChannelCount;
            audioSampleEntry.Samplerate = SamplingRate <= 0xFFFF ? SamplingRate << 16 : 0; // 16.16 fixed point
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.Samplesize = BitDepth;
            audioSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            audioSampleEntry.Reserved = new ushort[3]; // TODO simplify API

            // a FullBox of version 0, its flags 0, then the config
            var data = new byte[4 + Config.Length];
            Config.CopyTo(data, 4);
            var alac = new CodecConfigurationBox(IsoStream.FromFourCC("alac")) { Data = data };
            alac.SetParent(audioSampleEntry);
            audioSampleEntry.Children.Add(alac);

            return audioSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 256;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new AlacTrack((byte[])Config.Clone()));
        }
    }
}
