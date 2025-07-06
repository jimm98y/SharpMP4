using SharpISOBMFF;
using SharpH26X;
using SharpH266;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H266 track.
    /// </summary>
    /// <remarks>https://www.itu.int/rec/T-REC-H.266/en</remarks>
    public class H266Track : TrackBase, IH26XTrack
    {
        public const string BRAND = "vvc1";

        private List<byte[]> _nalBuffer = new List<byte[]>();
        private bool _nalBufferContainsVCL = false;
        private bool _nalBufferContainsIDR = false;

        /// <summary>
        /// VPS (Video Parameter Set) NAL units.
        /// </summary>
        public Dictionary<ulong, VideoParameterSetRbsp> Vps { get; set; } = new Dictionary<ulong, VideoParameterSetRbsp>();
        public Dictionary<ulong, byte[]> VpsRaw { get; set; } = new Dictionary<ulong, byte[]>();

        /// <summary>
        /// SPS (Sequence Parameter Set) NAL units.
        /// </summary>
        public Dictionary<ulong, SeqParameterSetRbsp> Sps { get; set; } = new Dictionary<ulong, SeqParameterSetRbsp>();
        public Dictionary<ulong, byte[]> SpsRaw { get; set; } = new Dictionary<ulong, byte[]>();

        /// <summary>
        /// PPS (Picture Parameter Set) NAL units.
        /// </summary>
        public Dictionary<ulong, PicParameterSetRbsp> Pps { get; set; } = new Dictionary<ulong, PicParameterSetRbsp>();
        public Dictionary<ulong, byte[]> PpsRaw { get; set; } = new Dictionary<ulong, byte[]>();

        /// <summary>
        /// Prefix SEI (Supplementary Enhancement Information) NAL units.
        /// </summary>
        public List<SeiRbsp> PrefixSei { get; set; } = new List<SeiRbsp>();
        public List<byte[]> PrefixSeiRaw { get; set; } = new List<byte[]>();

        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";
        public int NalLengthSize { get; set; } = 4;

        /// <summary>
        /// Overrides any auto-detected timescale.
        /// </summary>
        public uint TimescaleOverride { get; set; } = 0;

        /// <summary>
        /// Overrides any auto-detected frame tick.
        /// </summary>
        public int FrameTickOverride { get; set; } = 0;

        /// <summary>
        /// If it is not possible to retrieve timescale from the SPS, use this value as a fallback.
        /// </summary>
        public uint TimescaleFallback { get; set; } = 24000;

        /// <summary>
        /// If it is not possible to retrieve frame tick from the SPS, use this value as a fallback.
        /// </summary>
        public int FrameTickFallback { get; set; } = 1001;

        private H266Context _context = new H266Context();

        /// <summary>
        /// Ctor.
        /// </summary>
        public H266Track()
        {
            CompatibleBrand = BRAND; // vvc1
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
        }

        public H266Track(Box sampleEntry, uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;

            VisualSampleEntry visualSampleEntry = (VisualSampleEntry)sampleEntry;
            VvcConfigurationBox vvcC = visualSampleEntry.Children.OfType<VvcConfigurationBox>().Single();

            NalLengthSize = vvcC._VvcConfig._LengthSizeMinusOne + 1; // usually 4 bytes

            foreach (var nalus in vvcC._VvcConfig.NalUnit)
            {
                foreach (var nalu in nalus)
                {
                    ProcessSample(nalu, out _, out _);
                }
            }
        }

        /// <summary>
        /// Process 1 NAL (Network Abstraction Layer) unit.
        /// </summary>
        /// <param name="sample">NAL bytes.</param>
        /// <param name="output">Returns a completed sample (1 AU) or null when no sample is currently available.</returns>
        /// <param name="isRandomAccessPoint">true when the sample contains a keyframe.</param>
        public override void ProcessSample(byte[] sample, out byte[] output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = _nalBufferContainsIDR;
            output = null;

            if (sample == null)
            {
                // flush the last AU
                if (_nalBuffer.Count > 0 && _nalBufferContainsVCL)
                {
                    output = CreateSample(_nalBuffer);
                    _nalBuffer.Clear();
                    _nalBufferContainsVCL = false;
                    _nalBufferContainsIDR = false;
                }
                return;
            }

            using (ItuStream stream = new ItuStream(new MemoryStream(sample)))
            {
                ulong ituSize = 0;
                var nu = new NalUnit((uint)sample.Length);
                _context.NalHeader = nu;
                ituSize += nu.Read(_context, stream);

                if (nu.NalUnitHeader.NalUnitType == H266NALTypes.AUD_NUT)
                {
                    // TODO
                }
            }
        }

        private (uint Timescale, uint FrameTick) CalculateTimescale(SeqParameterSetRbsp sps)
        {
            throw new NotImplementedException();
        }

        private byte[] CreateSample(List<byte[]> buffer)
        {
            throw new NotImplementedException();
        }

        public override Box CreateSampleEntryBox()
        {
            throw new NotImplementedException();
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            throw new NotImplementedException();
        }

        public (uint Width, uint Height) CalculateDimensions(SeqParameterSetRbsp sps)
        {
            ulong width = sps.SpsPicWidthMaxInLumaSamples;
            ulong height = sps.SpsPicHeightMaxInLumaSamples;
            return ((uint)width, (uint)height);
        }

        public uint BuildGeneralProfileCompatibilityFlags(SeqParameterSetRbsp sps)
        {
            throw new NotImplementedException();
        }

        public ulong BuildGeneralProfileConstraintIndicatorFlags(VideoParameterSetRbsp vps)
        {
            throw new NotImplementedException();
        }

        public byte[][] GetVideoNALUs()
        {
            throw new NotImplementedException();
        }
    }
}
