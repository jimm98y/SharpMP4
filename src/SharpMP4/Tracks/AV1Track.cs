using SharpISOBMFF;
using SharpAV1;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AV1 Track.
    /// </summary>
    /// <remarks>
    /// https://aomedia.org/specifications/av1/
    /// </remarks>
    public class AV1Track : TrackBase, IVideoTrack
    {
        public const string BRAND = "av01";

        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";

        /// <summary>
        /// Overrides any auto-detected timescale.
        /// </summary>
        public uint TimescaleOverride { get; set; } = 0;

        /// <summary>
        /// Overrides any auto-detected frame tick.
        /// </summary>
        public int FrameTickOverride { get; set; } = 0;

        /// <summary>
        /// If it is not possible to retrieve timescale from the video, use this value as a fallback.
        /// </summary>
        public uint TimescaleFallback { get; set; } = 600;

        /// <summary>
        /// If it is not possible to retrieve frame tick from the video, use this value as a fallback.
        /// </summary>
        public int FrameTickFallback { get; set; } = 25;

        /// <summary>
        /// Sequence Header Open Bitstream Unit.
        /// </summary>
        public byte[] SequenceHeaderOBU { get; set; } = null;

        private AV1Context _context = new AV1Context();

        public AV1Track()
        {
            CompatibleBrand = BRAND; // av01
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
        }

        public AV1Track(Box sampleEntry, uint timescale, int sampleDuration)
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;

            VisualSampleEntry visualSampleEntry = (VisualSampleEntry)sampleEntry;
            AV1CodecConfigurationBox av01 = visualSampleEntry.Children.OfType<AV1CodecConfigurationBox>().Single();

            ProcessSample(av01.Av1Config.ConfigOBUs, out _, out _);
        }

        /// <summary>
        /// Process 1 OBU.
        /// </summary>
        /// <param name="sample">OBU bytes.</param>
        /// <param name="isRandomAccessPoint">true when the sample contains a keyframe.</param>
        public override void ProcessSample(byte[] sample, out byte[] output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = false; // TODO
            output = null;

            if (sample == null)
            {
                // flush the last OBU
                // TODO
                return;
            }

            // TODO
            base.ProcessSample(sample, out output, out isRandomAccessPoint);
        }

        public override Box CreateSampleEntryBox()
        {
            VisualSampleEntry visualSampleEntry = new VisualSampleEntry(IsoStream.FromFourCC(BRAND));
            visualSampleEntry.Children = new List<Box>();
            visualSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            visualSampleEntry.PreDefined0 = new uint[3]; // TODO simplify API

            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            // convert to fixed point 1616
            visualSampleEntry.Horizresolution = 72 << 16; // TODO simplify API
            visualSampleEntry.Vertresolution = 72 << 16; // TODO simplify API

            visualSampleEntry.Width = (ushort)_context._RenderWidth;
            visualSampleEntry.Height = (ushort)_context._RenderHeight;
            visualSampleEntry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            AV1CodecConfigurationBox av01ConfigurationBox = new AV1CodecConfigurationBox();
            av01ConfigurationBox.SetParent(visualSampleEntry);

            av01ConfigurationBox.Av1Config = new AV1CodecConfigurationRecord();
            av01ConfigurationBox.Av1Config.Reserved = 0;
            av01ConfigurationBox.Av1Config.Reserved0 = 0;
            av01ConfigurationBox.Av1Config.Version = 1;
            av01ConfigurationBox.Av1Config.InitialPresentationDelayMinusOne = 0;
            av01ConfigurationBox.Av1Config.InitialPresentationDelayPresent = false;
            av01ConfigurationBox.Av1Config.ChromaSamplePosition = (byte)_context._ChromaSamplePosition;
            av01ConfigurationBox.Av1Config.ChromaSubsamplingx = _context._Subsamplingx != 0;
            av01ConfigurationBox.Av1Config.ChromaSubsamplingy = _context._Subsamplingy != 0;
            av01ConfigurationBox.Av1Config.ConfigOBUs = SequenceHeaderOBU; 
            av01ConfigurationBox.Av1Config.HighBitdepth = _context._HighBitdepth != 0;
            av01ConfigurationBox.Av1Config.Marker = true; // shall be set to true
            av01ConfigurationBox.Av1Config.Monochrome = _context._MonoChrome != 0;
            av01ConfigurationBox.Av1Config.SeqLevelIdx0 = (byte)_context._SeqLevelIdx[0];
            av01ConfigurationBox.Av1Config.SeqProfile = (byte)_context._SeqProfile;
            av01ConfigurationBox.Av1Config.SeqTier0 = _context._SeqTier[0] != 0;
            av01ConfigurationBox.Av1Config.TwelveBit = _context._TwelveBit != 0;

            visualSampleEntry.Children.Add(av01ConfigurationBox);

            return visualSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Width = (uint)_context._RenderWidth;
            tkhd.Height = (uint)_context._RenderHeight;
        }

        public byte[][] GetVideoUnits()
        {
            if(SequenceHeaderOBU == null)
                return null;

            return new byte[][] { SequenceHeaderOBU };
        }
    }
}
