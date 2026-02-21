using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class VvcDecoderConfigurationRecord {
	bit(5) reserved = '11111'b;
	unsigned int(2) LengthSizeMinusOne;
	unsigned int(1) ptl_present_flag;
	if (ptl_present_flag) {
		unsigned int(9) ols_idx;
		unsigned int(3) num_sublayers;
		unsigned int(2) constant_frame_rate;
		unsigned int(2) chroma_format_idc;
		unsigned int(3) bit_depth_minus8;
		bit(5) reserved = '11111'b;
		VvcPTLRecord(num_sublayers) native_ptl;
		unsigned_int(16) max_picture_width;
		unsigned_int(16) max_picture_height;
		unsigned int(16) avg_frame_rate;
	}
	unsigned int(8) num_of_arrays;
	for (j=0; j < num_of_arrays; j++) {
		unsigned int(1) array_completeness;
		bit(2) reserved = 0;
		unsigned int(5) NAL_unit_type;
		if (NAL_unit_type != DCI_NUT  &&  NAL_unit_type != OPI_NUT) {
			unsigned int(16) num_nalus;
		for (i=0; i< num_nalus; i++) {
			unsigned int(16) nal_unit_length;
			bit(8*nal_unit_length) nal_unit;
		}
		}
	}
}
*/
public partial class VvcDecoderConfigurationRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "VvcDecoderConfigurationRecord"; } }

	protected byte reserved = 0b11111; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte LengthSizeMinusOne; 
	public byte _LengthSizeMinusOne { get { return this.LengthSizeMinusOne; } set { this.LengthSizeMinusOne = value; } }

	protected bool ptl_present_flag; 
	public bool PtlPresentFlag { get { return this.ptl_present_flag; } set { this.ptl_present_flag = value; } }

	protected ushort ols_idx; 
	public ushort OlsIdx { get { return this.ols_idx; } set { this.ols_idx = value; } }

	protected byte num_sublayers; 
	public byte NumSublayers { get { return this.num_sublayers; } set { this.num_sublayers = value; } }

	protected byte constant_frame_rate; 
	public byte ConstantFrameRate { get { return this.constant_frame_rate; } set { this.constant_frame_rate = value; } }

	protected byte chroma_format_idc; 
	public byte ChromaFormatIdc { get { return this.chroma_format_idc; } set { this.chroma_format_idc = value; } }

	protected byte bit_depth_minus8; 
	public byte BitDepthMinus8 { get { return this.bit_depth_minus8; } set { this.bit_depth_minus8 = value; } }

	protected byte reserved0 = 0b11111; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected VvcPTLRecord native_ptl; 
	public VvcPTLRecord NativePtl { get { return this.native_ptl; } set { this.native_ptl = value; } }

	protected ushort max_picture_width; 
	public ushort MaxPictureWidth { get { return this.max_picture_width; } set { this.max_picture_width = value; } }

	protected ushort max_picture_height; 
	public ushort MaxPictureHeight { get { return this.max_picture_height; } set { this.max_picture_height = value; } }

	protected ushort avg_frame_rate; 
	public ushort AvgFrameRate { get { return this.avg_frame_rate; } set { this.avg_frame_rate = value; } }

	protected byte num_of_arrays; 
	public byte NumOfArrays { get { return this.num_of_arrays; } set { this.num_of_arrays = value; } }

	protected bool[] array_completeness; 
	public bool[] ArrayCompleteness { get { return this.array_completeness; } set { this.array_completeness = value; } }

	protected byte[] reserved1; 
	public byte[] Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected byte[] NAL_unit_type; 
	public byte[] NALUnitType { get { return this.NAL_unit_type; } set { this.NAL_unit_type = value; } }

	protected ushort[] num_nalus; 
	public ushort[] NumNalus { get { return this.num_nalus; } set { this.num_nalus = value; } }

	protected ushort[][] nal_unit_length; 
	public ushort[][] NalUnitLength { get { return this.nal_unit_length; } set { this.nal_unit_length = value; } }

	protected byte[][][] nal_unit; 
	public byte[][][] NalUnit { get { return this.nal_unit; } set { this.nal_unit = value; } }

	public VvcDecoderConfigurationRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		const int OPI_NUT = 12;
		const int DCI_NUT = 13;

		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.LengthSizeMinusOne, "LengthSizeMinusOne"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.ptl_present_flag, "ptl_present_flag"); 

		if (ptl_present_flag)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 9,  out this.ols_idx, "ols_idx"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.num_sublayers, "num_sublayers"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.constant_frame_rate, "constant_frame_rate"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.chroma_format_idc, "chroma_format_idc"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bit_depth_minus8, "bit_depth_minus8"); 
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved0, "reserved0"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new VvcPTLRecord(num_sublayers),  out this.native_ptl, "native_ptl"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_picture_width, "max_picture_width"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_picture_height, "max_picture_height"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avg_frame_rate, "avg_frame_rate"); 
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_of_arrays, "num_of_arrays"); 

		this.array_completeness = new bool[IsoStream.GetInt( num_of_arrays)];
		this.reserved1 = new byte[IsoStream.GetInt( num_of_arrays)];
		this.NAL_unit_type = new byte[IsoStream.GetInt( num_of_arrays)];
		this.num_nalus = new ushort[IsoStream.GetInt( num_of_arrays)];
		this.nal_unit_length = new ushort[IsoStream.GetInt( num_of_arrays)][];
		this.nal_unit = new byte[IsoStream.GetInt( num_of_arrays)][][];
		for (int j=0; j < num_of_arrays; j++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.array_completeness[j], "array_completeness"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved1[j], "reserved1"); 
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.NAL_unit_type[j], "NAL_unit_type"); 

			if (NAL_unit_type[j] != DCI_NUT  &&  NAL_unit_type[j] != OPI_NUT)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_nalus[j], "num_nalus"); 

				this.nal_unit_length[j] = new ushort[IsoStream.GetInt( num_nalus[j])];
				this.nal_unit[j] = new byte[IsoStream.GetInt( num_nalus[j])][];
				for (int i=0; i< num_nalus[j]; i++)
				{
					boxSize += stream.ReadUInt16(boxSize, readSize,  out this.nal_unit_length[j][i], "nal_unit_length"); 
					boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*nal_unit_length[j][i] ),  out this.nal_unit[j][i], "nal_unit"); 
				}
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		const int OPI_NUT = 12;
		const int DCI_NUT = 13;

		boxSize += stream.WriteBits(5,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(2,  this.LengthSizeMinusOne, "LengthSizeMinusOne"); 
		boxSize += stream.WriteBit( this.ptl_present_flag, "ptl_present_flag"); 

		if (ptl_present_flag)
		{
			boxSize += stream.WriteBits(9,  this.ols_idx, "ols_idx"); 
			boxSize += stream.WriteBits(3,  this.num_sublayers, "num_sublayers"); 
			boxSize += stream.WriteBits(2,  this.constant_frame_rate, "constant_frame_rate"); 
			boxSize += stream.WriteBits(2,  this.chroma_format_idc, "chroma_format_idc"); 
			boxSize += stream.WriteBits(3,  this.bit_depth_minus8, "bit_depth_minus8"); 
			boxSize += stream.WriteBits(5,  this.reserved0, "reserved0"); 
			boxSize += stream.WriteClass( this.native_ptl, "native_ptl"); 
			boxSize += stream.WriteUInt16( this.max_picture_width, "max_picture_width"); 
			boxSize += stream.WriteUInt16( this.max_picture_height, "max_picture_height"); 
			boxSize += stream.WriteUInt16( this.avg_frame_rate, "avg_frame_rate"); 
		}
		boxSize += stream.WriteUInt8( this.num_of_arrays, "num_of_arrays"); 

		for (int j=0; j < num_of_arrays; j++)
		{
			boxSize += stream.WriteBit( this.array_completeness[j], "array_completeness"); 
			boxSize += stream.WriteBits(2,  this.reserved1[j], "reserved1"); 
			boxSize += stream.WriteBits(5,  this.NAL_unit_type[j], "NAL_unit_type"); 

			if (NAL_unit_type[j] != DCI_NUT  &&  NAL_unit_type[j] != OPI_NUT)
			{
				boxSize += stream.WriteUInt16( this.num_nalus[j], "num_nalus"); 

				for (int i=0; i< num_nalus[j]; i++)
				{
					boxSize += stream.WriteUInt16( this.nal_unit_length[j][i], "nal_unit_length"); 
					boxSize += stream.WriteBits((uint)(8*nal_unit_length[j][i] ),  this.nal_unit[j][i], "nal_unit"); 
				}
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		const int OPI_NUT = 12;
		const int DCI_NUT = 13;

		boxSize += 5; // reserved
		boxSize += 2; // LengthSizeMinusOne
		boxSize += 1; // ptl_present_flag

		if (ptl_present_flag)
		{
			boxSize += 9; // ols_idx
			boxSize += 3; // num_sublayers
			boxSize += 2; // constant_frame_rate
			boxSize += 2; // chroma_format_idc
			boxSize += 3; // bit_depth_minus8
			boxSize += 5; // reserved0
			boxSize += IsoStream.CalculateClassSize(native_ptl); // native_ptl
			boxSize += 16; // max_picture_width
			boxSize += 16; // max_picture_height
			boxSize += 16; // avg_frame_rate
		}
		boxSize += 8; // num_of_arrays

		for (int j=0; j < num_of_arrays; j++)
		{
			boxSize += 1; // array_completeness
			boxSize += 2; // reserved1
			boxSize += 5; // NAL_unit_type

			if (NAL_unit_type[j] != DCI_NUT  &&  NAL_unit_type[j] != OPI_NUT)
			{
				boxSize += 16; // num_nalus

				for (int i=0; i< num_nalus[j]; i++)
				{
					boxSize += 16; // nal_unit_length
					boxSize += (ulong)(8*nal_unit_length[j][i] ); // nal_unit
				}
			}
		}
		return boxSize;
	}
}

}
