﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMp4
{
    public class H264Track : TrackBase
    {
        public Dictionary<int, H264SpsNalUnit> Sps { get; set; } = new Dictionary<int, H264SpsNalUnit>();
        public Dictionary<int, H264PpsNalUnit> Pps { get; set; } = new Dictionary<int, H264PpsNalUnit>();

        public H264Track(uint trackID, uint timescale) : base(trackID)
        {
            base.Handler = "vide";
            this.Timescale = timescale;
        }

        private H264NalSliceHeader _lastSliceHeader;
        private List<byte[]> _nalBuffer = new List<byte[]>();

        public static string ToHexString(byte[] data)
        {
#if !NETCOREAPP
            string hexString = BitConverter.ToString(data);
            hexString = hexString.Replace("-", "");
            return hexString;
#else
            return Convert.ToHexString(data);
#endif
        }

        public override async Task ProcessSampleAsync(byte[] sample, uint duration)
        {
            var header = H264NalUnitHeader.ParseNALHeader(sample[0]);
            if (header.NalUnitType == H264NalUnitTypes.SPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed SPS: {ToHexString(sample)}");
                var sps = H264SpsNalUnit.Parse(sample);
                if (!Sps.ContainsKey(sps.SeqParameterSetId))
                {
                    Sps.Add(sps.SeqParameterSetId, sps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt SPS: {ToHexString(H264SpsNalUnit.Build(sps))}");
            }
            else if (header.NalUnitType == H264NalUnitTypes.PPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed PPS: {ToHexString(sample)}");
                var pps = H264PpsNalUnit.Parse(sample);
                if (!Pps.ContainsKey(pps.PicParameterSetId))
                {
                    Pps.Add(pps.PicParameterSetId, pps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt PPS: {ToHexString(H264PpsNalUnit.Build(pps))}");
            }
            else
            {
                var sliceHeader = ReadSliceHeader(sample);

                _nalBuffer.Add(sample);

                if (IsNewSample(_lastSliceHeader, sliceHeader))
                {
                    int len = sample.Length;
                    IEnumerable<byte> result = new byte[0];

                    foreach (var nal in _nalBuffer)
                    {
                        // for each NAL, add 4 byte NAL size
                        byte[] size = new byte[] {
                            (byte)((len & 0xff000000) >> 24),
                            (byte)((len & 0xff0000) >> 16),
                            (byte)((len & 0xff00) >> 8),
                            (byte)(len & 0xff)
                        };
                        result = result.Concat(size).Concat(sample);
                    }
                   
                    await base.ProcessSampleAsync(result.ToArray(), duration);
                    _nalBuffer.Clear();
                }

                _lastSliceHeader = sliceHeader;
            }
        }

        public static bool IsNewSample(H264NalSliceHeader oldHeader, H264NalSliceHeader newHeader)
        {
            if (oldHeader == null)
            {
                return true;
            }

            if (newHeader.FrameNum != oldHeader.FrameNum)
            {
                return true;
            }
            if (newHeader.PicParameterSetId != oldHeader.PicParameterSetId)
            {
                return true;
            }
            if (newHeader.FieldPicFlag != oldHeader.FieldPicFlag)
            {
                return true;
            }
            if (newHeader.FieldPicFlag != 0)
            {
                if (newHeader.BottomFieldFlag != oldHeader.BottomFieldFlag)
                {
                    return true;
                }
            }
            if (newHeader.NalRefIdc != oldHeader.NalRefIdc)
            {
                return true;
            }
            if (newHeader.PicOrderCntType == 0 && oldHeader.PicOrderCntType == 0)
            {
                if (newHeader.PicOrderCntLsb != oldHeader.PicOrderCntLsb)
                {
                    return true;
                }
                if (newHeader.DeltaPicOrderCntBottom != oldHeader.DeltaPicOrderCntBottom)
                {
                    return true;
                }
            }
            if (newHeader.PicOrderCntType == 1 && oldHeader.PicOrderCntType == 1)
            {
                if (newHeader.DeltaPicOrderCnt0 != oldHeader.DeltaPicOrderCnt0)
                {
                    return true;
                }
                if (newHeader.DeltaPicOrderCnt1 != oldHeader.DeltaPicOrderCnt1)
                {
                    return true;
                }
            }
            if (newHeader.IdrPicFlag != oldHeader.IdrPicFlag)
            {
                return true;
            }
            if (newHeader.IdrPicFlag && oldHeader.IdrPicFlag)
            {
                if (newHeader.IdrPicId != oldHeader.IdrPicId)
                {
                    return true;
                }
            }
            return false;
        }

        public enum H264NalSliceType
        {
            P, B, I, SP, SI
        }

        public class H264NalSliceHeader
        {
            public H264NalSliceHeader(
                int firstMbInSlice,
                int sliceTypeInt,
                H264NalSliceType sliceType,
                int picParameterSetId, 
                int seqParameterSetId,
                int colorPlaneId,
                int frameNum,
                int fieldPicFlag,
                int bottomFieldFlag,
                int idrPicId, 
                int picOrderCntLsb, 
                int deltaPicOrderCntBottom, 
                int deltaPicOrderCnt0,
                int deltaPicOrderCnt1,
                bool idrPicFlag, 
                int nalRefIdc, 
                int picOrderCntType)
            {
                FirstMbInSlice = firstMbInSlice;
                SliceTypeInt = sliceTypeInt;
                SliceType = sliceType;
                PicParameterSetId = picParameterSetId;
                SeqParameterSetId = seqParameterSetId;
                ColorPlaneId = colorPlaneId;
                FrameNum = frameNum;
                FieldPicFlag = fieldPicFlag;
                BottomFieldFlag = bottomFieldFlag;
                IdrPicId = idrPicId;
                PicOrderCntLsb = picOrderCntLsb;
                DeltaPicOrderCntBottom = deltaPicOrderCntBottom;
                DeltaPicOrderCnt0 = deltaPicOrderCnt0;
                DeltaPicOrderCnt1 = deltaPicOrderCnt1;
                IdrPicFlag = idrPicFlag;
                NalRefIdc = nalRefIdc;
                PicOrderCntType = picOrderCntType;
            }

            public int FirstMbInSlice { get; set; }
            public int SliceTypeInt { get; set; }
            public H264NalSliceType SliceType { get; set; }
            public int PicParameterSetId { get; set; }
            public int SeqParameterSetId { get; set; }
            public int ColorPlaneId { get; set; }
            public int FrameNum { get; set; }
            public int FieldPicFlag { get; set; }
            public int BottomFieldFlag { get; set; }
            public int IdrPicId { get; set; }
            public int PicOrderCntLsb { get; set; }
            public int DeltaPicOrderCntBottom { get; set; }
            public int DeltaPicOrderCnt0 { get; set; }
            public int DeltaPicOrderCnt1 { get; set; }
            public bool IdrPicFlag { get; set; }
            public int NalRefIdc { get; set; }
            public int PicOrderCntType { get; set; }
        }

        private H264NalSliceHeader ReadSliceHeader(byte[] sample)
        {
            using (BitStreamReader reader = new BitStreamReader(sample))
            {
                byte type = (byte)reader.ReadBits(8); // first byte = NAL header
                H264NalUnitHeader header = H264NalUnitHeader.ParseNALHeader(type);
                bool idrPicFlag = header.NalUnitType == H264NalUnitTypes.SLICE_IDR;

                int firstMbInSlice = reader.ReadUE();
                int sliceTypeInt = reader.ReadUE();

                H264NalSliceType sliceType;
                switch (sliceTypeInt)
                {
                    case 0:
                    case 5:
                        sliceType = H264NalSliceType.P;
                        break;

                    case 1:
                    case 6:
                        sliceType = H264NalSliceType.B;
                        break;

                    case 2:
                    case 7:
                        sliceType = H264NalSliceType.I;
                        break;

                    case 3:
                    case 8:
                        sliceType = H264NalSliceType.SP;
                        break;

                    case 4:
                    case 9:
                        sliceType = H264NalSliceType.SI;
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported slice type {sliceTypeInt}");
                }

                int picParameterSetId = reader.ReadUE();
                if (!Pps.ContainsKey(picParameterSetId))
                    throw new Exception($"Missing PPS with ID {picParameterSetId}!");

                var pps = Pps[picParameterSetId];
                int seqParameterSetId = pps.SeqParameterSetId;
                if (!Sps.ContainsKey(seqParameterSetId))
                    throw new Exception($"Missing SPS with ID {seqParameterSetId}!");

                var sps = Sps[seqParameterSetId];
                int colorPlaneId = 0;
                if (sps.ResidualColorTransformFlag != 0)
                {
                    colorPlaneId = reader.ReadBits(2);
                }

                int frameNum = reader.ReadBits(sps.Log2MaxFrameNumMinus4 + 4);
                int fieldPicFlag = 0;
                int bottomFieldFlag = 0;
                if (sps.FrameMbsOnlyFlag == 0)
                {
                    fieldPicFlag = reader.ReadBit();
                    if (fieldPicFlag != 0)
                    {
                        bottomFieldFlag = reader.ReadBit();
                    }
                }

                int idrPicId = 0;
                if (idrPicFlag)
                {
                    idrPicId = reader.ReadUE();
                }

                int picOrderCntLsb = 0;
                int deltaPicOrderCntBottom = 0;
                if (sps.PicOrderCntType == 0)
                {
                    picOrderCntLsb = reader.ReadBits(sps.Log2MaxPicOrderCntLsbMinus4 + 4);
                    if (pps.BottomFieldPicOrderInFramePresentFlag != 0 && fieldPicFlag == 0)
                    {
                        deltaPicOrderCntBottom = reader.ReadSE();
                    }
                }

                int deltaPicOrderCnt0 = 0;
                int deltaPicOrderCnt1 = 0;
                if (sps.PicOrderCntType == 1 && sps.DeltaPicOrderAlwaysZeroFlag == 0)
                {
                    deltaPicOrderCnt0 = reader.ReadSE();
                    if (pps.BottomFieldPicOrderInFramePresentFlag != 0 && fieldPicFlag == 0)
                    {
                        deltaPicOrderCnt1 = reader.ReadSE();
                    }
                }

                return new H264NalSliceHeader(
                    firstMbInSlice,
                    sliceTypeInt,
                    sliceType,
                    picParameterSetId,
                    seqParameterSetId,
                    colorPlaneId,
                    frameNum,
                    fieldPicFlag,
                    bottomFieldFlag,
                    idrPicId,
                    picOrderCntLsb,
                    deltaPicOrderCntBottom,
                    deltaPicOrderCnt0,
                    deltaPicOrderCnt1,
                    idrPicFlag,
                    header.NalRefIdc, // we add a few more pieces of info to the slice header
                    sps.PicOrderCntType);
            }
        }
    }

    public static class H264NalUnitTypes
    {
        // TODO
        public const int SLICE_IDR = 5;
        public const int SPS = 7;
        public const int PPS = 8;
    }

    public class AvcConfigurationBox : Mp4Box
    {
        public const string TYPE = "avcC";

        public AvcDecoderConfigurationRecord AvcDecoderConfigurationRecord { get; }

        public AvcConfigurationBox(uint size, Mp4Box parent, AvcDecoderConfigurationRecord record) : base(size, TYPE, parent)
        {
            AvcDecoderConfigurationRecord = record;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            AvcConfigurationBox avcc = new AvcConfigurationBox(
                size,
                parent,
                await AvcDecoderConfigurationRecord.Parse(size - 8, stream));

            return avcc;
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            AvcConfigurationBox b = (AvcConfigurationBox)box;
            return AvcDecoderConfigurationRecord.Build(b.AvcDecoderConfigurationRecord, stream);
        }

        public override uint CalculateSize()
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
        public int LengthSizeMinusOne { get; set; } = 4;
        public int NumberOfSequenceParameterSetsPaddingBits { get; set; } = 7;
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

        public static async Task<AvcDecoderConfigurationRecord> Parse(uint size, Stream stream)
        {
            byte configurationVersion = IsoReaderWriter.ReadByte(stream);
            byte avcProfileIndication = IsoReaderWriter.ReadByte(stream);
            byte profileCompatibility = IsoReaderWriter.ReadByte(stream);
            byte avcLevelIndication = IsoReaderWriter.ReadByte(stream);

            BitStreamReader bitstream = new BitStreamReader(stream);
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
                using (MemoryStream ms = new MemoryStream(sequenceParameterSetNALUnitBytes))
                {
                    H264SpsNalUnit sequenceParameterSetNALUnit = H264SpsNalUnit.Parse(sequenceParameterSetLength, ms);
                    sequenceParameterSets.Add(sequenceParameterSetNALUnit);
                }

                consumedLength = consumedLength + 2 + sequenceParameterSetLength;
            }
            byte numberOfPictureParameterSets = IsoReaderWriter.ReadByte(stream);
            for (int i = 0; i < numberOfPictureParameterSets; i++)
            {
                ushort pictureParameterSetLength = IsoReaderWriter.ReadUInt16(stream);
                byte[] pictureParameterSetNALUnitBytes = new byte[pictureParameterSetLength];
                await IsoReaderWriter.ReadBytesAsync(stream, pictureParameterSetNALUnitBytes, 0, pictureParameterSetLength);
                using (MemoryStream ms = new MemoryStream(pictureParameterSetNALUnitBytes))
                {
                    H264PpsNalUnit pictureParameterSetNALUnit = H264PpsNalUnit.Parse(pictureParameterSetLength, ms);
                    pictureParameterSets.Add(pictureParameterSetNALUnit);
                }
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

        public static async Task<uint> Build(AvcDecoderConfigurationRecord b, Stream stream)
        {
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.ConfigurationVersion);
            size += IsoReaderWriter.WriteByte(stream, b.AvcProfileIndication);
            size += IsoReaderWriter.WriteByte(stream, b.ProfileCompatibility);
            size += IsoReaderWriter.WriteByte(stream, b.AvcLevelIndication);

            BitStreamWriter bitstream = new BitStreamWriter(stream);
            bitstream.WriteBits(6, b.LengthSizeMinusOnePaddingBits);
            bitstream.WriteBits(2, b.LengthSizeMinusOne);
            bitstream.WriteBits(3, b.NumberOfSequenceParameterSetsPaddingBits);
            bitstream.WriteBits(5, b.SequenceParameterSets.Count);
            size += bitstream.WrittenBytes;

            foreach (H264SpsNalUnit sequenceParameterSetNALUnit in b.SequenceParameterSets)
            {
                uint nalSize = sequenceParameterSetNALUnit.CalculateSize();
                size += IsoReaderWriter.WriteUInt16(stream, (ushort)nalSize);
                size += H264SpsNalUnit.Build(stream, sequenceParameterSetNALUnit);
            }

            size += IsoReaderWriter.WriteByte(stream, (byte)b.PictureParameterSets.Count);

            foreach (H264PpsNalUnit pictureParameterSetNALUnit in b.PictureParameterSets)
            {
                uint nalSize = pictureParameterSetNALUnit.CalculateSize();
                size += IsoReaderWriter.WriteUInt16(stream, (ushort)nalSize);
                size += H264PpsNalUnit.Build(stream, pictureParameterSetNALUnit);
            }

            if (b.SequenceParameterSetExts.Count > 0 && (b.AvcProfileIndication == 100 || b.AvcProfileIndication == 110 || b.AvcProfileIndication == 122 || b.AvcProfileIndication == 144))
            {
                bitstream = new BitStreamWriter(stream);
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
            int entropyCodingModeFlag,
            int bottomFieldPicOrderInFramePresentFlag,
            int numSliceGroupsMinus1,
            int sliceGroupMapType,
            int[] topLeft,
            int[] bottomRight,
            int[] runLengthMinus1,
            int sliceGroupChangeDirectionFlag,
            int sliceGroupChangeRateMinus1,
            int numberBitsPerSliceGroupId,
            int picSizeInMapUnitsMinus1,
            int[] sliceGroupId,
            int numRefIdxL0ActiveMinus1,
            int numRefIdxL1ActiveMinus1,
            int weightedPredFlag,
            int weightedBipredIdc,
            int picInitQpMinus26,
            int picInitQsMinus26,
            int chromaQpIndexOffset,
            int deblockingFilterControlPresentFlag,
            int constrainedIntraPredFlag,
            int redundantPicCntPresentFlag,
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
        public int EntropyCodingModeFlag { get; set; }
        public int BottomFieldPicOrderInFramePresentFlag { get; set; }
        public int NumSliceGroupsMinus1 { get; set; }
        public int SliceGroupMapType { get; set; }
        public int[] TopLeft { get; set; }
        public int[] BottomRight { get; set; }
        public int[] RunLengthMinus1 { get; set; }
        public int SliceGroupChangeDirectionFlag { get; set; }
        public int SliceGroupChangeRateMinus1 { get; set; }
        public int NumberBitsPerSliceGroupId { get; set; }
        public int PicSizeInMapUnitsMinus1 { get; set; }
        public int[] SliceGroupId { get; set; }
        public int NumRefIdxL0ActiveMinus1 { get; set; }
        public int NumRefIdxL1ActiveMinus1 { get; set; }
        public int WeightedPredFlag { get; set; }
        public int WeightedBipredIdc { get; set; }
        public int PicInitQpMinus26 { get; set; }
        public int PicInitQsMinus26 { get; set; }
        public int ChromaQpIndexOffset { get; set; }
        public int DeblockingFilterControlPresentFlag { get; set; }
        public int ConstrainedIntraPredFlag { get; set; }
        public int RedundantPicCntPresentFlag { get; set; }
        public H264PpsExt Extended { get; set; }

        public static H264PpsNalUnit Parse(byte[] pps)
        {
            using (MemoryStream ms = new MemoryStream(pps))
            {
                return Parse((ushort)pps.Length, ms);
            }
        }

        public static H264PpsNalUnit Parse(ushort size, Stream stream)
        {
            BitStreamReader bitstream = new BitStreamReader(stream);
            int type = bitstream.ReadBits(8);
            H264NalUnitHeader header = H264NalUnitHeader.ParseNALHeader((byte)type);

            int picParameterSetId = bitstream.ReadUE();
            int seqParameterSetId = bitstream.ReadUE();
            int entropyCodingModeFlag = bitstream.ReadBit();
            int bottomFieldPicOrderInFramePresentFlag = bitstream.ReadBit();
            int numSliceGroupsMinus1 = bitstream.ReadUE();

            int sliceGroupMapType = 0;
            int[] topLeft = null;
            int[] bottomRight = null;
            int[] runLengthMinus1 = null;
            int sliceGroupChangeDirectionFlag = 0;
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
                    sliceGroupChangeDirectionFlag = bitstream.ReadBit();
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
            int weightedPredFlag = bitstream.ReadBit();
            int weightedBipredIdc = bitstream.ReadBits(2);
            int picInitQpMinus26 = bitstream.ReadSE();
            int picInitQsMinus26 = bitstream.ReadSE();
            int chromaQpIndexOffset = bitstream.ReadSE();
            int deblockingFilterControlPresentFlag = bitstream.ReadBit();
            int constrainedIntraPredFlag = bitstream.ReadBit();
            int redundantPicCntPresentFlag = bitstream.ReadBit();

            H264PpsExt extended = null;

            if (bitstream.HasMoreRBSPData((uint)size))
            {
                extended = H264PpsExt.Parse(bitstream);
            }

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

        public static uint Build(Stream stream, H264PpsNalUnit nal)
        {
            byte type = H264NalUnitHeader.BuildNALHeader(nal.NalHeader);

            BitStreamWriter bitstream = new BitStreamWriter(stream);
            bitstream.WriteBits(8, type);
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

            bitstream.WriteTrailingBit();
            bitstream.Flush();

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

        public H264PpsExt(int transform8x8ModeFlag, int picScalingMatrixPresentFlag, ScalingMatrix scalingMatrix, int secondChromaQpIndexOffset)
        {
            Transform8x8ModeFlag = transform8x8ModeFlag;
            PicScalingMatrixPresentFlag = picScalingMatrixPresentFlag;
            ScalingMatrix = scalingMatrix;
            SecondChromaQpIndexOffset = secondChromaQpIndexOffset;
        }

        public int Transform8x8ModeFlag { get; set; }
        public int PicScalingMatrixPresentFlag { get; set; }
        public ScalingMatrix ScalingMatrix { get; set; }
        public int PicScalingListPresentFlag { get; set; }
        public int SecondChromaQpIndexOffset { get; set; }

        public static H264PpsExt Parse(BitStreamReader bitstream)
        {
            int transform8x8ModeFlag = bitstream.ReadBit();
            int picScalingMatrixPresentFlag = bitstream.ReadBit();
            ScalingMatrix scalingMatrix = null;
            if (picScalingMatrixPresentFlag != 0)
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

        public static void Build(BitStreamWriter bitstream, H264PpsExt ext)
        {
            bitstream.WriteBit(ext.Transform8x8ModeFlag);
            bitstream.WriteBit(ext.PicScalingMatrixPresentFlag);
            if (ext.PicScalingMatrixPresentFlag != 0)
            {
                ScalingMatrix.Build(bitstream, ext.ScalingMatrix);
            }

            bitstream.WriteSE(ext.SecondChromaQpIndexOffset);
        }
    }

    public class ChromaFormat
    {
        public static readonly ChromaFormat MONOCHROME = new ChromaFormat(0, 0, 0);
        public static readonly ChromaFormat YUV_420 = new ChromaFormat(1, 2, 2);
        public static readonly ChromaFormat YUV_422 = new ChromaFormat(2, 2, 1);
        public static readonly ChromaFormat YUV_444 = new ChromaFormat(3, 1, 1);

        public int Id { get; }
        public int SubWidth { get; }
        public int SubHeight { get; }

        public ChromaFormat(int id, int subWidth, int subHeight)
        {
            this.Id = id;
            this.SubWidth = subWidth;
            this.SubHeight = subHeight;
        }

        public static ChromaFormat FromId(int id)
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
            ChromaFormat chromaFormat,
            int residualColorTransformFlag,
            int bitDepthLumaMinus8,
            int bitDepthChromaMinus8,
            int qpprimeYZeroTransformBypassFlag,
            int seqScalingMatrixPresent,
            ScalingMatrix scalingMatrix,
            int log2MaxFrameNumMinus4,
            int picOrderCntType,
            int log2MaxPicOrderCntLsbMinus4,
            int deltaPicOrderAlwaysZeroFlag,
            int offsetForNonRefPic,
            int offsetForTopToBottomField,
            int numRefFramesInPicOrderCntCycle,
            int[] offsetForRefFrame,
            int numRefFrames,
            int gapsInFrameNumValueAllowedFlag,
            int picWidthInMbsMinus1,
            int picHeightInMapUnitsMinus1,
            int frameMbsOnlyFlag,
            int mbAdaptiveFrameFieldFlag,
            int direct8x8InferenceFlag,
            int frameCroppingFlag,
            int frameCropLeftOffset,
            int frameCropRightOffset,
            int frameCropTopOffset,
            int frameCropBottomOffset,
            int vuiParametersPresentFlag,
            VuiParameters vuiParams)
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
            VuiParams = vuiParams;
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
        public ChromaFormat ChromaFormat { get; set; }
        public int ResidualColorTransformFlag { get; set; }
        public int BitDepthLumaMinus8 { get; set; }
        public int BitDepthChromaMinus8 { get; set; }
        public int QpprimeYZeroTransformBypassFlag { get; set; }
        public int SeqScalingMatrixPresent { get; set; }
        public ScalingMatrix ScalingMatrix { get; set; }
        public int Log2MaxFrameNumMinus4 { get; set; }
        public int PicOrderCntType { get; set; }
        public int Log2MaxPicOrderCntLsbMinus4 { get; set; }
        public int DeltaPicOrderAlwaysZeroFlag { get; set; }
        public int OffsetForNonRefPic { get; set; }
        public int OffsetForTopToBottomField { get; set; }
        public int NumRefFramesInPicOrderCntCycle { get; set; }
        public int[] OffsetForRefFrame { get; set; }
        public int NumRefFrames { get; set; }
        public int GapsInFrameNumValueAllowedFlag { get; set; }
        public int PicWidthInMbsMinus1 { get; set; }
        public int PicHeightInMapUnitsMinus1 { get; set; }
        public int FrameMbsOnlyFlag { get; set; }
        public int MbAdaptiveFrameFieldFlag { get; set; }
        public int Direct8x8InferenceFlag { get; set; }
        public int FrameCroppingFlag { get; set; }
        public int FrameCropLeftOffset { get; set; }
        public int FrameCropRightOffset { get; set; }
        public int FrameCropTopOffset { get; set; }
        public int FrameCropBottomOffset { get; set; }
        public int VuiParametersPresentFlag { get; set; }
        public VuiParameters VuiParams { get; set; }

        public static H264SpsNalUnit Parse(byte[] sps)
        {
            using (MemoryStream ms = new MemoryStream(sps))
            {
                return Parse((ushort)sps.Length, ms);
            }
        }

        public static H264SpsNalUnit Parse(ushort size, Stream stream)
        {
            BitStreamReader bitstream = new BitStreamReader(stream);
            
            int type = bitstream.ReadBits(8);
            H264NalUnitHeader header = H264NalUnitHeader.ParseNALHeader((byte)type);
            
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

            ChromaFormat chromaFormat;
            int residualColorTransformFlag = 0;
            int bitDepthLumaMinus8 = 0;
            int bitDepthChromaMinus8 = 0;
            int qpprimeYZeroTransformBypassFlag = 0;
            int seqScalingMatrixPresent = 0;
            ScalingMatrix scalingMatrix = null;
            if (profileIdc == 100 || profileIdc == 110 || profileIdc == 122 || profileIdc == 144)
            {
                int chromaFormatIdc = bitstream.ReadUE();
                chromaFormat = ChromaFormat.FromId(chromaFormatIdc);
                if (chromaFormat.Id == 3)
                {
                    residualColorTransformFlag = bitstream.ReadBit();
                }

                bitDepthLumaMinus8 = bitstream.ReadUE();
                bitDepthChromaMinus8 = bitstream.ReadUE();
                qpprimeYZeroTransformBypassFlag = bitstream.ReadBit();
                seqScalingMatrixPresent = bitstream.ReadBit();
                if (seqScalingMatrixPresent != 0)
                {
                    scalingMatrix = ScalingMatrix.Parse(bitstream);
                }
            }
            else
            {
                chromaFormat = ChromaFormat.FromId(1);
            }

            int log2MaxFrameNumMinus4 = bitstream.ReadUE();
            int picOrderCntType = bitstream.ReadUE();
            int log2MaxPicOrderCntLsbMinus4 = 0;
            int deltaPicOrderAlwaysZeroFlag = 0;
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
                deltaPicOrderAlwaysZeroFlag = bitstream.ReadBit();
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
            int gapsInFrameNumValueAllowedFlag = bitstream.ReadBit();
            int picWidthInMbsMinus1 = bitstream.ReadUE();
            int picHeightInMapUnitsMinus1 = bitstream.ReadUE();
            int frameMbsOnlyFlag = bitstream.ReadBit();
            int mbAdaptiveFrameFieldFlag = 0;

            if (frameMbsOnlyFlag == 0)
            {
                mbAdaptiveFrameFieldFlag = bitstream.ReadBit();
            }

            int direct8x8InferenceFlag = bitstream.ReadBit();
            int frameCroppingFlag = bitstream.ReadBit();
            int frameCropLeftOffset = 0;
            int frameCropRightOffset = 0;
            int frameCropTopOffset = 0;
            int frameCropBottomOffset = 0;

            if (frameCroppingFlag != 0)
            {
                frameCropLeftOffset = bitstream.ReadUE();
                frameCropRightOffset = bitstream.ReadUE();
                frameCropTopOffset = bitstream.ReadUE();
                frameCropBottomOffset = bitstream.ReadUE();
            }

            int vuiParametersPresentFlag = bitstream.ReadBit();
            VuiParameters vuiParams = null;

            if (vuiParametersPresentFlag != 0)
            {
                vuiParams = VuiParameters.Parse(bitstream);
            }

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

        public static uint Build(Stream stream, H264SpsNalUnit nal)
        {
            byte type = H264NalUnitHeader.BuildNALHeader(nal.NalHeader);
            BitStreamWriter bitstream = new BitStreamWriter(stream);
            bitstream.WriteBits(8, type);
            
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
                bitstream.WriteUE((uint)nal.ChromaFormat.Id);
                if (nal.ChromaFormat.Id == 3)
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

            if (nal.FrameMbsOnlyFlag == 0)
            {
                bitstream.WriteBit(nal.MbAdaptiveFrameFieldFlag);
            }

            bitstream.WriteBit(nal.Direct8x8InferenceFlag);
            bitstream.WriteBit(nal.FrameCroppingFlag);

            if (nal.FrameCroppingFlag != 0)
            {
                bitstream.WriteUE((uint)nal.FrameCropLeftOffset);
                bitstream.WriteUE((uint)nal.FrameCropRightOffset);
                bitstream.WriteUE((uint)nal.FrameCropTopOffset);
                bitstream.WriteUE((uint)nal.FrameCropBottomOffset);
            }

            bitstream.WriteBit(nal.VuiParametersPresentFlag);

            if (nal.VuiParametersPresentFlag != 0)
            {
                VuiParameters.Build(bitstream, nal.VuiParams);
            }

            bitstream.WriteTrailingBit();
            bitstream.Flush();

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
            if (this.FrameMbsOnlyFlag != 0)
            {
                mult = 1;
            }
            int height = 16 * (this.PicHeightInMapUnitsMinus1 + 1) * mult;
            if (this.FrameCroppingFlag != 0)
            {
                int chromaArrayType = 0;
                if (this.ResidualColorTransformFlag == 0)
                {
                    chromaArrayType = this.ChromaFormat.Id;
                }
                int cropUnitX = 1;
                int cropUnitY = mult;
                if (chromaArrayType != 0)
                {
                    cropUnitX = this.ChromaFormat.SubWidth;
                    cropUnitY = this.ChromaFormat.SubHeight * mult;
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
            var vui = this.VuiParams;
            if (vui != null)
            {
                timescale = vui.TimeScale >> 1; // Not sure why, but I found this in several places, and it works...
                frametick = vui.NumUnitsInTick;
                if (timescale == 0 || frametick == 0)
                {
                    if (Log.WarnEnabled) Log.Warn($"Invalid values in vui: timescale: {timescale} and frametick: {frametick}. Using default 25 fps.");
                    timescale = 0;
                    frametick = 0;
                }
                if (frametick > 0)
                {
                    if (timescale / frametick > 100)
                    {
                        if (Log.WarnEnabled) Log.Warn($"Framerate is {(timescale / frametick)}. Might not be correct.");
                    }
                }
                else
                {
                    if (Log.WarnEnabled) Log.Warn($"Frametick is {frametick}. Might not be correct.");
                }
            }
            else
            {
                if (Log.ErrorEnabled) Log.Error("Can't determine frame rate because SPS does not contain vuiParams");
                timescale = 0;
                frametick = 0;
            }

            return (timescale, frametick);
        }
    }

    public class H264NalUnitHeader
    {
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

        public static H264NalUnitHeader ParseNALHeader(byte type)
        {
            if ((type & 0b10000000) != 0)
                throw new Exception("Invalid NAL header!");

            H264NalUnitHeader nalUnitHeader = new H264NalUnitHeader();
            nalUnitHeader.NalRefIdc = type >> 5 & 3;
            nalUnitHeader.NalUnitType = type & 0x1f;
            return nalUnitHeader;
        }

        public static byte BuildNALHeader(H264NalUnitHeader nalUnitHeader)
        {
            byte type = (byte)(((nalUnitHeader.NalRefIdc & 3) << 5) | (nalUnitHeader.NalUnitType & 0x1f));
            return type;
        }
    }

    public static class H264BoxBuilder
    {
        public static VisualSampleEntryBox CreateVisualSampleEntryBox(Mp4Box parent, H264Track track)
        {
            var sps = track.Sps.First().Value; // TODO: not sure about multiple SPS values...
            var dim = sps.CalculateDimensions();

            VisualSampleEntryBox visualSampleEntry = new VisualSampleEntryBox(0, parent, "avc1");
            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            visualSampleEntry.HorizontalResolution = 72;
            visualSampleEntry.VerticalResolution = 72;
            visualSampleEntry.Width = dim.Width;
            visualSampleEntry.Height = dim.Height;
            visualSampleEntry.CompressorName = "h264\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
            visualSampleEntry.CompressorNameDisplayableData = 4;

            AvcConfigurationBox avcConfigurationBox = new AvcConfigurationBox(0, visualSampleEntry,
                new AvcDecoderConfigurationRecord()
                {
                    SequenceParameterSets = track.Sps.Values.ToList(),
                    PictureParameterSets = track.Pps.Values.ToList(),
                    NumberOfPictureParameterSets = (byte)track.Pps.Values.Count
                }
            );
            avcConfigurationBox.AvcDecoderConfigurationRecord.SequenceParameterSets = track.Sps.Values.ToList();
            avcConfigurationBox.AvcDecoderConfigurationRecord.NumberOfSeuqenceParameterSets = track.Sps.Count;
            avcConfigurationBox.AvcDecoderConfigurationRecord.PictureParameterSets = track.Pps.Values.ToList();
            avcConfigurationBox.AvcDecoderConfigurationRecord.NumberOfPictureParameterSets = (byte)track.Pps.Count;
            avcConfigurationBox.AvcDecoderConfigurationRecord.AvcLevelIndication = (byte)sps.LevelIdc;
            avcConfigurationBox.AvcDecoderConfigurationRecord.AvcProfileIndication = (byte)sps.ProfileIdc;
            //avcConfigurationBox.AvcDecoderConfigurationRecord.BitDepthLumaMinus8 = sps.BitDepthLumaMinus8;
            //avcConfigurationBox.AvcDecoderConfigurationRecord.BitDepthChromaMinus8 = sps.BitDepthChromaMinus8;
            //avcConfigurationBox.AvcDecoderConfigurationRecord.ChromaFormat = sps.ChromaFormat.Id;
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
    }
}
