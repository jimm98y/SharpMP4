using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMp4
{
    public class H265Track : TrackBase
    {
        public Dictionary<int, H265SpsNalUnit> Sps { get; set; } = new Dictionary<int, H265SpsNalUnit>();
        public Dictionary<int, H265PpsNalUnit> Pps { get; set; } = new Dictionary<int, H265PpsNalUnit>();
        public Dictionary<int, H265VpsNalUnit> Vps { get; set; } = new Dictionary<int, H265VpsNalUnit>();

        public H265Track(uint trackID, uint timescale) : base(trackID)
        {
            base.Handler = "vide";
            this.Timescale = timescale;
        }

        private H265NalSliceHeader _lastSliceHeader;
        private List<byte[]> _nalBuffer = new List<byte[]>();

        public override async Task ProcessSampleAsync(byte[] sample, uint duration)
        {
            var header = H265NalUnitHeader.ParseNALHeader(sample[0]);
            if (header.NalUnitType == H265NalUnitTypes.SPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed SPS: {ToHexString(sample)}");
                var sps = H265SpsNalUnit.Parse(sample);
                if (!Sps.ContainsKey(sps.SeqParameterSetId))
                {
                    Sps.Add(sps.SeqParameterSetId, sps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt SPS: {ToHexString(H265SpsNalUnit.Build(sps))}");
            }
            else if (header.NalUnitType == H265NalUnitTypes.PPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed PPS: {ToHexString(sample)}");
                var pps = H265PpsNalUnit.Parse(sample);
                if (!Pps.ContainsKey(pps.PicParameterSetId))
                {
                    Pps.Add(pps.PicParameterSetId, pps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt PPS: {ToHexString(H265PpsNalUnit.Build(pps))}");
            }
            else if (header.NalUnitType == H265NalUnitTypes.VPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed VPS: {ToHexString(sample)}");
                var vps = H265VpsNalUnit.Parse(sample);
                if (!Vps.ContainsKey(vps.VpsParameterSetId))
                {
                    Vps.Add(vps.VpsParameterSetId, vps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt VPS: {ToHexString(H265VpsNalUnit.Build(vps))}");
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

        public static bool IsNewSample(H265NalSliceHeader oldHeader, H265NalSliceHeader newHeader)
        {
            throw new NotImplementedException();
        }

        private H265NalSliceHeader ReadSliceHeader(byte[] sample)
        {
            throw new NotImplementedException();
        }
    }

    public class H265VpsNalUnit
    {
        public int VpsParameterSetId { get; set; }

        public static byte[] Build(H265VpsNalUnit vps)
        {
            throw new NotImplementedException();
        }

        public static H265VpsNalUnit Parse(byte[] vps)
        {
            using (MemoryStream ms = new MemoryStream(vps))
            {
                return Parse((ushort)vps.Length, ms);
            }
        }

        private static H265VpsNalUnit Parse(ushort length, MemoryStream ms)
        {
            throw new NotImplementedException();
        }
    }

    public class H265SpsNalUnit
    {
        public int SeqParameterSetId { get; set; }

        public static byte[] Build(H265SpsNalUnit sps)
        {
            throw new NotImplementedException();
        }

        public static H265SpsNalUnit Parse(byte[] sps)
        {
            using (MemoryStream ms = new MemoryStream(sps))
            {
                return Parse((ushort)sps.Length, ms);
            }
        }

        private static H265SpsNalUnit Parse(ushort length, MemoryStream ms)
        {
            throw new NotImplementedException();
        }

        public (ushort Width, ushort Height) CalculateDimensions()
        {
            throw new NotImplementedException();
        }
    }

    public class H265PpsNalUnit
    {
        public int PicParameterSetId { get; set; }

        public static byte[] Build(H265PpsNalUnit pps)
        {
            throw new NotImplementedException();
        }

        public static H265PpsNalUnit Parse(byte[] pps)
        {
            using (MemoryStream ms = new MemoryStream(pps))
            {
                return Parse((ushort)pps.Length, ms);
            }
        }

        private static H265PpsNalUnit Parse(ushort length, MemoryStream ms)
        {
            throw new NotImplementedException();
        }
    }

    public class H265NalUnitHeader
    {
        public int NalUnitType { get; set; }

        public static H265NalUnitHeader ParseNALHeader(byte header)
        {
            throw new NotImplementedException();
        }
    }

    public class H265NalUnitTypes
    {
        public const int VPS = 32;
        public const int SPS = 33;
        public const int PPS = 34;
    }

    public class H265NalSliceHeader
    {
    }

    public class HevcConfigurationBox : Mp4Box
    {
        public const string TYPE = "hvcC";

        public HevcDecoderConfigurationRecord HevcDecoderConfigurationRecord { get; }

        public HevcConfigurationBox(uint size, Mp4Box parent, HevcDecoderConfigurationRecord record) : base(size, TYPE, parent)
        {
            HevcDecoderConfigurationRecord = record;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            HevcConfigurationBox hvcc = new HevcConfigurationBox(
                size,
                parent,
                await HevcDecoderConfigurationRecord.Parse(size - 8, stream));

            return hvcc;
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            HevcConfigurationBox b = (HevcConfigurationBox)box;
            return HevcDecoderConfigurationRecord.Build(b.HevcDecoderConfigurationRecord, stream);
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + HevcDecoderConfigurationRecord.CalculateSize();
        }
    }

    public class HevcDecoderConfigurationRecord
    {
        public byte ConfigurationVersion { get; set; }
        public int GeneralProfileSpace { get; set; }
        public int GeneralTierFlag { get; set; }
        public int GeneralProfileIdc { get; set; }
        public uint GeneralProfileCompatibilityFlags { get; set; }
        public ulong GeneralConstraintIndicatorFlags { get; set; }
        public int FrameOnlyConstraintFlag { get; set; }
        public int NonPackedConstraintFlag { get; set; }
        public int InterlacedSourceFlag { get; set; }
        public int ProgressiveSourceFlag { get; set; }
        public byte GeneralLevelIdc { get; set; }
        public int Reserved1 { get; set; }
        public int MinSpatialSegmentationIdc { get; set; }
        public int Reserved2 { get; set; }
        public int ParallelismType { get; set; }
        public int Reserved3 { get; set; }
        public int ChromaFormat { get; set; }
        public int Reserved4 { get; set; }
        public int BitDepthLumaMinus8 { get; set; }
        public int Reserved5 { get; set; }
        public int BitDepthChromaMinus8 { get; set; }
        public ushort AvgFrameRate { get; set; }
        public int ConstantFrameRate { get; set; }
        public int NumTemporalLayers { get; set; }
        public int TemporalIdNested { get; set; }
        public int LengthSizeMinusOne { get; set; }
        public List<HevcNalArray> NalArrays { get; set; } = new List<HevcNalArray>();

        public HevcDecoderConfigurationRecord()
        { }

        public HevcDecoderConfigurationRecord(byte configurationVersion, int generalProfileSpace, int generalTierFlag, int generalProfileIdc, uint generalProfileCompatibilityFlags, ulong generalConstraintIndicatorFlags, int frameOnlyConstraintFlag, int nonPackedConstraintFlag, int interlacedSourceFlag, int progressiveSourceFlag, byte generalLevelIdc, int reserved1, int minSpatialSegmentationIdc, int reserved2, int parallelismType, int reserved3, int chromaFormat, int reserved4, int bitDepthLumaMinus8, int reserved5, int bitDepthChromaMinus8, ushort avgFrameRate, int constantFrameRate, int numTemporalLayers, int temporalIdNested, int lengthSizeMinusOne, List<HevcNalArray> nalArrays)
        {
            ConfigurationVersion = configurationVersion;
            GeneralProfileSpace = generalProfileSpace;
            GeneralTierFlag = generalTierFlag;
            GeneralProfileIdc = generalProfileIdc;
            GeneralProfileCompatibilityFlags = generalProfileCompatibilityFlags;
            GeneralConstraintIndicatorFlags = generalConstraintIndicatorFlags;
            FrameOnlyConstraintFlag = frameOnlyConstraintFlag;
            NonPackedConstraintFlag = nonPackedConstraintFlag;
            InterlacedSourceFlag = interlacedSourceFlag;
            ProgressiveSourceFlag = progressiveSourceFlag;
            GeneralLevelIdc = generalLevelIdc;
            Reserved1 = reserved1;
            MinSpatialSegmentationIdc = minSpatialSegmentationIdc;
            Reserved2 = reserved2;
            ParallelismType = parallelismType;
            Reserved3 = reserved3;
            ChromaFormat = chromaFormat;
            Reserved4 = reserved4;
            BitDepthLumaMinus8 = bitDepthLumaMinus8;
            Reserved5 = reserved5;
            BitDepthChromaMinus8 = bitDepthChromaMinus8;
            AvgFrameRate = avgFrameRate;
            ConstantFrameRate = constantFrameRate;
            NumTemporalLayers = numTemporalLayers;
            TemporalIdNested = temporalIdNested;
            LengthSizeMinusOne = lengthSizeMinusOne;
            NalArrays = nalArrays;
        }

        public static async Task<HevcDecoderConfigurationRecord> Parse(uint size, Stream stream)
        {
            byte configurationVersion = IsoReaderWriter.ReadByte(stream);

            int a = IsoReaderWriter.ReadByte(stream);
            int generalProfileSpace = (a & 0xC0) >> 6;
            int generalTierFlag = (a & 0x20) > 0 ? 1 : 0;
            int generalProfileIdc = a & 0x1F;

            uint generalProfileCompatibilityFlags = IsoReaderWriter.ReadUInt32(stream);
            ulong generalConstraintIndicatorFlags = IsoReaderWriter.ReadUInt48(stream);

            int frameOnlyConstraintFlag = (generalConstraintIndicatorFlags >> 44 & 0x08) > 0 ? 1 : 0;
            int nonPackedConstraintFlag = (generalConstraintIndicatorFlags >> 44 & 0x04) > 0 ? 1 : 0;
            int interlacedSourceFlag = (generalConstraintIndicatorFlags >> 44 & 0x02) > 0 ? 1 : 0;
            int progressiveSourceFlag = (generalConstraintIndicatorFlags >> 44 & 0x01) > 0 ? 1 : 0;

            generalConstraintIndicatorFlags &= 0x7fffffffffffL;

            byte generalLevelIdc = IsoReaderWriter.ReadByte(stream);

            a = IsoReaderWriter.ReadUInt16(stream);
            int reserved1 = (a & 0xF000) >> 12;
            int minSpatialSegmentationIdc = a & 0xFFF;

            a = IsoReaderWriter.ReadByte(stream);
            int reserved2 = (a & 0xFC) >> 2;
            int parallelismType = a & 0x3;

            a = IsoReaderWriter.ReadByte(stream);
            int reserved3 = (a & 0xFC) >> 2;
            int chromaFormat = a & 0x3;

            a = IsoReaderWriter.ReadByte(stream);
            int reserved4 = (a & 0xF8) >> 3;
            int bitDepthLumaMinus8 = a & 0x7;

            a = IsoReaderWriter.ReadByte(stream);
            int reserved5 = (a & 0xF8) >> 3;
            int bitDepthChromaMinus8 = a & 0x7;

            ushort avgFrameRate = IsoReaderWriter.ReadUInt16(stream);

            a = IsoReaderWriter.ReadByte(stream);
            int constantFrameRate = (a & 0xC0) >> 6;
            int numTemporalLayers = (a & 0x38) >> 3;
            int temporalIdNested = (a & 0x4) > 0 ? 1 : 0;
            int lengthSizeMinusOne = a & 0x3;

            int numOfArrays = IsoReaderWriter.ReadByte(stream);
            List<HevcNalArray> nalArrays = new List<HevcNalArray>();
            for (int i = 0; i < numOfArrays; i++)
            {
                HevcNalArray array = new HevcNalArray();

                a = IsoReaderWriter.ReadByte(stream);
                array.ArrayCompleteness = (a & 0x80) > 0 ? 1 : 0;
                array.Reserved = (a & 0x40) > 0 ? 1 : 0;
                array.NalUnitType = a & 0x3F;

                int numNalus = IsoReaderWriter.ReadUInt16(stream);
                for (int j = 0; j < numNalus; j++)
                {
                    int nalUnitLength = IsoReaderWriter.ReadUInt16(stream);
                    byte[] nal = new byte[nalUnitLength];
                    await IsoReaderWriter.ReadBytesAsync(stream, nal, 0, nal.Length);
                    array.NalUnits.Add(nal);
                }
                nalArrays.Add(array);
            }

            return new HevcDecoderConfigurationRecord(
                configurationVersion,
                generalProfileSpace,
                generalTierFlag,
                generalProfileIdc,
                generalProfileCompatibilityFlags,
                generalConstraintIndicatorFlags,
                frameOnlyConstraintFlag,
                nonPackedConstraintFlag,
                interlacedSourceFlag,
                progressiveSourceFlag,
                generalLevelIdc,
                reserved1,
                minSpatialSegmentationIdc,
                reserved2,
                parallelismType,
                reserved3,
                chromaFormat,
                reserved4,
                bitDepthLumaMinus8,
                reserved5,
                bitDepthChromaMinus8,
                avgFrameRate,
                constantFrameRate,
                numTemporalLayers,
                temporalIdNested,
                lengthSizeMinusOne,
                nalArrays
            );
        }

        public uint CalculateSize()
        {
            uint size = 23;
            foreach (HevcNalArray array in NalArrays)
            {
                size += 3;
                foreach (byte[] nalUnit in array.NalUnits)
                {
                    size += 2;
                    size += (uint)nalUnit.Length;
                }
            }
            return size;
        }

        public static async Task<uint> Build(HevcDecoderConfigurationRecord b, Stream stream)
        {
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.ConfigurationVersion);
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.GeneralProfileSpace << 6) + (b.GeneralTierFlag == 1 ? 0x20 : 0) + b.GeneralProfileIdc));
            size += IsoReaderWriter.WriteUInt32(stream, b.GeneralProfileCompatibilityFlags);
            
            ulong generalConstraintIndicatorFlags = b.GeneralConstraintIndicatorFlags;
            if (b.FrameOnlyConstraintFlag != 0)
            {
                generalConstraintIndicatorFlags |= 1L << 47;
            }
            if (b.NonPackedConstraintFlag != 0)
            {
                generalConstraintIndicatorFlags |= 1L << 46;
            }
            if (b.InterlacedSourceFlag != 0)
            {
                generalConstraintIndicatorFlags |= 1L << 45;
            }
            if (b.ProgressiveSourceFlag != 0)
            {
                generalConstraintIndicatorFlags |= 1L << 44;
            }

            size += IsoReaderWriter.WriteUInt48(stream, generalConstraintIndicatorFlags);
            size += IsoReaderWriter.WriteByte(stream, b.GeneralLevelIdc);
            size += IsoReaderWriter.WriteUInt16(stream, (ushort)((b.Reserved1 << 12) + b.MinSpatialSegmentationIdc));
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.Reserved2 << 2) + b.ParallelismType));
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.Reserved3 << 2) + b.ChromaFormat));
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.Reserved4 << 3) + b.BitDepthLumaMinus8));
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.Reserved5 << 3) + b.BitDepthChromaMinus8));
            size += IsoReaderWriter.WriteUInt16(stream, b.AvgFrameRate);
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.ConstantFrameRate << 6) + (b.NumTemporalLayers << 3) + (b.TemporalIdNested != 0 ? 0x4 : 0) + b.LengthSizeMinusOne));
            size += IsoReaderWriter.WriteByte(stream, (byte)b.NalArrays.Count);

            foreach (HevcNalArray nalArray in b.NalArrays)
            {
                size += IsoReaderWriter.WriteByte(stream, (byte)((nalArray.ArrayCompleteness != 0 ? 0x80 : 0) + (nalArray.Reserved != 0 ? 0x40 : 0) + nalArray.NalUnitType));
                size += IsoReaderWriter.WriteUInt16(stream, (ushort)nalArray.NalUnits.Count);
                
                foreach (byte[] nalUnit in nalArray.NalUnits)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, (ushort)nalUnit.Length);
                    size += await IsoReaderWriter.WriteBytesAsync(stream, nalUnit, 0, nalUnit.Length);
                }
            }

            return size;
        }
    }

    public sealed class HevcNalArray
    {
        public int ArrayCompleteness { get; set; }
        public int Reserved { get; set; }
        public int NalUnitType { get; set; }
        public List<byte[]> NalUnits { get; set; } = new List<byte[]>();
    }

    public class H265BoxBuilder
    {
        public static VisualSampleEntryBox CreateVisualSampleEntryBox(Mp4Box parent, H265Track track)
        {
            var sps = track.Sps.First().Value; // TODO: not sure about multiple SPS values...
            var vps = track.Vps.First().Value; // TODO: not sure about multiple VPS values...
            var dim = sps.CalculateDimensions();

            VisualSampleEntryBox visualSampleEntry = new VisualSampleEntryBox(0, parent, "hvc1");
            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            visualSampleEntry.HorizontalResolution = 72;
            visualSampleEntry.VerticalResolution = 72;
            visualSampleEntry.Width = dim.Width;
            visualSampleEntry.Height = dim.Height;
            visualSampleEntry.CompressorName = "hevc\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
            visualSampleEntry.CompressorNameDisplayableData = 4;

            HevcConfigurationBox hevcConfigurationBox = new HevcConfigurationBox(0, visualSampleEntry, new HevcDecoderConfigurationRecord());
            hevcConfigurationBox.HevcDecoderConfigurationRecord.ConfigurationVersion = 1;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralProfileIdc = sps.GeneralProfileIdc;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.ChromaFormat = sps.ChromaFormatIdc;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralLevelIdc = sps.GeneralLevelIdc;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralProfileCompatibilityFlags = sps.GeneralProfileCompatibilityFlags;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralConstraintIndicatorFlags = vps.GeneralProfileConstraintIndicatorFlags;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.BitDepthChromaMinus8 = sps.BitDepthChromaMinus8;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.BitDepthLumaMinus8 = sps.BitDepthLumaMinus8;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.TemporalIdNested = sps.SpsTemporalIdNestingFlag;
            hevcConfigurationBox.HevcDecoderConfigurationRecord.LengthSizeMinusOne = 3; // 4 bytes size block inserted in between NAL units

            visualSampleEntry.Children.Add(hevcConfigurationBox);

            return visualSampleEntry;
        }
    }
}
