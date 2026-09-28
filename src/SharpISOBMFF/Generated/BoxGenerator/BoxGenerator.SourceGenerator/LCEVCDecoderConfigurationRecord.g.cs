using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// as FFmpeg's ff_isom_write_lvcc writes it: the byte after the size is 0xFF, the NAL unit arrays as hvcC's
aligned(8) class LCEVCDecoderConfigurationRecord {
	unsigned int(8) configurationVersion = 1;
	unsigned int(8) LCEVCProfileIndication;
	unsigned int(8) LCEVCLevelIndication;
	unsigned int(2) chroma_format_idc;
	unsigned int(3) bit_depth_luma_minus8;
	unsigned int(3) bit_depth_chroma_minus8;
	unsigned int(2) lengthSizeMinusOne;
	bit(6) reserved = '111111'b;
	unsigned int(32) pic_width_in_luma_samples;
	unsigned int(32) pic_height_in_luma_samples;
	bit(8) reserved0 = 0xFF;
	unsigned int(8) numOfArrays;
	for (j=0; j < numOfArrays; j++) {
		bit(1) array_completeness;
		bit(1) reserved1 = 0;
		unsigned int(6) NAL_unit_type;
		unsigned int(16) numNalus;
		for (i=0; i< numNalus; i++) {
			unsigned int(16) nalUnitLength;
			bit(8*nalUnitLength) nalUnit;
		}
	}
}
*/
public partial class LCEVCDecoderConfigurationRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "LCEVCDecoderConfigurationRecord"; } }

	protected byte configurationVersion = 1; 
	public byte ConfigurationVersion { get { return this.configurationVersion; } set { this.configurationVersion = value; } }

	protected byte LCEVCProfileIndication; 
	public byte _LCEVCProfileIndication { get { return this.LCEVCProfileIndication; } set { this.LCEVCProfileIndication = value; } }

	protected byte LCEVCLevelIndication; 
	public byte _LCEVCLevelIndication { get { return this.LCEVCLevelIndication; } set { this.LCEVCLevelIndication = value; } }

	protected byte chroma_format_idc; 
	public byte ChromaFormatIdc { get { return this.chroma_format_idc; } set { this.chroma_format_idc = value; } }

	protected byte bit_depth_luma_minus8; 
	public byte BitDepthLumaMinus8 { get { return this.bit_depth_luma_minus8; } set { this.bit_depth_luma_minus8 = value; } }

	protected byte bit_depth_chroma_minus8; 
	public byte BitDepthChromaMinus8 { get { return this.bit_depth_chroma_minus8; } set { this.bit_depth_chroma_minus8 = value; } }

	protected byte lengthSizeMinusOne; 
	public byte LengthSizeMinusOne { get { return this.lengthSizeMinusOne; } set { this.lengthSizeMinusOne = value; } }

	protected byte reserved = 0b111111; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint pic_width_in_luma_samples; 
	public uint PicWidthInLumaSamples { get { return this.pic_width_in_luma_samples; } set { this.pic_width_in_luma_samples = value; } }

	protected uint pic_height_in_luma_samples; 
	public uint PicHeightInLumaSamples { get { return this.pic_height_in_luma_samples; } set { this.pic_height_in_luma_samples = value; } }

	protected byte reserved0 = 0xFF; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte numOfArrays; 
	public byte NumOfArrays { get { return this.numOfArrays; } set { this.numOfArrays = value; } }

	protected bool[] array_completeness; 
	public bool[] ArrayCompleteness { get { return this.array_completeness; } set { this.array_completeness = value; } }

	protected bool[] reserved1; 
	public bool[] Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected byte[] NAL_unit_type; 
	public byte[] NALUnitType { get { return this.NAL_unit_type; } set { this.NAL_unit_type = value; } }

	protected ushort[] numNalus; 
	public ushort[] NumNalus { get { return this.numNalus; } set { this.numNalus = value; } }

	protected ushort[][] nalUnitLength; 
	public ushort[][] NalUnitLength { get { return this.nalUnitLength; } set { this.nalUnitLength = value; } }

	protected byte[][][] nalUnit; 
	public byte[][][] NalUnit { get { return this.nalUnit; } set { this.nalUnit = value; } }

	public LCEVCDecoderConfigurationRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.configurationVersion, "configurationVersion"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.LCEVCProfileIndication, "LCEVCProfileIndication"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.LCEVCLevelIndication, "LCEVCLevelIndication"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.chroma_format_idc, "chroma_format_idc"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bit_depth_luma_minus8, "bit_depth_luma_minus8"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bit_depth_chroma_minus8, "bit_depth_chroma_minus8"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.lengthSizeMinusOne, "lengthSizeMinusOne"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.pic_width_in_luma_samples, "pic_width_in_luma_samples"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.pic_height_in_luma_samples, "pic_height_in_luma_samples"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numOfArrays, "numOfArrays"); 

		this.array_completeness = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( numOfArrays), "array_completeness");
		this.reserved1 = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( numOfArrays), "reserved1");
		this.NAL_unit_type = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( numOfArrays), "NAL_unit_type");
		this.numNalus = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( numOfArrays), "numNalus");
		this.nalUnitLength = stream.SafeAllocate<ushort[]>(boxSize, readSize, IsoStream.GetInt( numOfArrays), "nalUnitLength");
		this.nalUnit = stream.SafeAllocate<byte[][]>(boxSize, readSize, IsoStream.GetInt( numOfArrays), "nalUnit");
		for (int j=0; j < numOfArrays; j++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.array_completeness[j], "array_completeness"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved1[j], "reserved1"); 
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.NAL_unit_type[j], "NAL_unit_type"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.numNalus[j], "numNalus"); 

			this.nalUnitLength[j] = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( numNalus[j]), "nalUnitLength[j]");
			this.nalUnit[j] = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt( numNalus[j]), "nalUnit[j]");
			for (int i=0; i< numNalus[j]; i++)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.nalUnitLength[j][i], "nalUnitLength"); 
				boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*nalUnitLength[j][i] ),  out this.nalUnit[j][i], "nalUnit"); 
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8( this.configurationVersion, "configurationVersion"); 
		boxSize += stream.WriteUInt8( this.LCEVCProfileIndication, "LCEVCProfileIndication"); 
		boxSize += stream.WriteUInt8( this.LCEVCLevelIndication, "LCEVCLevelIndication"); 
		boxSize += stream.WriteBits(2,  this.chroma_format_idc, "chroma_format_idc"); 
		boxSize += stream.WriteBits(3,  this.bit_depth_luma_minus8, "bit_depth_luma_minus8"); 
		boxSize += stream.WriteBits(3,  this.bit_depth_chroma_minus8, "bit_depth_chroma_minus8"); 
		boxSize += stream.WriteBits(2,  this.lengthSizeMinusOne, "lengthSizeMinusOne"); 
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.pic_width_in_luma_samples, "pic_width_in_luma_samples"); 
		boxSize += stream.WriteUInt32( this.pic_height_in_luma_samples, "pic_height_in_luma_samples"); 
		boxSize += stream.WriteUInt8( this.reserved0, "reserved0"); 
		boxSize += stream.WriteUInt8( this.numOfArrays, "numOfArrays"); 

		for (int j=0; j < numOfArrays; j++)
		{
			boxSize += stream.WriteBit( this.array_completeness[j], "array_completeness"); 
			boxSize += stream.WriteBit( this.reserved1[j], "reserved1"); 
			boxSize += stream.WriteBits(6,  this.NAL_unit_type[j], "NAL_unit_type"); 
			boxSize += stream.WriteUInt16( this.numNalus[j], "numNalus"); 

			for (int i=0; i< numNalus[j]; i++)
			{
				boxSize += stream.WriteUInt16( this.nalUnitLength[j][i], "nalUnitLength"); 
				boxSize += stream.WriteBits((uint)(8*nalUnitLength[j][i] ),  this.nalUnit[j][i], "nalUnit"); 
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 8; // configurationVersion
		boxSize += 8; // LCEVCProfileIndication
		boxSize += 8; // LCEVCLevelIndication
		boxSize += 2; // chroma_format_idc
		boxSize += 3; // bit_depth_luma_minus8
		boxSize += 3; // bit_depth_chroma_minus8
		boxSize += 2; // lengthSizeMinusOne
		boxSize += 6; // reserved
		boxSize += 32; // pic_width_in_luma_samples
		boxSize += 32; // pic_height_in_luma_samples
		boxSize += 8; // reserved0
		boxSize += 8; // numOfArrays

		for (int j=0; j < numOfArrays; j++)
		{
			boxSize += 1; // array_completeness
			boxSize += 1; // reserved1
			boxSize += 6; // NAL_unit_type
			boxSize += 16; // numNalus

			for (int i=0; i< numNalus[j]; i++)
			{
				boxSize += 16; // nalUnitLength
				boxSize += (ulong)(8*nalUnitLength[j][i] ); // nalUnit
			}
		}
		return boxSize;
	}
}

}
