using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class MasteringDisplayColourVolumeBox extends Box('mdcv'){
	for (c = 0; c<3; c++) {
		unsigned int(16) display_primaries_x;
		unsigned int(16) display_primaries_y;
	}
	unsigned int(16) white_point_x;
	unsigned int(16) white_point_y;
	unsigned int(32) max_display_mastering_luminance;
	unsigned int(32) min_display_mastering_luminance;
}
*/
public partial class MasteringDisplayColourVolumeBox : Box
{
	public const string TYPE = "mdcv";
	public override string DisplayName { get { return "MasteringDisplayColourVolumeBox"; } }

	protected ushort[] display_primaries_x; 
	public ushort[] DisplayPrimariesx { get { return this.display_primaries_x; } set { this.display_primaries_x = value; } }

	protected ushort[] display_primaries_y; 
	public ushort[] DisplayPrimariesy { get { return this.display_primaries_y; } set { this.display_primaries_y = value; } }

	protected ushort white_point_x; 
	public ushort WhitePointx { get { return this.white_point_x; } set { this.white_point_x = value; } }

	protected ushort white_point_y; 
	public ushort WhitePointy { get { return this.white_point_y; } set { this.white_point_y = value; } }

	protected uint max_display_mastering_luminance; 
	public uint MaxDisplayMasteringLuminance { get { return this.max_display_mastering_luminance; } set { this.max_display_mastering_luminance = value; } }

	protected uint min_display_mastering_luminance; 
	public uint MinDisplayMasteringLuminance { get { return this.min_display_mastering_luminance; } set { this.min_display_mastering_luminance = value; } }

	public MasteringDisplayColourVolumeBox(): base(IsoStream.FromFourCC("mdcv"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		this.display_primaries_x = new ushort[IsoStream.GetInt(3)];
		this.display_primaries_y = new ushort[IsoStream.GetInt(3)];
		for (int c = 0; c<3; c++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.display_primaries_x[c], "display_primaries_x"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.display_primaries_y[c], "display_primaries_y"); 
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.white_point_x, "white_point_x"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.white_point_y, "white_point_y"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.max_display_mastering_luminance, "max_display_mastering_luminance"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.min_display_mastering_luminance, "min_display_mastering_luminance"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		for (int c = 0; c<3; c++)
		{
			boxSize += stream.WriteUInt16( this.display_primaries_x[c], "display_primaries_x"); 
			boxSize += stream.WriteUInt16( this.display_primaries_y[c], "display_primaries_y"); 
		}
		boxSize += stream.WriteUInt16( this.white_point_x, "white_point_x"); 
		boxSize += stream.WriteUInt16( this.white_point_y, "white_point_y"); 
		boxSize += stream.WriteUInt32( this.max_display_mastering_luminance, "max_display_mastering_luminance"); 
		boxSize += stream.WriteUInt32( this.min_display_mastering_luminance, "min_display_mastering_luminance"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		for (int c = 0; c<3; c++)
		{
			boxSize += 16; // display_primaries_x
			boxSize += 16; // display_primaries_y
		}
		boxSize += 16; // white_point_x
		boxSize += 16; // white_point_y
		boxSize += 32; // max_display_mastering_luminance
		boxSize += 32; // min_display_mastering_luminance
		return boxSize;
	}
}

}
