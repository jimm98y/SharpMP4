using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMp4
{
    /// <summary>
    /// H264 track.
    /// </summary>
    /// <remarks>
    /// https://www.itu.int/rec/T-REC-H.264/en
    /// https://yumichan.net/video-processing/video-compression/introduction-to-h264-nal-unit/
    /// https://stackoverflow.com/questions/38094302/how-to-understand-header-of-h264
    /// </remarks>
    public class H264Track : TrackBase
    {
        private List<byte[]> _nalBuffer = new List<byte[]>();
        private bool _nalBufferContainsVCL = false;

        /// <summary>
        /// SPS (Sequence Parameter Set) NAL units.
        /// </summary>
        public Dictionary<int, H264SpsNalUnit> Sps { get; set; } = new Dictionary<int, H264SpsNalUnit>();

        /// <summary>
        /// PPS (Picture Parameter Set) NAL units.
        /// </summary>
        public Dictionary<int, H264PpsNalUnit> Pps { get; set; } = new Dictionary<int, H264PpsNalUnit>();

        public override string HdlrName => HdlrNames.Video;
        public override string HdlrType => HdlrTypes.Video;

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
        public uint TimescaleFallback { get; set; } = 24000;

        /// <summary>
        /// If it is not possible to retrieve frame tick from the SPS, use this value as a fallback.
        /// </summary>
        public uint FrameTickFallback { get; set; } = 1001;

        /// <summary>
        /// Ctor.
        /// </summary>
        public H264Track()
        {
            this.CompatibleBrand = VisualSampleEntryBox.TYPE3; // avc1
            this.DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
        }

        /// <summary>
        /// Process 1 NAL (Network Abstraction Layer) unit.
        /// </summary>
        /// <param name="sample">NAL bytes.</param>
        /// <returns><see cref="Task"/></returns>
        public override async Task ProcessSampleAsync(byte[] sample)
        {
            using (NalBitStreamReader bitstream = new NalBitStreamReader(sample))
            {
                var header = H264NalUnitHeader.ParseNALHeader(bitstream);
                if (header.NalUnitType == H264NalUnitTypes.SPS)
                {
                    if (Log.DebugEnabled) Log.Debug($"-Parsed SPS: {Utilities.ToHexString(sample)}");
                    var sps = H264SpsNalUnit.Parse(sample);
                    if (!Sps.ContainsKey(sps.SeqParameterSetId))
                    {
                        Sps.Add(sps.SeqParameterSetId, sps);
                    }
                    if (Log.DebugEnabled) Log.Debug($"Rebuilt SPS: {Utilities.ToHexString(H264SpsNalUnit.Build(sps))}");

                    // if SPS contains the timescale, set it
                    if (Timescale == 0 || SampleDuration == 0)
                    {
                        var timescale = sps.CalculateTimescale();
                        if (timescale.Timescale != 0 && timescale.FrameTick != 0)
                        {
                            Timescale = (uint)timescale.Timescale; // MaxFPS = Ceil( time_scale / ( 2 * num_units_in_tick ) )
                            SampleDuration = (uint)timescale.FrameTick * 2;
                        }
                        else
                        {
                            Timescale = TimescaleFallback;
                            SampleDuration = FrameTickFallback;
                        }
                    }

                    if (TimescaleOverride != 0)
                    {
                        Timescale = TimescaleOverride;
                    }
                    if (FrameTickOverride != 0)
                    {
                        SampleDuration = FrameTickOverride;
                    }
                }
                else if (header.NalUnitType == H264NalUnitTypes.PPS)
                {
                    if (Log.DebugEnabled) Log.Debug($"-Parsed PPS: {Utilities.ToHexString(sample)}");
                    var pps = H264PpsNalUnit.Parse(sample);
                    if (!Pps.ContainsKey(pps.PicParameterSetId))
                    {
                        Pps.Add(pps.PicParameterSetId, pps);
                    }
                    if (Log.DebugEnabled) Log.Debug($"Rebuilt PPS: {Utilities.ToHexString(H264PpsNalUnit.Build(pps))}");
                }
                else if (header.NalUnitType == H264NalUnitTypes.SEI)
                {
                    if (_nalBufferContainsVCL)
                    {
                        await CreateSample();
                    }

                    _nalBuffer.Add(sample);
                }
                else
                {
                    if (header.IsVCL())
                    {
                        if ((sample[1] & 0x80) != 0) // https://stackoverflow.com/questions/69373668/ffmpeg-error-first-slice-in-a-frame-missing-when-decoding-h-265-stream
                        {
                            if (_nalBufferContainsVCL)
                            {
                                await CreateSample();
                            }
                        }

                        _nalBufferContainsVCL = true;
                    }

                    _nalBuffer.Add(sample);
                }
            }
        }

        /// <summary>
        /// Flush all remaining NAL units from the buffer.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public override async Task FlushAsync()
        {
            if (_nalBuffer.Count == 0 || !_nalBufferContainsVCL)
                return;

            if ((_nalBuffer[0][1] & 0x80) != 0)
            {
                await CreateSample();
            }
            else
            {
                if (Log.WarnEnabled) Log.Warn($"Invalid NAL in the buffer");
            }
        }

        private async Task CreateSample()
        {
            if (_nalBuffer.Count == 0)
                return;

            IEnumerable<byte> result = new byte[0];

            foreach (var nal in _nalBuffer)
            {
                int len = nal.Length;

                // for each NAL, add 4 byte NAL size
                byte[] size = new byte[]
                {
                    (byte)((len & 0xff000000) >> 24),
                    (byte)((len & 0xff0000) >> 16),
                    (byte)((len & 0xff00) >> 8),
                    (byte)(len & 0xff)
                };
                result = result.Concat(size).Concat(nal);
            }

            if (Log.DebugEnabled) Log.Debug($"AU: {_nalBuffer.Count}");

            await base.ProcessSampleAsync(result.ToArray());
            _nalBuffer.Clear();
            _nalBufferContainsVCL = false;
        }

        public override Mp4Box CreateSampleEntryBox(Mp4Box parent)
        {
            return CreateVisualSampleEntryBox(parent, this);
        }

        private static VisualSampleEntryBox CreateVisualSampleEntryBox(Mp4Box parent, H264Track track)
        {
            var sps = track.Sps.First().Value; 
            var dim = sps.CalculateDimensions();

            VisualSampleEntryBox visualSampleEntry = new VisualSampleEntryBox(0, 0, parent, VisualSampleEntryBox.TYPE3);
            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            visualSampleEntry.HorizontalResolution = 72;
            visualSampleEntry.VerticalResolution = 72;
            visualSampleEntry.Width = dim.Width;
            visualSampleEntry.Height = dim.Height;
            visualSampleEntry.CompressorName = "h264\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
            visualSampleEntry.CompressorNameDisplayableData = 4;

            AvcConfigurationBox avcConfigurationBox = new AvcConfigurationBox(0, 0, visualSampleEntry, new AvcDecoderConfigurationRecord());
            avcConfigurationBox.AvcDecoderConfigurationRecord.SequenceParameterSets = track.Sps.Values.ToList();
            avcConfigurationBox.AvcDecoderConfigurationRecord.NumberOfSeuqenceParameterSets = track.Sps.Count;
            avcConfigurationBox.AvcDecoderConfigurationRecord.PictureParameterSets = track.Pps.Values.ToList();
            avcConfigurationBox.AvcDecoderConfigurationRecord.NumberOfPictureParameterSets = (byte)track.Pps.Count;
            avcConfigurationBox.AvcDecoderConfigurationRecord.AvcLevelIndication = (byte)sps.LevelIdc;
            avcConfigurationBox.AvcDecoderConfigurationRecord.AvcProfileIndication = (byte)sps.ProfileIdc;
            avcConfigurationBox.AvcDecoderConfigurationRecord.BitDepthLumaMinus8 = sps.BitDepthLumaMinus8;
            avcConfigurationBox.AvcDecoderConfigurationRecord.BitDepthChromaMinus8 = sps.BitDepthChromaMinus8;
            avcConfigurationBox.AvcDecoderConfigurationRecord.ChromaFormat = sps.ChromaFormat == null ? -1 : sps.ChromaFormat.Id;
            avcConfigurationBox.AvcDecoderConfigurationRecord.ConfigurationVersion = 1;
            avcConfigurationBox.AvcDecoderConfigurationRecord.LengthSizeMinusOne = 3;
            avcConfigurationBox.AvcDecoderConfigurationRecord.ProfileCompatibility =
                (byte)((sps.ConstraintSet0Flag ? 128 : 0) +
                       (sps.ConstraintSet1Flag ? 64 : 0) +
                       (sps.ConstraintSet2Flag ? 32 : 0) +
                       (sps.ConstraintSet3Flag ? 16 : 0) +
                       (sps.ConstraintSet4Flag ? 8 : 0) +
                       (sps.ReservedZero2Bits & 0x3));
            visualSampleEntry.Children.Add(avcConfigurationBox);

            return visualSampleEntry;
        }

        public override void FillTkhdBox(TkhdBox tkhd)
        {
            var dim = Sps.FirstOrDefault().Value.CalculateDimensions();
            tkhd.Width = dim.Width;
            tkhd.Height = dim.Height;
        }
    }

    public static class H264NalUnitTypes
    {
        public const int SEI = 6;
        public const int SPS = 7;
        public const int PPS = 8;
        public const int AUD = 9; // Access Unit Delimiter
    }

    public class AvcConfigurationBox : Mp4Box
    {
        public const string TYPE = "avcC";

        public AvcDecoderConfigurationRecord AvcDecoderConfigurationRecord { get; }

        public AvcConfigurationBox(uint size, ulong largeSize, Mp4Box parent, AvcDecoderConfigurationRecord record) : base(size, largeSize, TYPE, parent)
        {
            AvcDecoderConfigurationRecord = record;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, ulong largeSize, string type, Mp4Box parent, Stream stream)
        {
            ulong recordSize = (size == 1 ? largeSize : size) - (ulong)GetParsedSize(size);
            AvcConfigurationBox avcc = new AvcConfigurationBox(
                size,
                largeSize,
                parent,
                await AvcDecoderConfigurationRecord.Parse(recordSize, stream));

            return avcc;
        }

        public static Task<ulong> BuildAsync(Mp4Box box, Stream stream)
        {
            AvcConfigurationBox b = (AvcConfigurationBox)box;
            return AvcDecoderConfigurationRecord.Build(b.AvcDecoderConfigurationRecord, stream);
        }

        public override ulong CalculateSize()
        {
            return base.CalculateSize() + AvcDecoderConfigurationRecord.CalculateSize();
        }
    }

    public class AvcDecoderConfigurationRecord
    {
        public AvcDecoderConfigurationRecord()
        { }

        public AvcDecoderConfigurationRecord(
            byte configurationVersion,
            byte avcProfileIndication,
            byte profileCompatibility,
            byte avcLevelIndication,
            int lengthSizeMinusOnePaddingBits,
            int lengthSizeMinusOne,
            int numberOfSequenceParameterSetsPaddingBits,
            int numberOfSeuqenceParameterSets,
            List<H264SpsNalUnit> sequenceParameterSets,
            byte numberOfPictureParameterSets,
            List<H264PpsNalUnit> pictureParameterSets,
            int chromaFormat,
            int bitDepthLumaMinus8,
            int bitDepthChromaMinus8,
            int chromaFormatPaddingBits,
            int bitDepthLumaMinus8PaddingBits,
            int bitDepthChromaMinus8PaddingBits,
            List<byte[]> sequenceParameterSetExts)
        {
            ConfigurationVersion = configurationVersion;
            AvcProfileIndication = avcProfileIndication;
            ProfileCompatibility = profileCompatibility;
            AvcLevelIndication = avcLevelIndication;
            LengthSizeMinusOnePaddingBits = lengthSizeMinusOnePaddingBits;
            LengthSizeMinusOne = lengthSizeMinusOne;
            NumberOfSequenceParameterSetsPaddingBits = numberOfSequenceParameterSetsPaddingBits;
            NumberOfSeuqenceParameterSets = numberOfSeuqenceParameterSets;
            SequenceParameterSets = sequenceParameterSets;
            NumberOfPictureParameterSets = numberOfPictureParameterSets;
            PictureParameterSets = pictureParameterSets;
            ChromaFormat = chromaFormat;
            BitDepthLumaMinus8 = bitDepthLumaMinus8;
            BitDepthChromaMinus8 = bitDepthChromaMinus8;
            ChromaFormatPaddingBits = chromaFormatPaddingBits;
            BitDepthLumaMinus8PaddingBits = bitDepthLumaMinus8PaddingBits;
            BitDepthChromaMinus8PaddingBits = bitDepthChromaMinus8PaddingBits;
            SequenceParameterSetExts = sequenceParameterSetExts;
        }

        public byte ConfigurationVersion { get; set; }
        public byte AvcProfileIndication { get; set; }
        public byte ProfileCompatibility { get; set; }
        public byte AvcLevelIndication { get; set; }
        public int LengthSizeMinusOnePaddingBits { get; set; } = 63;
        public int LengthSizeMinusOne { get; set; } = 3;
        public int NumberOfSequenceParameterSetsPaddingBits { get; set; } = 0;
        public int NumberOfSeuqenceParameterSets { get; set; }
        public List<H264SpsNalUnit> SequenceParameterSets { get; set; }
        public byte NumberOfPictureParameterSets { get; set; }
        public List<H264PpsNalUnit> PictureParameterSets { get; set; }
        public int ChromaFormat { get; set; } = -1;
        public int BitDepthLumaMinus8 { get; set; } = -1;
        public int BitDepthChromaMinus8 { get; set; } = -1;
        public int ChromaFormatPaddingBits { get; set; } = 31;
        public int BitDepthLumaMinus8PaddingBits { get; set; } = 31;
        public int BitDepthChromaMinus8PaddingBits { get; set; } = 31;
        public List<byte[]> SequenceParameterSetExts { get; set; } = new List<byte[]>();

        public static async Task<AvcDecoderConfigurationRecord> Parse(ulong size, Stream stream)
        {
            RawBitStreamReader bitstream = new RawBitStreamReader(stream);

            byte configurationVersion = (byte)bitstream.ReadBits(8);
            byte avcProfileIndication = (byte)bitstream.ReadBits(8);
            byte profileCompatibility = (byte)bitstream.ReadBits(8);
            byte avcLevelIndication = (byte)bitstream.ReadBits(8);

            int lengthSizeMinusOnePaddingBits = bitstream.ReadBits(6);
            int lengthSizeMinusOne = bitstream.ReadBits(2);

            List<H264SpsNalUnit> sequenceParameterSets = new List<H264SpsNalUnit>();
            List<H264PpsNalUnit> pictureParameterSets = new List<H264PpsNalUnit>();

            int numberOfSequenceParameterSetsPaddingBits = bitstream.ReadBits(3);
            int numberOfSeuqenceParameterSets = bitstream.ReadBits(5);
            uint consumedLength = 6; // so far we've read 6 bytes
            for (int i = 0; i < numberOfSeuqenceParameterSets; i++)
            {
                ushort sequenceParameterSetLength = IsoReaderWriter.ReadUInt16(stream);
                byte[] sequenceParameterSetNALUnitBytes = new byte[sequenceParameterSetLength];
                await IsoReaderWriter.ReadBytesAsync(stream, sequenceParameterSetNALUnitBytes, 0, sequenceParameterSetLength);
                if (Log.DebugEnabled) Log.Debug($"Read SPS: {Utilities.ToHexString(sequenceParameterSetNALUnitBytes)}");
                H264SpsNalUnit sequenceParameterSetNALUnit = H264SpsNalUnit.Parse(sequenceParameterSetNALUnitBytes);
                sequenceParameterSets.Add(sequenceParameterSetNALUnit);
                consumedLength = consumedLength + 2 + sequenceParameterSetLength;
            }
            byte numberOfPictureParameterSets = IsoReaderWriter.ReadByte(stream);
            for (int i = 0; i < numberOfPictureParameterSets; i++)
            {
                ushort pictureParameterSetLength = IsoReaderWriter.ReadUInt16(stream);
                byte[] pictureParameterSetNALUnitBytes = new byte[pictureParameterSetLength];
                await IsoReaderWriter.ReadBytesAsync(stream, pictureParameterSetNALUnitBytes, 0, pictureParameterSetLength);
                if (Log.DebugEnabled) Log.Debug($"Read PPS: {Utilities.ToHexString(pictureParameterSetNALUnitBytes)}");
                H264PpsNalUnit pictureParameterSetNALUnit = H264PpsNalUnit.Parse(pictureParameterSetNALUnitBytes);
                pictureParameterSets.Add(pictureParameterSetNALUnit);
                consumedLength = consumedLength + 2 + pictureParameterSetLength;
            }

            bool hasExts = true;
            if (size - consumedLength < 4)
            {
                hasExts = false;
            }

            int chromaFormat = -1;
            int bitDepthLumaMinus8 = -1;
            int bitDepthChromaMinus8 = -1;
            int chromaFormatPaddingBits = 31;
            int bitDepthLumaMinus8PaddingBits = 31;
            int bitDepthChromaMinus8PaddingBits = 31;
            List<byte[]> sequenceParameterSetExts = new List<byte[]>();

            if (hasExts && (avcProfileIndication == 100 || avcProfileIndication == 110 || avcProfileIndication == 122 || avcProfileIndication == 144))
            {
                chromaFormatPaddingBits = bitstream.ReadBits(6);
                chromaFormat = bitstream.ReadBits(2);

                bitDepthLumaMinus8PaddingBits = bitstream.ReadBits(5);
                bitDepthLumaMinus8 = bitstream.ReadBits(3);

                bitDepthChromaMinus8PaddingBits = bitstream.ReadBits(5);
                bitDepthChromaMinus8 = bitstream.ReadBits(3);

                byte numOfSequenceParameterSetExt = IsoReaderWriter.ReadByte(stream);
                for (int i = 0; i < numOfSequenceParameterSetExt; i++)
                {
                    ushort sequenceParameterSetExtLength = IsoReaderWriter.ReadUInt16(stream);
                    byte[] sequenceParameterSetExtNALUnit = new byte[sequenceParameterSetExtLength];
                    await IsoReaderWriter.ReadBytesAsync(stream, sequenceParameterSetExtNALUnit, 0, sequenceParameterSetExtNALUnit.Length);
                    sequenceParameterSetExts.Add(sequenceParameterSetExtNALUnit);
                }
            }

            AvcDecoderConfigurationRecord record = new AvcDecoderConfigurationRecord(
                configurationVersion,
                avcProfileIndication,
                profileCompatibility,
                avcLevelIndication,
                lengthSizeMinusOnePaddingBits,
                lengthSizeMinusOne,
                numberOfSequenceParameterSetsPaddingBits,
                numberOfSeuqenceParameterSets,
                sequenceParameterSets,
                numberOfPictureParameterSets,
                pictureParameterSets,
                chromaFormat,
                bitDepthLumaMinus8,
                bitDepthChromaMinus8,
                chromaFormatPaddingBits,
                bitDepthLumaMinus8PaddingBits,
                bitDepthChromaMinus8PaddingBits,
                sequenceParameterSetExts
                );

            return record;
        }

        public static async Task<ulong> Build(AvcDecoderConfigurationRecord b, Stream stream)
        {
            // TODO fix mixed readers/writers
            ulong size = 0;
            RawBitStreamWriter bitstream = new RawBitStreamWriter(stream);

            bitstream.WriteBits(8, b.ConfigurationVersion);
            bitstream.WriteBits(8, b.AvcProfileIndication);
            bitstream.WriteBits(8, b.ProfileCompatibility);
            bitstream.WriteBits(8, b.AvcLevelIndication);

            bitstream.WriteBits(6, b.LengthSizeMinusOnePaddingBits);
            bitstream.WriteBits(2, b.LengthSizeMinusOne);
            bitstream.WriteBits(3, b.NumberOfSequenceParameterSetsPaddingBits);
            bitstream.WriteBits(5, b.SequenceParameterSets.Count);
            size += bitstream.WrittenBytes;

            foreach (H264SpsNalUnit sequenceParameterSetNALUnit in b.SequenceParameterSets)
            {
                uint nalSize = sequenceParameterSetNALUnit.CalculateSize();
                size += IsoReaderWriter.WriteUInt16(stream, (ushort)nalSize);
                using (var ms = new MemoryStream())
                {
                    H264SpsNalUnit.Build(ms, sequenceParameterSetNALUnit);
                    byte[] bytes = ms.ToArray();
                    size += await IsoReaderWriter.WriteBytesAsync(stream, bytes, 0, bytes.Length);
                }
            }

            size += IsoReaderWriter.WriteByte(stream, (byte)b.PictureParameterSets.Count);

            foreach (H264PpsNalUnit pictureParameterSetNALUnit in b.PictureParameterSets)
            {
                uint nalSize = pictureParameterSetNALUnit.CalculateSize();
                size += IsoReaderWriter.WriteUInt16(stream, (ushort)nalSize);
                using (var ms = new MemoryStream())
                {
                    H264PpsNalUnit.Build(ms, pictureParameterSetNALUnit);
                    byte[] bytes = ms.ToArray();
                    size += await IsoReaderWriter.WriteBytesAsync(stream, bytes, 0, bytes.Length);
                }
            }

            if (b.SequenceParameterSetExts.Count > 0 && (b.AvcProfileIndication == 100 || b.AvcProfileIndication == 110 || b.AvcProfileIndication == 122 || b.AvcProfileIndication == 144))
            {
                bitstream = new RawBitStreamWriter(stream);
                bitstream.WriteBits(6, b.SequenceParameterSets.Count);
                bitstream.WriteBits(2, b.ChromaFormat);
                bitstream.WriteBits(5, b.BitDepthLumaMinus8PaddingBits);
                bitstream.WriteBits(3, b.BitDepthLumaMinus8);
                bitstream.WriteBits(5, b.BitDepthChromaMinus8PaddingBits);
                bitstream.WriteBits(3, b.BitDepthChromaMinus8);
                size += bitstream.WrittenBytes;

                foreach (byte[] sequenceParameterSetExtNALUnit in b.SequenceParameterSetExts)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, (ushort)sequenceParameterSetExtNALUnit.Length);
                    size += await IsoReaderWriter.WriteBytesAsync(stream, sequenceParameterSetExtNALUnit, 0, sequenceParameterSetExtNALUnit.Length);
                }
            }

            return size;
        }

        public uint CalculateSize()
        {
            uint size = 5;
            size += 1;
            foreach (var sequenceParameterSetNALUnit in SequenceParameterSets)
            {
                size += 2;
                size += sequenceParameterSetNALUnit.CalculateSize();
            }
            size += 1;
            foreach (var pictureParameterSetNALUnit in PictureParameterSets)
            {
                size += 2;
                size += pictureParameterSetNALUnit.CalculateSize();
            }
            if (SequenceParameterSetExts.Count > 0 && (AvcProfileIndication == 100 || AvcProfileIndication == 110 || AvcProfileIndication == 122 || AvcProfileIndication == 144))
            {
                size += 4;
                foreach (byte[] sequenceParameterSetExtNALUnit in SequenceParameterSetExts)
                {
                    size += 2;
                    size += (uint)sequenceParameterSetExtNALUnit.Length;
                }
            }

            return size;
        }
    }

    public class H264PpsNalUnit
    {
        public H264PpsNalUnit()
        { }

        public H264PpsNalUnit(
            H264NalUnitHeader nalHeader,
            int picParameterSetId,
            int seqParameterSetId,
            bool entropyCodingModeFlag,
            bool bottomFieldPicOrderInFramePresentFlag,
            int numSliceGroupsMinus1,
            int sliceGroupMapType,
            int[] topLeft,
            int[] bottomRight,
            int[] runLengthMinus1,
            bool sliceGroupChangeDirectionFlag,
            int sliceGroupChangeRateMinus1,
            int numberBitsPerSliceGroupId,
            int picSizeInMapUnitsMinus1,
            int[] sliceGroupId,
            int numRefIdxL0ActiveMinus1,
            int numRefIdxL1ActiveMinus1,
            bool weightedPredFlag,
            int weightedBipredIdc,
            int picInitQpMinus26,
            int picInitQsMinus26,
            int chromaQpIndexOffset,
            bool deblockingFilterControlPresentFlag,
            bool constrainedIntraPredFlag,
            bool redundantPicCntPresentFlag,
            H264PpsExt extended)
        {
            NalHeader = nalHeader;
            PicParameterSetId = picParameterSetId;
            SeqParameterSetId = seqParameterSetId;
            EntropyCodingModeFlag = entropyCodingModeFlag;
            BottomFieldPicOrderInFramePresentFlag = bottomFieldPicOrderInFramePresentFlag;
            NumSliceGroupsMinus1 = numSliceGroupsMinus1;
            SliceGroupMapType = sliceGroupMapType;
            TopLeft = topLeft;
            BottomRight = bottomRight;
            RunLengthMinus1 = runLengthMinus1;
            SliceGroupChangeDirectionFlag = sliceGroupChangeDirectionFlag;
            SliceGroupChangeRateMinus1 = sliceGroupChangeRateMinus1;
            NumberBitsPerSliceGroupId = numberBitsPerSliceGroupId;
            PicSizeInMapUnitsMinus1 = picSizeInMapUnitsMinus1;
            SliceGroupId = sliceGroupId;
            NumRefIdxL0ActiveMinus1 = numRefIdxL0ActiveMinus1;
            NumRefIdxL1ActiveMinus1 = numRefIdxL1ActiveMinus1;
            WeightedPredFlag = weightedPredFlag;
            WeightedBipredIdc = weightedBipredIdc;
            PicInitQpMinus26 = picInitQpMinus26;
            PicInitQsMinus26 = picInitQsMinus26;
            ChromaQpIndexOffset = chromaQpIndexOffset;
            DeblockingFilterControlPresentFlag = deblockingFilterControlPresentFlag;
            ConstrainedIntraPredFlag = constrainedIntraPredFlag;
            RedundantPicCntPresentFlag = redundantPicCntPresentFlag;
            Extended = extended;
        }

        public H264NalUnitHeader NalHeader { get; set; }
        public int PicParameterSetId { get; set; }
        public int SeqParameterSetId { get; set; }
        public bool EntropyCodingModeFlag { get; set; }
        public bool BottomFieldPicOrderInFramePresentFlag { get; set; }
        public int NumSliceGroupsMinus1 { get; set; }
        public int SliceGroupMapType { get; set; }
        public int[] TopLeft { get; set; }
        public int[] BottomRight { get; set; }
        public int[] RunLengthMinus1 { get; set; }
        public bool SliceGroupChangeDirectionFlag { get; set; }
        public int SliceGroupChangeRateMinus1 { get; set; }
        public int NumberBitsPerSliceGroupId { get; set; }
        public int PicSizeInMapUnitsMinus1 { get; set; }
        public int[] SliceGroupId { get; set; }
        public int NumRefIdxL0ActiveMinus1 { get; set; }
        public int NumRefIdxL1ActiveMinus1 { get; set; }
        public bool WeightedPredFlag { get; set; }
        public int WeightedBipredIdc { get; set; }
        public int PicInitQpMinus26 { get; set; }
        public int PicInitQsMinus26 { get; set; }
        public int ChromaQpIndexOffset { get; set; }
        public bool DeblockingFilterControlPresentFlag { get; set; }
        public bool ConstrainedIntraPredFlag { get; set; }
        public bool RedundantPicCntPresentFlag { get; set; }
        public H264PpsExt Extended { get; set; }

        public static H264PpsNalUnit Parse(byte[] pps)
        {
            using (MemoryStream ms = new MemoryStream(pps))
            {
                return Parse((ushort)pps.Length, ms);
            }
        }

        public static H264PpsNalUnit Parse(ushort size, MemoryStream stream)
        {
            NalBitStreamReader bitstream = new NalBitStreamReader(stream);
            H264NalUnitHeader header = H264NalUnitHeader.ParseNALHeader(bitstream);

            int picParameterSetId = bitstream.ReadUE();
            int seqParameterSetId = bitstream.ReadUE();
            bool entropyCodingModeFlag = bitstream.ReadBit() != 0;
            bool bottomFieldPicOrderInFramePresentFlag = bitstream.ReadBit() != 0;
            int numSliceGroupsMinus1 = bitstream.ReadUE();

            int sliceGroupMapType = 0;
            int[] topLeft = null;
            int[] bottomRight = null;
            int[] runLengthMinus1 = null;
            bool sliceGroupChangeDirectionFlag = false;
            int sliceGroupChangeRateMinus1 = 0;
            int NumberBitsPerSliceGroupId = 0;
            int picSizeInMapUnitsMinus1 = 0;
            int[] sliceGroupId = null;

            if (numSliceGroupsMinus1 > 0)
            {
                sliceGroupMapType = bitstream.ReadUE();
                topLeft = new int[numSliceGroupsMinus1 + 1];
                bottomRight = new int[numSliceGroupsMinus1 + 1];
                runLengthMinus1 = new int[numSliceGroupsMinus1 + 1];
                if (sliceGroupMapType == 0)
                {
                    for (int iGroup = 0; iGroup <= numSliceGroupsMinus1; iGroup++)
                    {
                        runLengthMinus1[iGroup] = bitstream.ReadUE();
                    }
                }
                else if (sliceGroupMapType == 2)
                {
                    for (int iGroup = 0; iGroup < numSliceGroupsMinus1; iGroup++)
                    {
                        topLeft[iGroup] = bitstream.ReadUE();
                        bottomRight[iGroup] = bitstream.ReadUE();
                    }
                }
                else if (sliceGroupMapType == 3 || sliceGroupMapType == 4 || sliceGroupMapType == 5)
                {
                    sliceGroupChangeDirectionFlag = bitstream.ReadBit() != 0;
                    sliceGroupChangeRateMinus1 = bitstream.ReadUE();
                }
                else if (sliceGroupMapType == 6)
                {
                    if (numSliceGroupsMinus1 + 1 > 4)
                        NumberBitsPerSliceGroupId = 3;
                    else if (numSliceGroupsMinus1 + 1 > 2)
                        NumberBitsPerSliceGroupId = 2;
                    else
                        NumberBitsPerSliceGroupId = 1;
                    picSizeInMapUnitsMinus1 = bitstream.ReadUE();
                    sliceGroupId = new int[picSizeInMapUnitsMinus1 + 1];
                    for (int i = 0; i <= picSizeInMapUnitsMinus1; i++)
                    {
                        sliceGroupId[i] = bitstream.ReadBits(NumberBitsPerSliceGroupId);
                    }
                }
            }

            int numRefIdxL0ActiveMinus1 = bitstream.ReadUE();
            int numRefIdxL1ActiveMinus1 = bitstream.ReadUE();
            bool weightedPredFlag = bitstream.ReadBit() != 0;
            int weightedBipredIdc = bitstream.ReadBits(2);
            int picInitQpMinus26 = bitstream.ReadSE();
            int picInitQsMinus26 = bitstream.ReadSE();
            int chromaQpIndexOffset = bitstream.ReadSE();
            bool deblockingFilterControlPresentFlag = bitstream.ReadBit() != 0;
            bool constrainedIntraPredFlag = bitstream.ReadBit() != 0;
            bool redundantPicCntPresentFlag = bitstream.ReadBit() != 0;

            H264PpsExt extended = null;

            if (bitstream.HasMoreRBSPData(size))
            {
                extended = H264PpsExt.Parse(bitstream);
            }

            bitstream.ReadTrailingBits();

            H264PpsNalUnit pps = new H264PpsNalUnit(
                header,
                picParameterSetId,
                seqParameterSetId,
                entropyCodingModeFlag,
                bottomFieldPicOrderInFramePresentFlag,
                numSliceGroupsMinus1,
                sliceGroupMapType,
                topLeft,
                bottomRight,
                runLengthMinus1,
                sliceGroupChangeDirectionFlag,
                sliceGroupChangeRateMinus1,
                NumberBitsPerSliceGroupId,
                picSizeInMapUnitsMinus1,
                sliceGroupId,
                numRefIdxL0ActiveMinus1,
                numRefIdxL1ActiveMinus1,
                weightedPredFlag,
                weightedBipredIdc,
                picInitQpMinus26,
                picInitQsMinus26,
                chromaQpIndexOffset,
                deblockingFilterControlPresentFlag,
                constrainedIntraPredFlag,
                redundantPicCntPresentFlag,
                extended
                );

            return pps;
        }

        public static byte[] Build(H264PpsNalUnit pps)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Build(ms, pps);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public static uint Build(MemoryStream stream, H264PpsNalUnit nal)
        {
            NalBitStreamWriter bitstream = new NalBitStreamWriter(stream);
            H264NalUnitHeader.BuildNALHeader(bitstream, nal.NalHeader);
            bitstream.WriteUE((uint)nal.PicParameterSetId);
            bitstream.WriteUE((uint)nal.SeqParameterSetId);
            bitstream.WriteBit(nal.EntropyCodingModeFlag);
            bitstream.WriteBit(nal.BottomFieldPicOrderInFramePresentFlag);
            bitstream.WriteUE((uint)nal.NumSliceGroupsMinus1);

            if (nal.NumSliceGroupsMinus1 > 0)
            {
                bitstream.WriteUE((uint)nal.SliceGroupMapType);
                if (nal.SliceGroupMapType == 0)
                {
                    for (int iGroup = 0; iGroup <= nal.NumSliceGroupsMinus1; iGroup++)
                    {
                        bitstream.WriteUE((uint)nal.RunLengthMinus1[iGroup]);
                    }
                }
                else if (nal.SliceGroupMapType == 2)
                {
                    for (int iGroup = 0; iGroup < nal.NumSliceGroupsMinus1; iGroup++)
                    {
                        bitstream.WriteUE((uint)nal.TopLeft[iGroup]);
                        bitstream.WriteUE((uint)nal.BottomRight[iGroup]);
                    }
                }
                else if (nal.SliceGroupMapType == 3 || nal.SliceGroupMapType == 4 || nal.SliceGroupMapType == 5)
                {
                    bitstream.WriteBit(nal.SliceGroupChangeDirectionFlag);
                    bitstream.WriteUE((uint)nal.SliceGroupChangeRateMinus1);
                }
                else if (nal.SliceGroupMapType == 6)
                {
                    bitstream.WriteUE((uint)nal.PicSizeInMapUnitsMinus1);
                    for (int i = 0; i <= nal.PicSizeInMapUnitsMinus1; i++)
                    {
                        bitstream.WriteBits((uint)nal.NumberBitsPerSliceGroupId, nal.SliceGroupId[i]);
                    }
                }
            }

            bitstream.WriteUE((uint)nal.NumRefIdxL0ActiveMinus1);
            bitstream.WriteUE((uint)nal.NumRefIdxL1ActiveMinus1);
            bitstream.WriteBit(nal.WeightedPredFlag);
            bitstream.WriteBits(2, nal.WeightedBipredIdc);
            bitstream.WriteSE(nal.PicInitQpMinus26);
            bitstream.WriteSE(nal.PicInitQsMinus26);
            bitstream.WriteSE(nal.ChromaQpIndexOffset);
            bitstream.WriteBit(nal.DeblockingFilterControlPresentFlag);
            bitstream.WriteBit(nal.ConstrainedIntraPredFlag);
            bitstream.WriteBit(nal.RedundantPicCntPresentFlag);

            if (nal.Extended != null)
            {
                H264PpsExt.Build(bitstream, nal.Extended);
            }

            bitstream.WriteTrailingBits();

            return bitstream.WrittenBytes;
        }

        public uint CalculateSize()
        {
            // TODO: optimize?
            using (MemoryStream ms = new MemoryStream())
            {
                Build(ms, this);
                return (uint)ms.Length;
            }
        }
    }

    public class H264PpsExt
    {
        public H264PpsExt()
        { }

        public H264PpsExt(bool transform8x8ModeFlag, bool picScalingMatrixPresentFlag, ScalingMatrix scalingMatrix, int secondChromaQpIndexOffset)
        {
            Transform8x8ModeFlag = transform8x8ModeFlag;
            PicScalingMatrixPresentFlag = picScalingMatrixPresentFlag;
            ScalingMatrix = scalingMatrix;
            SecondChromaQpIndexOffset = secondChromaQpIndexOffset;
        }

        public bool Transform8x8ModeFlag { get; set; }
        public bool PicScalingMatrixPresentFlag { get; set; }
        public ScalingMatrix ScalingMatrix { get; set; }
        public bool PicScalingListPresentFlag { get; set; }
        public int SecondChromaQpIndexOffset { get; set; }

        public static H264PpsExt Parse(NalBitStreamReader bitstream)
        {
            bool transform8x8ModeFlag = bitstream.ReadBit() != 0;
            bool picScalingMatrixPresentFlag = bitstream.ReadBit() != 0;
            ScalingMatrix scalingMatrix = null;
            if (picScalingMatrixPresentFlag)
            {
                scalingMatrix = ScalingMatrix.Parse(bitstream);
            }

            int secondChromaQpIndexOffset = bitstream.ReadSE();

            H264PpsExt ppsExt = new H264PpsExt(
                transform8x8ModeFlag,
                picScalingMatrixPresentFlag,
                scalingMatrix,
                secondChromaQpIndexOffset
                );
            return ppsExt;
        }

        public static void Build(NalBitStreamWriter bitstream, H264PpsExt ext)
        {
            bitstream.WriteBit(ext.Transform8x8ModeFlag);
            bitstream.WriteBit(ext.PicScalingMatrixPresentFlag);
            if (ext.PicScalingMatrixPresentFlag)
            {
                ScalingMatrix.Build(bitstream, ext.ScalingMatrix);
            }

            bitstream.WriteSE(ext.SecondChromaQpIndexOffset);
        }
    }

    public class H264ChromaFormat
    {
        public static readonly H264ChromaFormat MONOCHROME = new H264ChromaFormat(0, 0, 0);
        public static readonly H264ChromaFormat YUV_420 = new H264ChromaFormat(1, 2, 2);
        public static readonly H264ChromaFormat YUV_422 = new H264ChromaFormat(2, 2, 1);
        public static readonly H264ChromaFormat YUV_444 = new H264ChromaFormat(3, 1, 1);

        public int Id { get; }
        public int SubWidth { get; }
        public int SubHeight { get; }

        public H264ChromaFormat(int id, int subWidth, int subHeight)
        {
            this.Id = id;
            this.SubWidth = subWidth;
            this.SubHeight = subHeight;
        }

        public static H264ChromaFormat FromId(int id)
        {
            if (id == MONOCHROME.Id)
            {
                return MONOCHROME;
            }
            else if (id == YUV_420.Id)
            {
                return YUV_420;
            }
            else if (id == YUV_422.Id)
            {
                return YUV_422;
            }
            else if (id == YUV_444.Id)
            {
                return YUV_444;
            }
            return null;
        }
    }

    public class H264SpsNalUnit
    {
        public H264SpsNalUnit(
            H264NalUnitHeader nalHeader,
            int profileIdc,
            bool constraintSet0Flag,
            bool constraintSet1Flag,
            bool constraintSet2Flag,
            bool constraintSet3Flag,
            bool constraintSet4Flag,
            bool constraintSet5Flag,
            int reservedZero2Bits,
            int levelIdc,
            int seqParameterSetId,
            H264ChromaFormat chromaFormat,
            bool residualColorTransformFlag,
            int bitDepthLumaMinus8,
            int bitDepthChromaMinus8,
            bool qpprimeYZeroTransformBypassFlag,
            int seqScalingMatrixPresent,
            ScalingMatrix scalingMatrix,
            int log2MaxFrameNumMinus4,
            int picOrderCntType,
            int log2MaxPicOrderCntLsbMinus4,
            bool deltaPicOrderAlwaysZeroFlag,
            int offsetForNonRefPic,
            int offsetForTopToBottomField,
            int numRefFramesInPicOrderCntCycle,
            int[] offsetForRefFrame,
            int numRefFrames,
            bool gapsInFrameNumValueAllowedFlag,
            int picWidthInMbsMinus1,
            int picHeightInMapUnitsMinus1,
            bool frameMbsOnlyFlag,
            bool mbAdaptiveFrameFieldFlag,
            bool direct8x8InferenceFlag,
            bool frameCroppingFlag,
            int frameCropLeftOffset,
            int frameCropRightOffset,
            int frameCropTopOffset,
            int frameCropBottomOffset,
            bool vuiParametersPresentFlag,
            H264VuiParameters vuiParameters)
        {
            NalHeader = nalHeader;
            ProfileIdc = profileIdc;
            ConstraintSet0Flag = constraintSet0Flag;
            ConstraintSet1Flag = constraintSet1Flag;
            ConstraintSet2Flag = constraintSet2Flag;
            ConstraintSet3Flag = constraintSet3Flag;
            ConstraintSet4Flag = constraintSet4Flag;
            ConstraintSet5Flag = constraintSet5Flag;
            ReservedZero2Bits = reservedZero2Bits;
            LevelIdc = levelIdc;
            SeqParameterSetId = seqParameterSetId;
            ChromaFormat = chromaFormat;
            ResidualColorTransformFlag = residualColorTransformFlag;
            BitDepthLumaMinus8 = bitDepthLumaMinus8;
            BitDepthChromaMinus8 = bitDepthChromaMinus8;
            QpprimeYZeroTransformBypassFlag = qpprimeYZeroTransformBypassFlag;
            SeqScalingMatrixPresent = seqScalingMatrixPresent;
            ScalingMatrix = scalingMatrix;
            Log2MaxFrameNumMinus4 = log2MaxFrameNumMinus4;
            PicOrderCntType = picOrderCntType;
            Log2MaxPicOrderCntLsbMinus4 = log2MaxPicOrderCntLsbMinus4;
            DeltaPicOrderAlwaysZeroFlag = deltaPicOrderAlwaysZeroFlag;
            OffsetForNonRefPic = offsetForNonRefPic;
            OffsetForTopToBottomField = offsetForTopToBottomField;
            NumRefFramesInPicOrderCntCycle = numRefFramesInPicOrderCntCycle;
            OffsetForRefFrame = offsetForRefFrame;
            NumRefFrames = numRefFrames;
            GapsInFrameNumValueAllowedFlag = gapsInFrameNumValueAllowedFlag;
            PicWidthInMbsMinus1 = picWidthInMbsMinus1;
            PicHeightInMapUnitsMinus1 = picHeightInMapUnitsMinus1;
            FrameMbsOnlyFlag = frameMbsOnlyFlag;
            MbAdaptiveFrameFieldFlag = mbAdaptiveFrameFieldFlag;
            Direct8x8InferenceFlag = direct8x8InferenceFlag;
            FrameCroppingFlag = frameCroppingFlag;
            FrameCropLeftOffset = frameCropLeftOffset;
            FrameCropRightOffset = frameCropRightOffset;
            FrameCropTopOffset = frameCropTopOffset;
            FrameCropBottomOffset = frameCropBottomOffset;
            VuiParametersPresentFlag = vuiParametersPresentFlag;
            VuiParameters = vuiParameters;
        }

        public H264NalUnitHeader NalHeader { get; set; }
        public int ProfileIdc { get; set; }
        public bool ConstraintSet0Flag { get; set; }
        public bool ConstraintSet1Flag { get; set; }
        public bool ConstraintSet2Flag { get; set; }
        public bool ConstraintSet3Flag { get; set; }
        public bool ConstraintSet4Flag { get; set; }
        public bool ConstraintSet5Flag { get; set; }
        public int ReservedZero2Bits { get; set; }
        public int LevelIdc { get; set; }
        public int SeqParameterSetId { get; set; }
        public H264ChromaFormat ChromaFormat { get; set; }
        public bool ResidualColorTransformFlag { get; set; }
        public int BitDepthLumaMinus8 { get; set; }
        public int BitDepthChromaMinus8 { get; set; }
        public bool QpprimeYZeroTransformBypassFlag { get; set; }
        public int SeqScalingMatrixPresent { get; set; }
        public ScalingMatrix ScalingMatrix { get; set; }
        public int Log2MaxFrameNumMinus4 { get; set; }
        public int PicOrderCntType { get; set; }
        public int Log2MaxPicOrderCntLsbMinus4 { get; set; }
        public bool DeltaPicOrderAlwaysZeroFlag { get; set; }
        public int OffsetForNonRefPic { get; set; }
        public int OffsetForTopToBottomField { get; set; }
        public int NumRefFramesInPicOrderCntCycle { get; set; }
        public int[] OffsetForRefFrame { get; set; }
        public int NumRefFrames { get; set; }
        public bool GapsInFrameNumValueAllowedFlag { get; set; }
        public int PicWidthInMbsMinus1 { get; set; }
        public int PicHeightInMapUnitsMinus1 { get; set; }
        public bool FrameMbsOnlyFlag { get; set; }
        public bool MbAdaptiveFrameFieldFlag { get; set; }
        public bool Direct8x8InferenceFlag { get; set; }
        public bool FrameCroppingFlag { get; set; }
        public int FrameCropLeftOffset { get; set; }
        public int FrameCropRightOffset { get; set; }
        public int FrameCropTopOffset { get; set; }
        public int FrameCropBottomOffset { get; set; }
        public bool VuiParametersPresentFlag { get; set; }
        public H264VuiParameters VuiParameters { get; set; }

        public static H264SpsNalUnit Parse(byte[] sps)
        {
            using (MemoryStream ms = new MemoryStream(sps))
            {
                return Parse((ushort)sps.Length, ms);
            }
        }

        public static H264SpsNalUnit Parse(ushort size, MemoryStream stream)
        {
            NalBitStreamReader bitstream = new NalBitStreamReader(stream);
            H264NalUnitHeader header = H264NalUnitHeader.ParseNALHeader(bitstream);
            
            // 2nd byte is profile IDC
            int profileIdc = bitstream.ReadBits(8);

            // 3rd byte
            bool constraintSet0Flag = bitstream.ReadBit() != 0;
            bool constraintSet1Flag = bitstream.ReadBit() != 0;
            bool constraintSet2Flag = bitstream.ReadBit() != 0;
            bool constraintSet3Flag = bitstream.ReadBit() != 0;
            bool constraintSet4Flag = bitstream.ReadBit() != 0;
            bool constraintSet5Flag = bitstream.ReadBit() != 0;

            int reservedZero2Bits = bitstream.ReadBits(2);
            if (reservedZero2Bits != 0)
                throw new Exception("Reserved zero 2 bits are not zero!");

            // 4th byte level IDC
            int levelIdc = bitstream.ReadBits(8);

            int seqParameterSetId = bitstream.ReadUE();

            H264ChromaFormat chromaFormat = null;
            bool residualColorTransformFlag = false;
            int bitDepthLumaMinus8 = 0;
            int bitDepthChromaMinus8 = 0;
            bool qpprimeYZeroTransformBypassFlag = false;
            int seqScalingMatrixPresent = 0;
            ScalingMatrix scalingMatrix = null;
            if (profileIdc == 100 || profileIdc == 110 || profileIdc == 122 || profileIdc == 144)
            {
                int chromaFormatIdc = bitstream.ReadUE();
                chromaFormat = H264ChromaFormat.FromId(chromaFormatIdc);
                if (chromaFormat.Id == 3)
                {
                    residualColorTransformFlag = bitstream.ReadBit() != 0;
                }

                bitDepthLumaMinus8 = bitstream.ReadUE();
                bitDepthChromaMinus8 = bitstream.ReadUE();
                qpprimeYZeroTransformBypassFlag = bitstream.ReadBit() != 0;
                seqScalingMatrixPresent = bitstream.ReadBit();
                if (seqScalingMatrixPresent != 0)
                {
                    scalingMatrix = ScalingMatrix.Parse(bitstream);
                }
            }

            int log2MaxFrameNumMinus4 = bitstream.ReadUE();
            int picOrderCntType = bitstream.ReadUE();
            int log2MaxPicOrderCntLsbMinus4 = 0;
            bool deltaPicOrderAlwaysZeroFlag = false;
            int offsetForNonRefPic = 0;
            int offsetForTopToBottomField = 0;
            int numRefFramesInPicOrderCntCycle = 0;
            int[] offsetForRefFrame = null;

            if (picOrderCntType == 0)
            {
                log2MaxPicOrderCntLsbMinus4 = bitstream.ReadUE();
            }
            else if (picOrderCntType == 1)
            {
                deltaPicOrderAlwaysZeroFlag = bitstream.ReadBit() != 0;
                offsetForNonRefPic = bitstream.ReadSE();
                offsetForTopToBottomField = bitstream.ReadSE();
                numRefFramesInPicOrderCntCycle = bitstream.ReadUE();
                offsetForRefFrame = new int[numRefFramesInPicOrderCntCycle];
                for (int i = 0; i < numRefFramesInPicOrderCntCycle; i++)
                {
                    offsetForRefFrame[i] = bitstream.ReadSE();
                }
            }

            int numRefFrames = bitstream.ReadUE();
            bool gapsInFrameNumValueAllowedFlag = bitstream.ReadBit() != 0;
            int picWidthInMbsMinus1 = bitstream.ReadUE();
            int picHeightInMapUnitsMinus1 = bitstream.ReadUE();
            bool frameMbsOnlyFlag = bitstream.ReadBit() != 0;
            bool mbAdaptiveFrameFieldFlag = false;

            if (!frameMbsOnlyFlag)
            {
                mbAdaptiveFrameFieldFlag = bitstream.ReadBit() != 0;
            }

            bool direct8x8InferenceFlag = bitstream.ReadBit() != 0;
            bool frameCroppingFlag = bitstream.ReadBit() != 0;
            int frameCropLeftOffset = 0;
            int frameCropRightOffset = 0;
            int frameCropTopOffset = 0;
            int frameCropBottomOffset = 0;

            if (frameCroppingFlag)
            {
                frameCropLeftOffset = bitstream.ReadUE();
                frameCropRightOffset = bitstream.ReadUE();
                frameCropTopOffset = bitstream.ReadUE();
                frameCropBottomOffset = bitstream.ReadUE();
            }

            bool vuiParametersPresentFlag = bitstream.ReadBit() != 0;
            H264VuiParameters vuiParams = null;

            if (vuiParametersPresentFlag)
            {
                vuiParams = H264VuiParameters.Parse(bitstream);
            }

            bitstream.ReadTrailingBits();

            H264SpsNalUnit sps = new H264SpsNalUnit(
                header,
                profileIdc,
                constraintSet0Flag,
                constraintSet1Flag,
                constraintSet2Flag,
                constraintSet3Flag,
                constraintSet4Flag,
                constraintSet5Flag,
                reservedZero2Bits,
                levelIdc,
                seqParameterSetId,
                chromaFormat,
                residualColorTransformFlag,
                bitDepthLumaMinus8,
                bitDepthChromaMinus8,
                qpprimeYZeroTransformBypassFlag,
                seqScalingMatrixPresent,
                scalingMatrix,
                log2MaxFrameNumMinus4,
                picOrderCntType,
                log2MaxPicOrderCntLsbMinus4,
                deltaPicOrderAlwaysZeroFlag,
                offsetForNonRefPic,
                offsetForTopToBottomField,
                numRefFramesInPicOrderCntCycle,
                offsetForRefFrame,
                numRefFrames,
                gapsInFrameNumValueAllowedFlag,
                picWidthInMbsMinus1,
                picHeightInMapUnitsMinus1,
                frameMbsOnlyFlag,
                mbAdaptiveFrameFieldFlag,
                direct8x8InferenceFlag,
                frameCroppingFlag,
                frameCropLeftOffset,
                frameCropRightOffset,
                frameCropTopOffset,
                frameCropBottomOffset,
                vuiParametersPresentFlag,
                vuiParams
                );
            return sps;
        }

        public static byte[] Build(H264SpsNalUnit sps)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Build(ms, sps);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public static uint Build(MemoryStream stream, H264SpsNalUnit nal)
        {
            NalBitStreamWriter bitstream = new NalBitStreamWriter(stream);
            H264NalUnitHeader.BuildNALHeader(bitstream, nal.NalHeader);
            
            // 2nd byte is profile IDC
            bitstream.WriteBits(8, nal.ProfileIdc);

            // 3rd byte
            bitstream.WriteBit(nal.ConstraintSet0Flag);
            bitstream.WriteBit(nal.ConstraintSet1Flag);
            bitstream.WriteBit(nal.ConstraintSet2Flag);
            bitstream.WriteBit(nal.ConstraintSet3Flag);
            bitstream.WriteBit(nal.ConstraintSet4Flag);
            bitstream.WriteBit(nal.ConstraintSet5Flag);

            bitstream.WriteBits(2, nal.ReservedZero2Bits);
            
            // 4th byte level IDC
            bitstream.WriteBits(8, nal.LevelIdc);

            bitstream.WriteUE((uint)nal.SeqParameterSetId);

            if (nal.ProfileIdc == 100 || nal.ProfileIdc == 110 || nal.ProfileIdc == 122 || nal.ProfileIdc == 144)
            {
                var chromaFormat = nal.ChromaFormat;
                if (chromaFormat == null)
                    chromaFormat = H264ChromaFormat.FromId(1); // default

                bitstream.WriteUE((uint)chromaFormat.Id);
                if (chromaFormat.Id == 3)
                {
                    bitstream.WriteBit(nal.ResidualColorTransformFlag);
                }

                bitstream.WriteUE((uint)nal.BitDepthLumaMinus8);
                bitstream.WriteUE((uint)nal.BitDepthChromaMinus8);
                bitstream.WriteBit(nal.QpprimeYZeroTransformBypassFlag);
                bitstream.WriteBit(nal.SeqScalingMatrixPresent);
                if (nal.SeqScalingMatrixPresent != 0)
                {
                    ScalingMatrix.Build(bitstream, nal.ScalingMatrix);
                }
            }

            bitstream.WriteUE((uint)nal.Log2MaxFrameNumMinus4);
            bitstream.WriteUE((uint)nal.PicOrderCntType);

            if (nal.PicOrderCntType == 0)
            {
                bitstream.WriteUE((uint)nal.Log2MaxPicOrderCntLsbMinus4);
            }
            else if (nal.PicOrderCntType == 1)
            {
                bitstream.WriteBit(nal.DeltaPicOrderAlwaysZeroFlag);
                bitstream.WriteSE(nal.OffsetForNonRefPic);
                bitstream.WriteSE(nal.OffsetForTopToBottomField);
                bitstream.WriteUE((uint)nal.NumRefFramesInPicOrderCntCycle);
                for (int i = 0; i < nal.NumRefFramesInPicOrderCntCycle; i++)
                {
                    bitstream.WriteSE(nal.OffsetForRefFrame[i]);
                }
            }

            bitstream.WriteUE((uint)nal.NumRefFrames);
            bitstream.WriteBit(nal.GapsInFrameNumValueAllowedFlag);
            bitstream.WriteUE((uint)nal.PicWidthInMbsMinus1);
            bitstream.WriteUE((uint)nal.PicHeightInMapUnitsMinus1);
            bitstream.WriteBit(nal.FrameMbsOnlyFlag);

            if (!nal.FrameMbsOnlyFlag)
            {
                bitstream.WriteBit(nal.MbAdaptiveFrameFieldFlag);
            }

            bitstream.WriteBit(nal.Direct8x8InferenceFlag);
            bitstream.WriteBit(nal.FrameCroppingFlag);

            if (nal.FrameCroppingFlag)
            {
                bitstream.WriteUE((uint)nal.FrameCropLeftOffset);
                bitstream.WriteUE((uint)nal.FrameCropRightOffset);
                bitstream.WriteUE((uint)nal.FrameCropTopOffset);
                bitstream.WriteUE((uint)nal.FrameCropBottomOffset);
            }

            bitstream.WriteBit(nal.VuiParametersPresentFlag);

            if (nal.VuiParametersPresentFlag)
            {
                H264VuiParameters.Build(bitstream, nal.VuiParameters);
            }

            bitstream.WriteTrailingBits();

            return bitstream.WrittenBytes;
        }

        public uint CalculateSize()
        {
            // TODO: optimize?
            using (MemoryStream ms = new MemoryStream())
            {
                Build(ms, this);
                return (uint)ms.Length;
            }
        }

        public (ushort Width, ushort Height) CalculateDimensions()
        {
            int width = (this.PicWidthInMbsMinus1 + 1) * 16;
            int mult = 2;
            if (this.FrameMbsOnlyFlag)
            {
                mult = 1;
            }
            int height = 16 * (this.PicHeightInMapUnitsMinus1 + 1) * mult;
            if (this.FrameCroppingFlag)
            {
                var chromaFormat = this.ChromaFormat;
                if (chromaFormat == null)
                    chromaFormat = H264ChromaFormat.FromId(1); // default

                int chromaArrayType = 0;
                if (!this.ResidualColorTransformFlag)
                {
                    chromaArrayType = chromaFormat.Id;
                }
                int cropUnitX = 1;
                int cropUnitY = mult;
                if (chromaArrayType != 0)
                {
                    cropUnitX = chromaFormat.SubWidth;
                    cropUnitY = chromaFormat.SubHeight * mult;
                }

                width -= cropUnitX * (this.FrameCropLeftOffset + this.FrameCropRightOffset);
                height -= cropUnitY * (this.FrameCropTopOffset + this.FrameCropBottomOffset);
            }
            return ((ushort)width, (ushort)height);
        }

        public (int Timescale, int FrameTick) CalculateTimescale()
        {
            int timescale = 0;
            int frametick = 0;
            var vui = this.VuiParameters;
            if (vui != null && vui.TimingInfoPresentFlag)
            {
                // MaxFPS = Ceil( time_scale / ( 2 * num_units_in_tick ) )
                timescale = vui.TimeScale;
                frametick = vui.NumUnitsInTick;
                
                if (timescale == 0 || frametick == 0)
                {
                    if (Log.WarnEnabled) Log.Warn($"Invalid values in vui: timescale: {timescale} and frametick: {frametick}.");
                    timescale = 0;
                    frametick = 0;
                }
            }
            else
            {
                if (Log.WarnEnabled) Log.Warn("Can't determine frame rate because SPS does not contain vuiParams");
            }

            return (timescale, frametick);
        }
    }

    public class H264VuiParameters
    {
        public H264VuiParameters(
            bool aspectRatioInfoPresentFlag,
            int aspectRatio,
            int sarWidth,
            int sarHeight,
            bool overscanInfoPresentFlag,
            bool overscanAppropriateFlag,
            bool videoSignalTypePresentFlag,
            int videoFormat,
            bool videoFullRangeFlag,
            bool colorDescriptionPresentFlag,
            int colorPrimaries,
            int transferCharacteristics,
            int matrixCoefficients,
            bool chromaLocInfoPresentFlag,
            int chromaSampleLocTypeTopField,
            int chromaSampleLocTypeBottomField,
            bool timingInfoPresentFlag,
            int numUnitsInTick,
            int timeScale,
            bool fixedFrameRateFlag,
            bool nalHrdParametersPresentFlag,
            H264HrdParameters nalHrdParams,
            bool vclHrdParametersPresentFlag,
            H264HrdParameters vclHrdParams,
            bool lowDelayHrdFlag,
            bool picStructPresentFlag,
            bool bitstreamRestrictionFlag,
            H264BitstreamRestriction bitstreamRestriction)
        {
            AspectRatioInfoPresentFlag = aspectRatioInfoPresentFlag;
            AspectRatio = aspectRatio;
            SarWidth = sarWidth;
            SarHeight = sarHeight;
            OverscanInfoPresentFlag = overscanInfoPresentFlag;
            OverscanAppropriateFlag = overscanAppropriateFlag;
            VideoSignalTypePresentFlag = videoSignalTypePresentFlag;
            VideoFormat = videoFormat;
            VideoFullRangeFlag = videoFullRangeFlag;
            ColorDescriptionPresentFlag = colorDescriptionPresentFlag;
            ColorPrimaries = colorPrimaries;
            TransferCharacteristics = transferCharacteristics;
            MatrixCoefficients = matrixCoefficients;
            ChromaLocInfoPresentFlag = chromaLocInfoPresentFlag;
            ChromaSampleLocTypeTopField = chromaSampleLocTypeTopField;
            ChromaSampleLocTypeBottomField = chromaSampleLocTypeBottomField;
            TimingInfoPresentFlag = timingInfoPresentFlag;
            NumUnitsInTick = numUnitsInTick;
            TimeScale = timeScale;
            FixedFrameRateFlag = fixedFrameRateFlag;
            NalHrdParametersPresentFlag = nalHrdParametersPresentFlag;
            NalHrdParams = nalHrdParams;
            VclHrdParametersPresentFlag = vclHrdParametersPresentFlag;
            VclHrdParams = vclHrdParams;
            LowDelayHrdFlag = lowDelayHrdFlag;
            PicStructPresentFlag = picStructPresentFlag;
            BitstreamRestrictionFlag = bitstreamRestrictionFlag;
            BitstreamRestriction = bitstreamRestriction;
        }

        public bool AspectRatioInfoPresentFlag { get; set; }
        public int AspectRatio { get; set; }
        public int SarWidth { get; set; }
        public int SarHeight { get; set; }
        public bool OverscanInfoPresentFlag { get; set; }
        public bool OverscanAppropriateFlag { get; set; }
        public bool VideoSignalTypePresentFlag { get; set; }
        public int VideoFormat { get; set; }
        public bool VideoFullRangeFlag { get; set; }
        public bool ColorDescriptionPresentFlag { get; set; }
        public int ColorPrimaries { get; set; }
        public int TransferCharacteristics { get; set; }
        public int MatrixCoefficients { get; set; }
        public bool ChromaLocInfoPresentFlag { get; set; }
        public int ChromaSampleLocTypeTopField { get; set; }
        public int ChromaSampleLocTypeBottomField { get; set; }
        public bool TimingInfoPresentFlag { get; set; }
        public int NumUnitsInTick { get; set; }
        public int TimeScale { get; set; }
        public bool FixedFrameRateFlag { get; set; }
        public bool NalHrdParametersPresentFlag { get; set; }
        public H264HrdParameters NalHrdParams { get; set; }
        public bool VclHrdParametersPresentFlag { get; set; }
        public H264HrdParameters VclHrdParams { get; set; }
        public bool LowDelayHrdFlag { get; set; }
        public bool PicStructPresentFlag { get; set; }
        public bool BitstreamRestrictionFlag { get; set; }
        public H264BitstreamRestriction BitstreamRestriction { get; set; }

        public static H264VuiParameters Parse(NalBitStreamReader bitstream)
        {
            bool aspectRatioInfoPresentFlag = bitstream.ReadBit() != 0;
            int aspectRatio = 0;
            int sarWidth = 0;
            int sarHeight = 0;
            if (aspectRatioInfoPresentFlag)
            {
                aspectRatio = bitstream.ReadBits(8);
                if (aspectRatio == 255)
                {
                    sarWidth = bitstream.ReadBits(16);
                    sarHeight = bitstream.ReadBits(16);
                }
            }

            bool overscanInfoPresentFlag = bitstream.ReadBit() != 0;
            bool overscanAppropriateFlag = false;
            if (overscanInfoPresentFlag)
            {
                overscanAppropriateFlag = bitstream.ReadBit() != 0;
            }

            bool videoSignalTypePresentFlag = bitstream.ReadBit() != 0;
            int videoFormat = 0;
            bool videoFullRangeFlag = false;
            bool colourDescriptionPresentFlag = false;
            int colourPrimaries = 0;
            int transferCharacteristics = 0;
            int matrixCoefficients = 0;

            if (videoSignalTypePresentFlag)
            {
                videoFormat = bitstream.ReadBits(3);
                videoFullRangeFlag = bitstream.ReadBit() != 0;
                colourDescriptionPresentFlag = bitstream.ReadBit() != 0;

                if (colourDescriptionPresentFlag)
                {
                    colourPrimaries = bitstream.ReadBits(8);
                    transferCharacteristics = bitstream.ReadBits(8);
                    matrixCoefficients = bitstream.ReadBits(8);
                }
            }

            bool chromaLocInfoPresentFlag = bitstream.ReadBit() != 0;
            int chromaSampleLocTypeTopField = 0;
            int chromaSampleLocTypeBottomField = 0;

            if (chromaLocInfoPresentFlag)
            {
                chromaSampleLocTypeTopField = bitstream.ReadUE();
                chromaSampleLocTypeBottomField = bitstream.ReadUE();
            }

            bool timingInfoPresentFlag = bitstream.ReadBit() != 0;
            int numUnitsInTick = 0;
            int timeScale = 0;
            bool fixedFrameRateFlag = false;
            if (timingInfoPresentFlag)
            {
                numUnitsInTick = bitstream.ReadBits(32);
                timeScale = bitstream.ReadBits(32);
                fixedFrameRateFlag = bitstream.ReadBit() != 0;
            }

            bool nalHrdParametersPresentFlag = bitstream.ReadBit() != 0;
            H264HrdParameters nalHRDParams = null;
            if (nalHrdParametersPresentFlag)
            {
                nalHRDParams = H264HrdParameters.Parse(bitstream);
            }

            bool vclHrdParametersPresentFlag = bitstream.ReadBit() != 0;
            H264HrdParameters vclHRDParams = null;
            if (vclHrdParametersPresentFlag)
            {
                vclHRDParams = H264HrdParameters.Parse(bitstream);
            }

            bool lowDelayHrdFlag = false;
            if (nalHrdParametersPresentFlag || vclHrdParametersPresentFlag)
            {
                lowDelayHrdFlag = bitstream.ReadBit() != 0;
            }

            bool picStructPresentFlag = bitstream.ReadBit() != 0;
            bool bitstreamRestrictionFlag = bitstream.ReadBit() != 0;
            H264BitstreamRestriction bitstreamRestriction = null;
            if (bitstreamRestrictionFlag)
            {
                bitstreamRestriction = new H264BitstreamRestriction();
                bitstreamRestriction.MotionVectorsOverPicBoundariesFlag = bitstream.ReadBit() != 0;
                bitstreamRestriction.MaxBytesPerPicDenom = bitstream.ReadUE();
                bitstreamRestriction.MaxBitsPerMbDenom = bitstream.ReadUE();
                bitstreamRestriction.Log2MaxMvLengthHorizontal = bitstream.ReadUE();
                bitstreamRestriction.Log2MaxMvLengthVertical = bitstream.ReadUE();
                bitstreamRestriction.NumReorderFrames = bitstream.ReadUE();
                bitstreamRestriction.MaxDecFrameBuffering = bitstream.ReadUE();
            }

            return new H264VuiParameters(
                aspectRatioInfoPresentFlag,
                aspectRatio,
                sarWidth,
                sarHeight,
                overscanInfoPresentFlag,
                overscanAppropriateFlag,
                videoSignalTypePresentFlag,
                videoFormat,
                videoFullRangeFlag,
                colourDescriptionPresentFlag,
                colourPrimaries,
                transferCharacteristics,
                matrixCoefficients,
                chromaLocInfoPresentFlag,
                chromaSampleLocTypeTopField,
                chromaSampleLocTypeBottomField,
                timingInfoPresentFlag,
                numUnitsInTick,
                timeScale,
                fixedFrameRateFlag,
                nalHrdParametersPresentFlag,
                nalHRDParams,
                vclHrdParametersPresentFlag,
                vclHRDParams,
                lowDelayHrdFlag,
                picStructPresentFlag,
                bitstreamRestrictionFlag,
                bitstreamRestriction
                );
        }

        public static void Build(NalBitStreamWriter bitstream, H264VuiParameters vui)
        {
            bitstream.WriteBit(vui.AspectRatioInfoPresentFlag);
            if (vui.AspectRatioInfoPresentFlag)
            {
                bitstream.WriteBits(8, vui.AspectRatio);
                if (vui.AspectRatio == 255)
                {
                    bitstream.WriteBits(16, vui.SarWidth);
                    bitstream.WriteBits(16, vui.SarHeight);
                }
            }

            bitstream.WriteBit(vui.OverscanInfoPresentFlag);
            if (vui.OverscanInfoPresentFlag)
            {
                bitstream.WriteBit(vui.OverscanAppropriateFlag);
            }

            bitstream.WriteBit(vui.VideoSignalTypePresentFlag);

            if (vui.VideoSignalTypePresentFlag)
            {
                bitstream.WriteBits(3, vui.VideoFormat);
                bitstream.WriteBit(vui.VideoFullRangeFlag);
                bitstream.WriteBit(vui.ColorDescriptionPresentFlag);

                if (vui.ColorDescriptionPresentFlag)
                {
                    bitstream.WriteBits(8, vui.ColorPrimaries);
                    bitstream.WriteBits(8, vui.TransferCharacteristics);
                    bitstream.WriteBits(8, vui.MatrixCoefficients);
                }
            }

            bitstream.WriteBit(vui.ChromaLocInfoPresentFlag);

            if (vui.ChromaLocInfoPresentFlag)
            {
                bitstream.WriteUE((uint)vui.ChromaSampleLocTypeTopField);
                bitstream.WriteUE((uint)vui.ChromaSampleLocTypeBottomField);
            }

            bitstream.WriteBit(vui.TimingInfoPresentFlag);
            if (vui.TimingInfoPresentFlag)
            {
                bitstream.WriteBits(32, vui.NumUnitsInTick);
                bitstream.WriteBits(32, vui.TimeScale);
                bitstream.WriteBit(vui.FixedFrameRateFlag);
            }

            bitstream.WriteBit(vui.NalHrdParametersPresentFlag);
            if (vui.NalHrdParametersPresentFlag)
            {
                H264HrdParameters.Build(bitstream, vui.NalHrdParams);
            }

            bitstream.WriteBit(vui.VclHrdParametersPresentFlag);

            if (vui.VclHrdParametersPresentFlag)
            {
                H264HrdParameters.Build(bitstream, vui.VclHrdParams);
            }

            if (vui.NalHrdParametersPresentFlag || vui.VclHrdParametersPresentFlag)
            {
                bitstream.WriteBit(vui.LowDelayHrdFlag);
            }

            bitstream.WriteBit(vui.PicStructPresentFlag);
            bitstream.WriteBit(vui.BitstreamRestrictionFlag);
            if (vui.BitstreamRestrictionFlag)
            {
                bitstream.WriteBit(vui.BitstreamRestriction.MotionVectorsOverPicBoundariesFlag);
                bitstream.WriteUE((uint)vui.BitstreamRestriction.MaxBytesPerPicDenom);
                bitstream.WriteUE((uint)vui.BitstreamRestriction.MaxBitsPerMbDenom);
                bitstream.WriteUE((uint)vui.BitstreamRestriction.Log2MaxMvLengthHorizontal);
                bitstream.WriteUE((uint)vui.BitstreamRestriction.Log2MaxMvLengthVertical);
                bitstream.WriteUE((uint)vui.BitstreamRestriction.NumReorderFrames);
                bitstream.WriteUE((uint)vui.BitstreamRestriction.MaxDecFrameBuffering);
            }
        }
    }

    public sealed class H264BitstreamRestriction
    {
        public bool MotionVectorsOverPicBoundariesFlag { get; set; }
        public int MaxBytesPerPicDenom { get; set; }
        public int MaxBitsPerMbDenom { get; set; }
        public int Log2MaxMvLengthHorizontal { get; set; }
        public int Log2MaxMvLengthVertical { get; set; }
        public int NumReorderFrames { get; set; }
        public int MaxDecFrameBuffering { get; set; }
    }

    public class H264HrdParameters
    {
        public H264HrdParameters(
            int cpbCntMinus1,
            int bitRateScale,
            int cpbSizeScale,
            int[] bitRateValueMinus1,
            int[] cpbSizeValueMinus1,
            bool[] cbrFlag,
            int initialCpbRemovalDelayLengthMinus1,
            int cpbRemovalDelayLengthMinus1,
            int dpbOutputDelayLengthMinus1,
            int timeOffsetLength)
        {
            CpbCntMinus1 = cpbCntMinus1;
            BitRateScale = bitRateScale;
            CpbSizeScale = cpbSizeScale;
            BitRateValueMinus1 = bitRateValueMinus1;
            CpbSizeValueMinus1 = cpbSizeValueMinus1;
            CbrFlag = cbrFlag;
            InitialCpbRemovalDelayLengthMinus1 = initialCpbRemovalDelayLengthMinus1;
            CpbRemovalDelayLengthMinus1 = cpbRemovalDelayLengthMinus1;
            DpbOutputDelayLengthMinus1 = dpbOutputDelayLengthMinus1;
            TimeOffsetLength = timeOffsetLength;
        }

        public int CpbCntMinus1 { get; set; }
        public int BitRateScale { get; set; }
        public int CpbSizeScale { get; set; }
        public int[] BitRateValueMinus1 { get; set; }
        public int[] CpbSizeValueMinus1 { get; set; }
        public bool[] CbrFlag { get; set; }
        public int InitialCpbRemovalDelayLengthMinus1 { get; set; }
        public int CpbRemovalDelayLengthMinus1 { get; set; }
        public int DpbOutputDelayLengthMinus1 { get; set; }
        public int TimeOffsetLength { get; set; }

        public static H264HrdParameters Parse(NalBitStreamReader bitstream)
        {
            int cpbCntMinus1 = bitstream.ReadUE();
            int bitRateScale = bitstream.ReadBits(4);
            int cpbSizeScale = bitstream.ReadBits(4);
            int[] bitRateValueMinus1 = new int[cpbCntMinus1 + 1];
            int[] cpbSizeValueMinus1 = new int[cpbCntMinus1 + 1];
            bool[] cbrFlag = new bool[cpbCntMinus1 + 1];

            for (int SchedSelIdx = 0; SchedSelIdx <= cpbCntMinus1; SchedSelIdx++)
            {
                bitRateValueMinus1[SchedSelIdx] = bitstream.ReadUE();
                cpbSizeValueMinus1[SchedSelIdx] = bitstream.ReadUE();
                cbrFlag[SchedSelIdx] = bitstream.ReadBit() != 0;
            }

            int initialCpbRemovalDelayLengthMinus1 = bitstream.ReadBits(5);
            int cpbRemovalDelayLengthMinus1 = bitstream.ReadBits(5);
            int dpbOutputDelayLengthMinus1 = bitstream.ReadBits(5);
            int timeOffsetLength = bitstream.ReadBits(5);

            return new H264HrdParameters(
                cpbCntMinus1,
                bitRateScale,
                cpbSizeScale,
                bitRateValueMinus1,
                cpbSizeValueMinus1,
                cbrFlag,
                initialCpbRemovalDelayLengthMinus1,
                cpbRemovalDelayLengthMinus1,
                dpbOutputDelayLengthMinus1,
                timeOffsetLength
                );
        }

        public static void Build(NalBitStreamWriter bitstream, H264HrdParameters hrd)
        {
            bitstream.WriteUE((uint)hrd.CpbCntMinus1);
            bitstream.WriteBits(4, hrd.BitRateScale);
            bitstream.WriteBits(4, hrd.CpbSizeScale);

            for (int SchedSelIdx = 0; SchedSelIdx <= hrd.CpbCntMinus1; SchedSelIdx++)
            {
                bitstream.WriteUE((uint)hrd.BitRateValueMinus1[SchedSelIdx]);
                bitstream.WriteUE((uint)hrd.CpbSizeValueMinus1[SchedSelIdx]);
                bitstream.WriteBit(hrd.CbrFlag[SchedSelIdx]);
            }

            bitstream.WriteBits(5, hrd.InitialCpbRemovalDelayLengthMinus1);
            bitstream.WriteBits(5, hrd.CpbRemovalDelayLengthMinus1);
            bitstream.WriteBits(5, hrd.DpbOutputDelayLengthMinus1);
            bitstream.WriteBits(5, hrd.TimeOffsetLength);
        }
    }

    public class H264NalUnitHeader
    {
        public H264NalUnitHeader(int nalRefIdc, int nalUnitType)
        {
            NalRefIdc = nalRefIdc;
            NalUnitType = nalUnitType;
        }

        public int NalRefIdc { get; set; }
        public int NalUnitType { get; set; }
        
        public bool IsVCL()
        {
            return NalUnitType >= 1 && NalUnitType <= 5;
        }

        public bool IsReference()
        {
            return NalRefIdc > 0;
        }

        public static H264NalUnitHeader ParseNALHeader(NalBitStreamReader bitstream)
        {
            if (bitstream.ReadBit() != 0)
                throw new Exception("Invalid NAL header!");

            int nalRefIdc = bitstream.ReadBits(2);
            int nalUnitType = bitstream.ReadBits(5);
            H264NalUnitHeader header = new H264NalUnitHeader(
                nalRefIdc,
                nalUnitType);
            return header;
        }

        public static void BuildNALHeader(NalBitStreamWriter bitstream, H264NalUnitHeader header)
        {
            bitstream.WriteBit(0); // forbidden 0
            bitstream.WriteBits(2, header.NalRefIdc);
            bitstream.WriteBits(5, header.NalUnitType);
        }
    }
}
