using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class MVDDepthResolutionBox extends Box('3dpr')
{
	unsigned int(16) depth_width;
	unsigned int(16) depth_height;
/* The following 5 fields are collectively optional; they are either all present or all absent. When grid_pos_num_views is not present, the for loop is not present, equivalent to grid_pos_num_views equal to 0. These fields may be present or absent whenever the box is present (e.g., in MVCDConfigurationBox or A3DConfigurationBox). *//*
	unsigned int(16) depth_hor_mult_minus1; // optional
	unsigned int(16) depth_ver_mult_minus1; // optional
	unsigned int(4) depth_hor_rsh; // optional
	unsigned int(4) depth_ver_rsh; // optional
	unsigned int(16) grid_pos_num_views; // optional
	for(i = 0; i < grid_pos_num_views; i++) {
		bit(6) reserved=0;
		unsigned int(10) grid_pos_view_id[i];
		signed int(16) grid_pos_x[grid_pos_view_id[i]];
		signed int(16) grid_pos_y[grid_pos_view_id[i]];
	}
}
*/
public partial class MVDDepthResolutionBox : Box
{
	public const string TYPE = "3dpr";
	public override string DisplayName { get { return "MVDDepthResolutionBox"; } }

	protected ushort depth_width; 
	public ushort DepthWidth { get { return this.depth_width; } set { this.depth_width = value; } }

	protected ushort depth_height; 
	public ushort DepthHeight { get { return this.depth_height; } set { this.depth_height = value; } }

	protected ushort depth_hor_mult_minus1;  //  optional
	public ushort DepthHorMultMinus1 { get { return this.depth_hor_mult_minus1; } set { this.depth_hor_mult_minus1 = value; } }

	protected ushort depth_ver_mult_minus1;  //  optional
	public ushort DepthVerMultMinus1 { get { return this.depth_ver_mult_minus1; } set { this.depth_ver_mult_minus1 = value; } }

	protected byte depth_hor_rsh;  //  optional
	public byte DepthHorRsh { get { return this.depth_hor_rsh; } set { this.depth_hor_rsh = value; } }

	protected byte depth_ver_rsh;  //  optional
	public byte DepthVerRsh { get { return this.depth_ver_rsh; } set { this.depth_ver_rsh = value; } }

	protected ushort grid_pos_num_views;  //  optional
	public ushort GridPosNumViews { get { return this.grid_pos_num_views; } set { this.grid_pos_num_views = value; } }

	protected byte[] reserved; 
	public byte[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort[] grid_pos_view_id; 
	public ushort[] GridPosViewId { get { return this.grid_pos_view_id; } set { this.grid_pos_view_id = value; } }

	protected short[] grid_pos_x; 
	public short[] GridPosx { get { return this.grid_pos_x; } set { this.grid_pos_x = value; } }

	protected short[] grid_pos_y; 
	public short[] GridPosy { get { return this.grid_pos_y; } set { this.grid_pos_y = value; } }

	public MVDDepthResolutionBox(): base(IsoStream.FromFourCC("3dpr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.depth_width, "depth_width"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.depth_height, "depth_height"); 
		/*  The following 5 fields are collectively optional; they are either all present or all absent. When grid_pos_num_views is not present, the for loop is not present, equivalent to grid_pos_num_views equal to 0. These fields may be present or absent whenever the box is present (e.g., in MVCDConfigurationBox or A3DConfigurationBox).  */
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadUInt16(boxSize, readSize,  out this.depth_hor_mult_minus1, "depth_hor_mult_minus1"); // optional
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadUInt16(boxSize, readSize,  out this.depth_ver_mult_minus1, "depth_ver_mult_minus1"); // optional
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.depth_hor_rsh, "depth_hor_rsh"); // optional
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.depth_ver_rsh, "depth_ver_rsh"); // optional
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadUInt16(boxSize, readSize,  out this.grid_pos_num_views, "grid_pos_num_views"); // optional

		this.reserved = new byte[IsoStream.GetInt( grid_pos_num_views)];
		this.grid_pos_view_id = new ushort[IsoStream.GetInt( grid_pos_num_views)];
		this.grid_pos_x = new short[IsoStream.GetInt( grid_pos_num_views)];
		this.grid_pos_y = new short[IsoStream.GetInt( grid_pos_num_views)];
		for (int i = 0; i < grid_pos_num_views; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved[i], "reserved"); 
			boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.grid_pos_view_id[i], "grid_pos_view_id"); 
			boxSize += stream.ReadInt16(boxSize, readSize,  out this.grid_pos_x[grid_pos_view_id[i]], "grid_pos_x"); 
			boxSize += stream.ReadInt16(boxSize, readSize,  out this.grid_pos_y[grid_pos_view_id[i]], "grid_pos_y"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.depth_width, "depth_width"); 
		boxSize += stream.WriteUInt16( this.depth_height, "depth_height"); 
		/*  The following 5 fields are collectively optional; they are either all present or all absent. When grid_pos_num_views is not present, the for loop is not present, equivalent to grid_pos_num_views equal to 0. These fields may be present or absent whenever the box is present (e.g., in MVCDConfigurationBox or A3DConfigurationBox).  */
		boxSize += stream.WriteUInt16( this.depth_hor_mult_minus1, "depth_hor_mult_minus1"); // optional
		boxSize += stream.WriteUInt16( this.depth_ver_mult_minus1, "depth_ver_mult_minus1"); // optional
		boxSize += stream.WriteBits(4,  this.depth_hor_rsh, "depth_hor_rsh"); // optional
		boxSize += stream.WriteBits(4,  this.depth_ver_rsh, "depth_ver_rsh"); // optional
		boxSize += stream.WriteUInt16( this.grid_pos_num_views, "grid_pos_num_views"); // optional

		for (int i = 0; i < grid_pos_num_views; i++)
		{
			boxSize += stream.WriteBits(6,  this.reserved[i], "reserved"); 
			boxSize += stream.WriteBits(10,  this.grid_pos_view_id[i], "grid_pos_view_id"); 
			boxSize += stream.WriteInt16( this.grid_pos_x[grid_pos_view_id[i]], "grid_pos_x"); 
			boxSize += stream.WriteInt16( this.grid_pos_y[grid_pos_view_id[i]], "grid_pos_y"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // depth_width
		boxSize += 16; // depth_height
		/*  The following 5 fields are collectively optional; they are either all present or all absent. When grid_pos_num_views is not present, the for loop is not present, equivalent to grid_pos_num_views equal to 0. These fields may be present or absent whenever the box is present (e.g., in MVCDConfigurationBox or A3DConfigurationBox).  */
		boxSize += 16; // depth_hor_mult_minus1
		boxSize += 16; // depth_ver_mult_minus1
		boxSize += 4; // depth_hor_rsh
		boxSize += 4; // depth_ver_rsh
		boxSize += 16; // grid_pos_num_views

		for (int i = 0; i < grid_pos_num_views; i++)
		{
			boxSize += 6; // reserved
			boxSize += 10; // grid_pos_view_id
			boxSize += 16; // grid_pos_x
			boxSize += 16; // grid_pos_y
		}
		return boxSize;
	}
}

}
