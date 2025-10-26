﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class HEVCDecoderConfigurationRecord {
	unsigned int(8) configurationVersion = 1;
	unsigned int(2) general_profile_space;
	unsigned int(1) general_tier_flag;
	unsigned int(5) general_profile_idc;
	unsigned int(32) general_profile_compatibility_flags;
	unsigned int(48) general_constraint_indicator_flags;
	unsigned int(8) general_level_idc;
	bit(4) reserved = '1111'b;
	unsigned int(12) min_spatial_segmentation_idc;
	bit(6) reserved = '111111'b;
	unsigned int(2) parallelismType;
	bit(6) reserved = '111111'b;
	unsigned int(2) chromaFormat;
	bit(5) reserved = '11111'b;
	unsigned int(3) bitDepthLumaMinus8;
	bit(5) reserved = '11111'b;
	unsigned int(3) bitDepthChromaMinus8;
	bit(16) avgFrameRate;
	bit(2) constantFrameRate;
	bit(3) numTemporalLayers;
	bit(1) temporalIdNested;
	unsigned int(2) lengthSizeMinusOne; 
	unsigned int(8) numOfArrays;
	for (j=0; j < numOfArrays; j++) {
		bit(1) array_completeness;
		unsigned int(1) reserved = 0;
		unsigned int(6) NAL_unit_type;
		unsigned int(16) numNalus;
		for (i=0; i< numNalus; i++) {
			unsigned int(16) nalUnitLength;
			bit(8*nalUnitLength) nalUnit;
		}
	}
}
*/
public partial class HEVCDecoderConfigurationRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "HEVCDecoderConfigurationRecord"; } }

	protected byte configurationVersion = 1; 
	public byte ConfigurationVersion { get { return this.configurationVersion; } set { this.configurationVersion = value; } }

	protected byte general_profile_space; 
	public byte GeneralProfileSpace { get { return this.general_profile_space; } set { this.general_profile_space = value; } }

	protected bool general_tier_flag; 
	public bool GeneralTierFlag { get { return this.general_tier_flag; } set { this.general_tier_flag = value; } }

	protected byte general_profile_idc; 
	public byte GeneralProfileIdc { get { return this.general_profile_idc; } set { this.general_profile_idc = value; } }

	protected uint general_profile_compatibility_flags; 
	public uint GeneralProfileCompatibilityFlags { get { return this.general_profile_compatibility_flags; } set { this.general_profile_compatibility_flags = value; } }

	protected ulong general_constraint_indicator_flags; 
	public ulong GeneralConstraintIndicatorFlags { get { return this.general_constraint_indicator_flags; } set { this.general_constraint_indicator_flags = value; } }

	protected byte general_level_idc; 
	public byte GeneralLevelIdc { get { return this.general_level_idc; } set { this.general_level_idc = value; } }

	protected byte reserved = 0b1111; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort min_spatial_segmentation_idc; 
	public ushort MinSpatialSegmentationIdc { get { return this.min_spatial_segmentation_idc; } set { this.min_spatial_segmentation_idc = value; } }

	protected byte reserved0 = 0b111111; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte parallelismType; 
	public byte ParallelismType { get { return this.parallelismType; } set { this.parallelismType = value; } }

	protected byte reserved1 = 0b111111; 
	public byte Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected byte chromaFormat; 
	public byte ChromaFormat { get { return this.chromaFormat; } set { this.chromaFormat = value; } }

	protected byte reserved2 = 0b11111; 
	public byte Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected byte bitDepthLumaMinus8; 
	public byte BitDepthLumaMinus8 { get { return this.bitDepthLumaMinus8; } set { this.bitDepthLumaMinus8 = value; } }

	protected byte reserved3 = 0b11111; 
	public byte Reserved3 { get { return this.reserved3; } set { this.reserved3 = value; } }

	protected byte bitDepthChromaMinus8; 
	public byte BitDepthChromaMinus8 { get { return this.bitDepthChromaMinus8; } set { this.bitDepthChromaMinus8 = value; } }

	protected ushort avgFrameRate; 
	public ushort AvgFrameRate { get { return this.avgFrameRate; } set { this.avgFrameRate = value; } }

	protected byte constantFrameRate; 
	public byte ConstantFrameRate { get { return this.constantFrameRate; } set { this.constantFrameRate = value; } }

	protected byte numTemporalLayers; 
	public byte NumTemporalLayers { get { return this.numTemporalLayers; } set { this.numTemporalLayers = value; } }

	protected bool temporalIdNested; 
	public bool TemporalIdNested { get { return this.temporalIdNested; } set { this.temporalIdNested = value; } }

	protected byte lengthSizeMinusOne; 
	public byte LengthSizeMinusOne { get { return this.lengthSizeMinusOne; } set { this.lengthSizeMinusOne = value; } }

	protected byte numOfArrays; 
	public byte NumOfArrays { get { return this.numOfArrays; } set { this.numOfArrays = value; } }

	protected bool[] array_completeness; 
	public bool[] ArrayCompleteness { get { return this.array_completeness; } set { this.array_completeness = value; } }

	protected bool[] reserved4; 
	public bool[] Reserved4 { get { return this.reserved4; } set { this.reserved4 = value; } }

	protected byte[] NAL_unit_type; 
	public byte[] NALUnitType { get { return this.NAL_unit_type; } set { this.NAL_unit_type = value; } }

	protected ushort[] numNalus; 
	public ushort[] NumNalus { get { return this.numNalus; } set { this.numNalus = value; } }

	protected ushort[][] nalUnitLength; 
	public ushort[][] NalUnitLength { get { return this.nalUnitLength; } set { this.nalUnitLength = value; } }

	protected byte[][][] nalUnit; 
	public byte[][][] NalUnit { get { return this.nalUnit; } set { this.nalUnit = value; } }

	public HEVCDecoderConfigurationRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.configurationVersion, "configurationVersion"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.general_profile_space, "general_profile_space"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.general_tier_flag, "general_tier_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.general_profile_idc, "general_profile_idc"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.general_profile_compatibility_flags, "general_profile_compatibility_flags"); 
		boxSize += stream.ReadUInt48(boxSize, readSize,  out this.general_constraint_indicator_flags, "general_constraint_indicator_flags"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.general_level_idc, "general_level_idc"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.min_spatial_segmentation_idc, "min_spatial_segmentation_idc"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.parallelismType, "parallelismType"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.chromaFormat, "chromaFormat"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bitDepthLumaMinus8, "bitDepthLumaMinus8"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved3, "reserved3"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bitDepthChromaMinus8, "bitDepthChromaMinus8"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avgFrameRate, "avgFrameRate"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.constantFrameRate, "constantFrameRate"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.numTemporalLayers, "numTemporalLayers"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.temporalIdNested, "temporalIdNested"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.lengthSizeMinusOne, "lengthSizeMinusOne"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numOfArrays, "numOfArrays"); 

		this.array_completeness = new bool[IsoStream.GetInt( numOfArrays)];
		this.reserved4 = new bool[IsoStream.GetInt( numOfArrays)];
		this.NAL_unit_type = new byte[IsoStream.GetInt( numOfArrays)];
		this.numNalus = new ushort[IsoStream.GetInt( numOfArrays)];
		this.nalUnitLength = new ushort[IsoStream.GetInt( numOfArrays)][];
		this.nalUnit = new byte[IsoStream.GetInt( numOfArrays)][][];
		for (int j=0; j < numOfArrays; j++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.array_completeness[j], "array_completeness"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved4[j], "reserved4"); 
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.NAL_unit_type[j], "NAL_unit_type"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.numNalus[j], "numNalus"); 

			this.nalUnitLength[j] = new ushort[IsoStream.GetInt( numNalus[j])];
			this.nalUnit[j] = new byte[IsoStream.GetInt( numNalus[j])][];
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
		boxSize += stream.WriteBits(2,  this.general_profile_space, "general_profile_space"); 
		boxSize += stream.WriteBit( this.general_tier_flag, "general_tier_flag"); 
		boxSize += stream.WriteBits(5,  this.general_profile_idc, "general_profile_idc"); 
		boxSize += stream.WriteUInt32( this.general_profile_compatibility_flags, "general_profile_compatibility_flags"); 
		boxSize += stream.WriteUInt48( this.general_constraint_indicator_flags, "general_constraint_indicator_flags"); 
		boxSize += stream.WriteUInt8( this.general_level_idc, "general_level_idc"); 
		boxSize += stream.WriteBits(4,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(12,  this.min_spatial_segmentation_idc, "min_spatial_segmentation_idc"); 
		boxSize += stream.WriteBits(6,  this.reserved0, "reserved0"); 
		boxSize += stream.WriteBits(2,  this.parallelismType, "parallelismType"); 
		boxSize += stream.WriteBits(6,  this.reserved1, "reserved1"); 
		boxSize += stream.WriteBits(2,  this.chromaFormat, "chromaFormat"); 
		boxSize += stream.WriteBits(5,  this.reserved2, "reserved2"); 
		boxSize += stream.WriteBits(3,  this.bitDepthLumaMinus8, "bitDepthLumaMinus8"); 
		boxSize += stream.WriteBits(5,  this.reserved3, "reserved3"); 
		boxSize += stream.WriteBits(3,  this.bitDepthChromaMinus8, "bitDepthChromaMinus8"); 
		boxSize += stream.WriteUInt16( this.avgFrameRate, "avgFrameRate"); 
		boxSize += stream.WriteBits(2,  this.constantFrameRate, "constantFrameRate"); 
		boxSize += stream.WriteBits(3,  this.numTemporalLayers, "numTemporalLayers"); 
		boxSize += stream.WriteBit( this.temporalIdNested, "temporalIdNested"); 
		boxSize += stream.WriteBits(2,  this.lengthSizeMinusOne, "lengthSizeMinusOne"); 
		boxSize += stream.WriteUInt8( this.numOfArrays, "numOfArrays"); 

		for (int j=0; j < numOfArrays; j++)
		{
			boxSize += stream.WriteBit( this.array_completeness[j], "array_completeness"); 
			boxSize += stream.WriteBit( this.reserved4[j], "reserved4"); 
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
		boxSize += 2; // general_profile_space
		boxSize += 1; // general_tier_flag
		boxSize += 5; // general_profile_idc
		boxSize += 32; // general_profile_compatibility_flags
		boxSize += 48; // general_constraint_indicator_flags
		boxSize += 8; // general_level_idc
		boxSize += 4; // reserved
		boxSize += 12; // min_spatial_segmentation_idc
		boxSize += 6; // reserved0
		boxSize += 2; // parallelismType
		boxSize += 6; // reserved1
		boxSize += 2; // chromaFormat
		boxSize += 5; // reserved2
		boxSize += 3; // bitDepthLumaMinus8
		boxSize += 5; // reserved3
		boxSize += 3; // bitDepthChromaMinus8
		boxSize += 16; // avgFrameRate
		boxSize += 2; // constantFrameRate
		boxSize += 3; // numTemporalLayers
		boxSize += 1; // temporalIdNested
		boxSize += 2; // lengthSizeMinusOne
		boxSize += 8; // numOfArrays

		for (int j=0; j < numOfArrays; j++)
		{
			boxSize += 1; // array_completeness
			boxSize += 1; // reserved4
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
