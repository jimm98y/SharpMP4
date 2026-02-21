using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class EVCDecoderConfigurationRecord {
unsigned int(8) configurationVersion=1;
unsigned int(8) profile_idc;
unsigned int(8) level_idc;
 unsigned int(32) toolset_idc;
unsigned int(2) chroma_format_idc;
 unsigned int(3) bit_depth_luma_minus8;
unsigned int(3) bit_depth_chroma_minus8;
unsigned int(32) pic_width_in_luma_samples;
unsigned int(32) pic_height_in_luma_samples;
unsigned int(5) reserved='00000'b;
unsigned int(1) sps_in_stream;
 unsigned int(1) pps_in_stream;
unsigned int(1) aps_in_stream;
 unsigned int(8) numOfArrays;
 for (j=0; j<numOfArrays; j++) {
bit(2) reserved='00'b;
unsigned int(6) NAL_unit_type;
 unsigned int(16) numNalus;
for (i=0; i<numNalus; i++) {
 unsigned int(16) nalUnitLength;
bit(8*nalUnitLength) nalUnit;
}
}
}
*/
public partial class EVCDecoderConfigurationRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "EVCDecoderConfigurationRecord"; } }

	protected byte configurationVersion =1; 
	public byte ConfigurationVersion { get { return this.configurationVersion; } set { this.configurationVersion = value; } }

	protected byte profile_idc; 
	public byte ProfileIdc { get { return this.profile_idc; } set { this.profile_idc = value; } }

	protected byte level_idc; 
	public byte LevelIdc { get { return this.level_idc; } set { this.level_idc = value; } }

	protected uint toolset_idc; 
	public uint ToolsetIdc { get { return this.toolset_idc; } set { this.toolset_idc = value; } }

	protected byte chroma_format_idc; 
	public byte ChromaFormatIdc { get { return this.chroma_format_idc; } set { this.chroma_format_idc = value; } }

	protected byte bit_depth_luma_minus8; 
	public byte BitDepthLumaMinus8 { get { return this.bit_depth_luma_minus8; } set { this.bit_depth_luma_minus8 = value; } }

	protected byte bit_depth_chroma_minus8; 
	public byte BitDepthChromaMinus8 { get { return this.bit_depth_chroma_minus8; } set { this.bit_depth_chroma_minus8 = value; } }

	protected uint pic_width_in_luma_samples; 
	public uint PicWidthInLumaSamples { get { return this.pic_width_in_luma_samples; } set { this.pic_width_in_luma_samples = value; } }

	protected uint pic_height_in_luma_samples; 
	public uint PicHeightInLumaSamples { get { return this.pic_height_in_luma_samples; } set { this.pic_height_in_luma_samples = value; } }

	protected byte reserved =0b00000; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool sps_in_stream; 
	public bool SpsInStream { get { return this.sps_in_stream; } set { this.sps_in_stream = value; } }

	protected bool pps_in_stream; 
	public bool PpsInStream { get { return this.pps_in_stream; } set { this.pps_in_stream = value; } }

	protected bool aps_in_stream; 
	public bool ApsInStream { get { return this.aps_in_stream; } set { this.aps_in_stream = value; } }

	protected byte numOfArrays; 
	public byte NumOfArrays { get { return this.numOfArrays; } set { this.numOfArrays = value; } }

	protected byte[] reserved0; 
	public byte[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte[] NAL_unit_type; 
	public byte[] NALUnitType { get { return this.NAL_unit_type; } set { this.NAL_unit_type = value; } }

	protected ushort[] numNalus; 
	public ushort[] NumNalus { get { return this.numNalus; } set { this.numNalus = value; } }

	protected ushort[][] nalUnitLength; 
	public ushort[][] NalUnitLength { get { return this.nalUnitLength; } set { this.nalUnitLength = value; } }

	protected byte[][][] nalUnit; 
	public byte[][][] NalUnit { get { return this.nalUnit; } set { this.nalUnit = value; } }

	public EVCDecoderConfigurationRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.configurationVersion, "configurationVersion"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.profile_idc, "profile_idc"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level_idc, "level_idc"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.toolset_idc, "toolset_idc"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.chroma_format_idc, "chroma_format_idc"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bit_depth_luma_minus8, "bit_depth_luma_minus8"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bit_depth_chroma_minus8, "bit_depth_chroma_minus8"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.pic_width_in_luma_samples, "pic_width_in_luma_samples"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.pic_height_in_luma_samples, "pic_height_in_luma_samples"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.sps_in_stream, "sps_in_stream"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.pps_in_stream, "pps_in_stream"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.aps_in_stream, "aps_in_stream"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numOfArrays, "numOfArrays"); 

		this.reserved0 = new byte[IsoStream.GetInt(numOfArrays)];
		this.NAL_unit_type = new byte[IsoStream.GetInt(numOfArrays)];
		this.numNalus = new ushort[IsoStream.GetInt(numOfArrays)];
		this.nalUnitLength = new ushort[IsoStream.GetInt(numOfArrays)][];
		this.nalUnit = new byte[IsoStream.GetInt(numOfArrays)][][];
		for (int j=0; j<numOfArrays; j++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved0[j], "reserved0"); 
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.NAL_unit_type[j], "NAL_unit_type"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.numNalus[j], "numNalus"); 

			this.nalUnitLength[j] = new ushort[IsoStream.GetInt(numNalus[j])];
			this.nalUnit[j] = new byte[IsoStream.GetInt(numNalus[j])][];
			for (int i=0; i<numNalus[j]; i++)
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
		boxSize += stream.WriteUInt8( this.profile_idc, "profile_idc"); 
		boxSize += stream.WriteUInt8( this.level_idc, "level_idc"); 
		boxSize += stream.WriteUInt32( this.toolset_idc, "toolset_idc"); 
		boxSize += stream.WriteBits(2,  this.chroma_format_idc, "chroma_format_idc"); 
		boxSize += stream.WriteBits(3,  this.bit_depth_luma_minus8, "bit_depth_luma_minus8"); 
		boxSize += stream.WriteBits(3,  this.bit_depth_chroma_minus8, "bit_depth_chroma_minus8"); 
		boxSize += stream.WriteUInt32( this.pic_width_in_luma_samples, "pic_width_in_luma_samples"); 
		boxSize += stream.WriteUInt32( this.pic_height_in_luma_samples, "pic_height_in_luma_samples"); 
		boxSize += stream.WriteBits(5,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.sps_in_stream, "sps_in_stream"); 
		boxSize += stream.WriteBit( this.pps_in_stream, "pps_in_stream"); 
		boxSize += stream.WriteBit( this.aps_in_stream, "aps_in_stream"); 
		boxSize += stream.WriteUInt8( this.numOfArrays, "numOfArrays"); 

		for (int j=0; j<numOfArrays; j++)
		{
			boxSize += stream.WriteBits(2,  this.reserved0[j], "reserved0"); 
			boxSize += stream.WriteBits(6,  this.NAL_unit_type[j], "NAL_unit_type"); 
			boxSize += stream.WriteUInt16( this.numNalus[j], "numNalus"); 

			for (int i=0; i<numNalus[j]; i++)
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
		boxSize += 8; // profile_idc
		boxSize += 8; // level_idc
		boxSize += 32; // toolset_idc
		boxSize += 2; // chroma_format_idc
		boxSize += 3; // bit_depth_luma_minus8
		boxSize += 3; // bit_depth_chroma_minus8
		boxSize += 32; // pic_width_in_luma_samples
		boxSize += 32; // pic_height_in_luma_samples
		boxSize += 5; // reserved
		boxSize += 1; // sps_in_stream
		boxSize += 1; // pps_in_stream
		boxSize += 1; // aps_in_stream
		boxSize += 8; // numOfArrays

		for (int j=0; j<numOfArrays; j++)
		{
			boxSize += 2; // reserved0
			boxSize += 6; // NAL_unit_type
			boxSize += 16; // numNalus

			for (int i=0; i<numNalus[j]; i++)
			{
				boxSize += 16; // nalUnitLength
				boxSize += (ulong)(8*nalUnitLength[j][i] ); // nalUnit
			}
		}
		return boxSize;
	}
}

}
