using SharpH264;
using SharpH26X;
using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H264 track.
    /// </summary>
    /// <remarks>
    /// https://www.itu.int/rec/T-REC-H.264/en
    /// https://yumichan.net/video-processing/video-compression/introduction-to-h264-nal-unit/
    /// https://stackoverflow.com/questions/38094302/how-to-understand-header-of-h264
    /// </remarks>
    public class H264Track : TrackBase, IH26XTrack
    {
        public const string BRAND = "avc1";
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";
        public int NalLengthSize { get; set; } = 4;

        private List<byte[]> _nalBuffer = new List<byte[]>();
        private bool _nalBufferContainsVCL = false; 
        private bool _nalBufferContainsIDR = false;

        // first VCL detection
        private ulong last_frame_num;
        private ulong last_pic_parameter_set_id;
        private byte last_field_pic_flag;
        private byte last_bottom_field_flag;
        private uint last_nal_ref_idc;
        private ulong last_pic_order_cnt_type;
        private ulong last_pic_order_cnt_lsb;
        private long last_delta_pic_order_cnt_bottom;
        private long last_delta_pic_order_cnt0;
        private long last_delta_pic_order_cnt1;
        private uint last_IdrPicFlag;
        private ulong last_idr_pic_id;
        private bool last_filled = false;

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
        /// Overrides any auto-detected timescale.
        /// </summary>
        public uint TimescaleOverride { get; set; } = 0;

        /// <summary>
        /// Overrides any auto-detected frame tick.
        /// </summary>
        public uint FrameTickOverride { get; set; } = 0;

        /// <summary>
        /// If it is not possible to retrieve timescale from the SPS, use this value as a fallback.
        /// </summary>
        public uint TimescaleFallback { get; set; } = 600;

        /// <summary>
        /// If it is not possible to retrieve frame tick from the SPS, use this value as a fallback.
        /// </summary>
        public uint FrameTickFallback { get; set; } = 25;

        private H264Context _context = new H264Context();

        /// <summary>
        /// Ctor.
        /// </summary>
        public H264Track()
        {
            CompatibleBrand = BRAND; // avc1
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
        }

        public H264Track(Box sampleEntry, uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;

            VisualSampleEntry visualSampleEntry = (VisualSampleEntry)sampleEntry;
            AVCConfigurationBox avcC = visualSampleEntry.Children.OfType<AVCConfigurationBox>().Single();

            NalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

            foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
            {
                ProcessSample(spsBinary, out _, out _);
            }

            foreach (var ppsBinary in avcC._AVCConfig.PictureParameterSetNALUnit)
            {
                ProcessSample(ppsBinary, out _, out _);
            }

            // TODO SampleDuration
        }

        /// <summary>
        /// Process 1 NAL (Network Abstraction Layer) unit.
        /// </summary>
        /// <param name="sample">NAL bytes.</param>
        /// <param name="isRandomAccessPoint">true when the sample contains a keyframe.</param>
        public override void ProcessSample(byte[] sample, out byte[] output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = _nalBufferContainsIDR;
            output = null;

            if(sample == null)
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

                if(nu.NalUnitType == H264NALTypes.AUD)
                {
                    // access unit delimiter NAL unit(when present)
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else if (nu.NalUnitType == H264NALTypes.SPS)
                {
                    _context.SeqParameterSetRbsp = new SeqParameterSetRbsp();
                    _context.SeqParameterSetRbsp.Read(_context, stream);
                    if (!Sps.ContainsKey(_context.SeqParameterSetRbsp.SeqParameterSetData.SeqParameterSetId))
                    {
                        Sps.Add(_context.SeqParameterSetRbsp.SeqParameterSetData.SeqParameterSetId, _context.SeqParameterSetRbsp);
                        SpsRaw.Add(_context.SeqParameterSetRbsp.SeqParameterSetData.SeqParameterSetId, sample);
                    }

                    // if SPS contains the timescale, set it
                    if (Timescale == 0 || DefaultSampleDuration == 0)
                    {
                        var timescale = CalculateTimescale(_context.SeqParameterSetRbsp);
                        if (timescale.Timescale != 0 && timescale.FrameTick != 0)
                        {
                            Timescale = timescale.Timescale; // MaxFPS = Ceil( time_scale / ( 2 * num_units_in_tick ) )
                            DefaultSampleDuration = (int)timescale.FrameTick * 2;
                        }
                        else
                        {
                            Timescale = TimescaleFallback;
                            DefaultSampleDuration = (int)FrameTickFallback;
                        }
                    }

                    if (TimescaleOverride != 0)
                    {
                        Timescale = TimescaleOverride;
                    }
                    if (FrameTickOverride != 0)
                    {
                        DefaultSampleDuration = (int)FrameTickOverride;
                    }

                    // sequence parameter set NAL unit (when present)
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else if (nu.NalUnitType == H264NALTypes.PPS)
                {
                    _context.PicParameterSetRbsp = new PicParameterSetRbsp();
                    _context.PicParameterSetRbsp.Read(_context, stream);
                    if (!Pps.ContainsKey(_context.PicParameterSetRbsp.PicParameterSetId))
                    {
                        Pps.Add(_context.PicParameterSetRbsp.PicParameterSetId, _context.PicParameterSetRbsp);
                        PpsRaw.Add(_context.PicParameterSetRbsp.PicParameterSetId, sample);
                    }

                    // picture parameter set NAL unit (when present)
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else if (nu.NalUnitType == H264NALTypes.SEI)
                {
                    // SEI NAL unit (when present)
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }

                    _nalBuffer.Add(sample);
                }
                else if(nu.NalUnitType >= H264NALTypes.PREFIX_NAL && nu.NalUnitType <= H264NALTypes.RESERVED1)
                {
                    // NAL units with nal_unit_type in the range of 14 to 18, inclusive (when present),
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else
                {
                    if (nu.NalUnitType >= H264NALTypes.SLICE && nu.NalUnitType <= H264NALTypes.IDR_SLICE)
                    {
                        ulong frame_num;
                        ulong pic_parameter_set_id;
                        byte field_pic_flag;
                        byte bottom_field_flag;
                        uint nal_ref_idc;
                        ulong pic_order_cnt_type;
                        ulong pic_order_cnt_lsb;
                        long delta_pic_order_cnt_bottom;
                        long delta_pic_order_cnt0;
                        long delta_pic_order_cnt1;
                        uint IdrPicFlag;
                        ulong idr_pic_id;

                        SliceHeader sliceHeader;

                        // first VCL NAL unit of a primary coded picture (always present)
                        if (nu.NalUnitType == H264NALTypes.SLICE) // 1
                        {
                            _context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                            _context.SliceLayerWithoutPartitioningRbsp.Read(_context, stream);
                            sliceHeader = _context.SliceLayerWithoutPartitioningRbsp.SliceHeader;
                        }
                        else if (nu.NalUnitType == H264NALTypes.DPA) // 2
                        {
                            _context.SliceDataPartitionaLayerRbsp = new SliceDataPartitionaLayerRbsp();
                            _context.SliceDataPartitionaLayerRbsp.Read(_context, stream);
                            sliceHeader = _context.SliceDataPartitionaLayerRbsp.SliceHeader;
                        }
                        else if (nu.NalUnitType == H264NALTypes.DPB) // 3
                        {
                            _context.SliceDataPartitionbLayerRbsp = new SliceDataPartitionbLayerRbsp();
                            _context.SliceDataPartitionbLayerRbsp.Read(_context, stream);
                            sliceHeader = _context.SliceDataPartitionaLayerRbsp.SliceHeader;
                        }
                        else if (nu.NalUnitType == H264NALTypes.DPC) // 4
                        {
                            _context.SliceDataPartitioncLayerRbsp = new SliceDataPartitioncLayerRbsp();
                            _context.SliceDataPartitioncLayerRbsp.Read(_context, stream);
                            sliceHeader = _context.SliceDataPartitionaLayerRbsp.SliceHeader;
                        }
                        else if (nu.NalUnitType == H264NALTypes.IDR_SLICE) // 5
                        {
                            // keyframe
                            _nalBufferContainsIDR = true;

                            _context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                            _context.SliceLayerWithoutPartitioningRbsp.Read(_context, stream);
                            sliceHeader = _context.SliceLayerWithoutPartitioningRbsp.SliceHeader;
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }

                        frame_num = sliceHeader.FrameNum;
                        pic_parameter_set_id = sliceHeader.PicParameterSetId;
                        ulong seq_parameter_set_id = _context.PicParameterSets[pic_parameter_set_id].SeqParameterSetId;
                        field_pic_flag = sliceHeader.FieldPicFlag;
                        bottom_field_flag = sliceHeader.BottomFieldFlag;
                        nal_ref_idc = nu.NalRefIdc;
                        pic_order_cnt_type = _context.SeqParameterSets[seq_parameter_set_id].SeqParameterSetData.PicOrderCntType;
                        pic_order_cnt_lsb = sliceHeader.PicOrderCntLsb;
                        delta_pic_order_cnt_bottom = sliceHeader.DeltaPicOrderCntBottom;
                        delta_pic_order_cnt0 = sliceHeader.DeltaPicOrderCnt[0];
                        delta_pic_order_cnt1 = sliceHeader.DeltaPicOrderCnt[1];
                        IdrPicFlag = _context.IdrPicFlag;
                        idr_pic_id = sliceHeader.IdrPicId;
                                               
                        if (
                            !last_filled ||
                            (frame_num != last_frame_num) || // frame_num differs in value
                            (pic_parameter_set_id != last_pic_parameter_set_id) || // pic_parameter_set_id differs in value
                            (field_pic_flag != last_field_pic_flag) || // field_pic_flag differs in value
                            (field_pic_flag != 0 && last_field_pic_flag != 0 && bottom_field_flag != last_bottom_field_flag) || // bottom_field_flag is present in both and differs in value
                            ((nal_ref_idc == 0 || last_nal_ref_idc == 0) && nal_ref_idc != last_nal_ref_idc) || // nal_ref_idc differs in value with one of the nal_ref_idc values being equal to 0.
                            (pic_order_cnt_type == 0 && last_pic_order_cnt_type == 0 && (pic_order_cnt_lsb != last_pic_order_cnt_lsb || delta_pic_order_cnt_bottom != last_delta_pic_order_cnt_bottom)) || // pic_order_cnt_type is equal to 0 for both and either pic_order_cnt_lsb differs in value, or delta_pic_order_cnt_bottom differs in value
                            (pic_order_cnt_type == 1 && last_pic_order_cnt_type == 1 && (delta_pic_order_cnt0 != last_delta_pic_order_cnt0 || delta_pic_order_cnt1 != last_delta_pic_order_cnt1)) || // pic_order_cnt_type is equal to 1 for both and either delta_pic_order_cnt[ 0 ] differs in value, or delta_pic_order_cnt[ 1 ] differs in value
                            (IdrPicFlag != last_IdrPicFlag) || // IdrPicFlag differs in value
                            (IdrPicFlag == 1 && last_IdrPicFlag == 1 && idr_pic_id != last_idr_pic_id) // IdrPicFlag is equal to 1 for both and idr_pic_id differs in value
                            )
                        {
                            if (_nalBufferContainsVCL)
                            {
                                output = CreateSample(_nalBuffer);
                                _nalBuffer.Clear();
                                isRandomAccessPoint = _nalBufferContainsIDR;
                                _nalBufferContainsVCL = false;
                                _nalBufferContainsIDR = false;
                            }
                        }

                        last_frame_num = frame_num;
                        last_pic_parameter_set_id = pic_parameter_set_id; 
                        last_field_pic_flag = field_pic_flag;
                        last_bottom_field_flag = bottom_field_flag;
                        last_nal_ref_idc = nal_ref_idc;
                        last_pic_order_cnt_type = pic_order_cnt_type;
                        last_pic_order_cnt_lsb = pic_order_cnt_lsb;
                        last_delta_pic_order_cnt_bottom = delta_pic_order_cnt_bottom;
                        last_delta_pic_order_cnt0 = delta_pic_order_cnt0;
                        last_delta_pic_order_cnt1 = delta_pic_order_cnt1;
                        last_IdrPicFlag = IdrPicFlag;
                        last_idr_pic_id = idr_pic_id;
                        last_filled = true;

                        _nalBufferContainsVCL = true;
                    }

                    _nalBuffer.Add(sample);
                }
            }
        }

        private byte[] CreateSample(List<byte[]> buffer)
        {
            if (buffer.Count == 0)
                return null;

            IEnumerable<byte> result = new byte[0];

            foreach (var nal in _nalBuffer)
            {
                uint nalUnitLength = (uint)nal.Length;

                byte[] size;
                switch (NalLengthSize)
                {
                    case 1:
                        {
                            if (nalUnitLength > byte.MaxValue) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                            size = new byte[]
                            {
                                (byte)(nalUnitLength & 0xff)
                            };
                        }
                        break;

                    case 2:
                        {
                            if (nalUnitLength > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                            size = new byte[]
                            {
                                (byte)((nalUnitLength & 0xff00) >> 8),
                                (byte)(nalUnitLength & 0xff)
                            };
                        }
                        break;

                    case 3:
                        {
                            if (nalUnitLength > 16777215) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                            size = new byte[]
                            {
                                (byte)((nalUnitLength & 0xff0000) >> 16),
                                (byte)((nalUnitLength & 0xff00) >> 8),
                                (byte)(nalUnitLength & 0xff)
                            };
                        }
                        break;

                    case 4:
                        {
                            if (nalUnitLength > uint.MaxValue) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                            size = new byte[]
                            {
                                (byte)((nalUnitLength & 0xff000000) >> 24),
                                (byte)((nalUnitLength & 0xff0000) >> 16),
                                (byte)((nalUnitLength & 0xff00) >> 8),
                                (byte)(nalUnitLength & 0xff)
                            };
                        }
                        break;

                    default:
                        throw new NotSupportedException($"NAL unit length {NalLengthSize} not supported!");
                }
                result = result.Concat(size).Concat(nal);
            }

            if (Log.DebugEnabled) Log.Debug($"{nameof(H264Track)}: AU: {_nalBuffer.Count}");

            return result.ToArray();
        }

        public override Box CreateSampleEntryBox()
        {
            var sps = Sps.First().Value;
            var dim = CalculateDimensions(sps);

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
           
            visualSampleEntry.Width = (ushort)dim.Width;
            visualSampleEntry.Height = (ushort)dim.Height;
            visualSampleEntry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            AVCConfigurationBox avcConfigurationBox = new AVCConfigurationBox();
            avcConfigurationBox.SetParent(visualSampleEntry);

            avcConfigurationBox._AVCConfig = new AVCDecoderConfigurationRecord();
            avcConfigurationBox._AVCConfig.Reserved0 = 0; // TODO: Investigate this value
            avcConfigurationBox._AVCConfig.SequenceParameterSetNALUnit = SpsRaw.Values.ToArray();
            avcConfigurationBox._AVCConfig.SequenceParameterSetLength = SpsRaw.Values.Select(x => (ushort)x.Length).ToArray();
            avcConfigurationBox._AVCConfig.NumOfSequenceParameterSets = (byte)Sps.Count;
            avcConfigurationBox._AVCConfig.PictureParameterSetNALUnit = PpsRaw.Values.ToArray();
            avcConfigurationBox._AVCConfig.PictureParameterSetLength = PpsRaw.Values.Select(x => (ushort)x.Length).ToArray();
            avcConfigurationBox._AVCConfig.NumOfPictureParameterSets = (byte)Pps.Count;
            avcConfigurationBox._AVCConfig._AVCLevelIndication = (byte)sps.SeqParameterSetData.LevelIdc;
            avcConfigurationBox._AVCConfig._AVCProfileIndication = (byte)sps.SeqParameterSetData.ProfileIdc;
            avcConfigurationBox._AVCConfig.BitDepthLumaMinus8 = (byte)sps.SeqParameterSetData.BitDepthLumaMinus8;
            avcConfigurationBox._AVCConfig.BitDepthChromaMinus8 = (byte)sps.SeqParameterSetData.BitDepthChromaMinus8;
            avcConfigurationBox._AVCConfig.ChromaFormat = (byte)sps.SeqParameterSetData.ChromaFormatIdc;
            avcConfigurationBox._AVCConfig.ConfigurationVersion = 1;
            avcConfigurationBox._AVCConfig.LengthSizeMinusOne = 3;
            avcConfigurationBox._AVCConfig.ProfileCompatibility =
                (byte)((sps.SeqParameterSetData.ConstraintSet0Flag != 0 ? 128 : 0) +
                       (sps.SeqParameterSetData.ConstraintSet1Flag != 0 ? 64 : 0) +
                       (sps.SeqParameterSetData.ConstraintSet2Flag != 0 ? 32 : 0) +
                       (sps.SeqParameterSetData.ConstraintSet3Flag != 0 ? 16 : 0) +
                       (sps.SeqParameterSetData.ConstraintSet4Flag != 0 ? 8 : 0) +
                       (sps.SeqParameterSetData.ReservedZero2bits & 0x3));
            visualSampleEntry.Children.Add(avcConfigurationBox);

            return visualSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            var dim = CalculateDimensions(Sps.FirstOrDefault().Value);
            // convert to fixed point 1616
            tkhd.Width = dim.Width << 16; // TODO: simplify API
            tkhd.Height = dim.Height << 16; // TODO: simplify API
        }

        public (uint Width, uint Height) CalculateDimensions(SeqParameterSetRbsp sps)
        {
            var spsData = sps.SeqParameterSetData;
            ulong width = (spsData.PicWidthInMbsMinus1 + 1) * 16;
            ulong mult = 2;
            if (spsData.FrameMbsOnlyFlag != 0)
            {
                mult = 1;
            }
            ulong height = 16 * (spsData.PicHeightInMapUnitsMinus1 + 1) * mult;
            if (spsData.FrameCroppingFlag != 0)
            {
                ulong chromaFormat = spsData.ChromaFormatIdc;
                ulong chromaArrayType = 0;
                if (spsData.SeparateColourPlaneFlag == 0)
                {
                    chromaArrayType = chromaFormat;
                }

                ulong cropUnitX = 1;
                ulong cropUnitY = mult;
                if (chromaArrayType != 0)
                {
                    uint subWidth = 2;
                    uint subHeight = 1;
                    if(chromaFormat == 3) 
                        subWidth = 1;
                    if(chromaFormat == 1) 
                        subHeight = 2;

                    cropUnitX = subWidth;
                    cropUnitY = subHeight * mult;
                }

                width -= cropUnitX * (spsData.FrameCropLeftOffset + spsData.FrameCropRightOffset);
                height -= cropUnitY * (spsData.FrameCropTopOffset + spsData.FrameCropBottomOffset);
            }
            return ((uint)width, (uint)height);
        }

        private (uint Timescale, uint FrameTick) CalculateTimescale(SeqParameterSetRbsp sps)
        {
            uint timescale = 0;
            uint frametick = 0;
            var vui = sps.SeqParameterSetData.VuiParameters;
            if (vui != null && vui.TimingInfoPresentFlag != 0)
            {
                // MaxFPS = Ceil( time_scale / ( 2 * num_units_in_tick ) )
                timescale = vui.TimeScale;
                frametick = vui.NumUnitsInTick;

                if (timescale == 0 || frametick == 0)
                {
                    if (Log.WarnEnabled) Log.Warn($"{nameof(H264Track)}: Invalid values in vui: timescale: {timescale} and frametick: {frametick}.");
                    timescale = 0;
                    frametick = 0;
                }
            }
            else
            {
                if (Log.WarnEnabled) Log.Warn($"{nameof(H264Track)}: Can't determine frame rate because SPS does not contain vuiParams");
            }

            return (timescale, frametick);
        }

        public byte[][] GetVideoNALUs()
        {
            return SpsRaw.Values.ToArray().Concat(PpsRaw.Values.ToArray()).ToArray();
        }
    }
}
