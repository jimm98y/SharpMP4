using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class IroiInfoBox extends Box('iroi'){
	unsigned int(2) iroi_type;
	bit(6) reserved = 0;
	if(iroi_type == 0) { 
		unsigned int(8) grid_roi_mb_width;
		unsigned int(8) grid_roi_mb_height;
	}
	else if(iroi_type == 1){
		unsigned int(24) num_roi;
		for(i=1; i<= num_roi; i++) {
			unsigned int(32) top_left_mb;
			unsigned int(8) roi_mb_width;
			unsigned int(8) roi_mb_height;
		}
	}
}
*/
public partial class IroiInfoBox : Box
{
	public const string TYPE = "iroi";
	public override string DisplayName { get { return "IroiInfoBox"; } }

	protected byte iroi_type; 
	public byte IroiType { get { return this.iroi_type; } set { this.iroi_type = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte grid_roi_mb_width; 
	public byte GridRoiMbWidth { get { return this.grid_roi_mb_width; } set { this.grid_roi_mb_width = value; } }

	protected byte grid_roi_mb_height; 
	public byte GridRoiMbHeight { get { return this.grid_roi_mb_height; } set { this.grid_roi_mb_height = value; } }

	protected uint num_roi; 
	public uint NumRoi { get { return this.num_roi; } set { this.num_roi = value; } }

	protected uint[] top_left_mb; 
	public uint[] TopLeftMb { get { return this.top_left_mb; } set { this.top_left_mb = value; } }

	protected byte[] roi_mb_width; 
	public byte[] RoiMbWidth { get { return this.roi_mb_width; } set { this.roi_mb_width = value; } }

	protected byte[] roi_mb_height; 
	public byte[] RoiMbHeight { get { return this.roi_mb_height; } set { this.roi_mb_height = value; } }

	public IroiInfoBox(): base(IsoStream.FromFourCC("iroi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.iroi_type, "iroi_type"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 

		if (iroi_type == 0)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.grid_roi_mb_width, "grid_roi_mb_width"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.grid_roi_mb_height, "grid_roi_mb_height"); 
		}

		else if (iroi_type == 1)
		{
			boxSize += stream.ReadUInt24(boxSize, readSize,  out this.num_roi, "num_roi"); 

			this.top_left_mb = new uint[IsoStream.GetInt( num_roi)];
			this.roi_mb_width = new byte[IsoStream.GetInt( num_roi)];
			this.roi_mb_height = new byte[IsoStream.GetInt( num_roi)];
			for (int i=0; i< num_roi; i++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.top_left_mb[i], "top_left_mb"); 
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.roi_mb_width[i], "roi_mb_width"); 
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.roi_mb_height[i], "roi_mb_height"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(2,  this.iroi_type, "iroi_type"); 
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 

		if (iroi_type == 0)
		{
			boxSize += stream.WriteUInt8( this.grid_roi_mb_width, "grid_roi_mb_width"); 
			boxSize += stream.WriteUInt8( this.grid_roi_mb_height, "grid_roi_mb_height"); 
		}

		else if (iroi_type == 1)
		{
			boxSize += stream.WriteUInt24( this.num_roi, "num_roi"); 

			for (int i=0; i< num_roi; i++)
			{
				boxSize += stream.WriteUInt32( this.top_left_mb[i], "top_left_mb"); 
				boxSize += stream.WriteUInt8( this.roi_mb_width[i], "roi_mb_width"); 
				boxSize += stream.WriteUInt8( this.roi_mb_height[i], "roi_mb_height"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 2; // iroi_type
		boxSize += 6; // reserved

		if (iroi_type == 0)
		{
			boxSize += 8; // grid_roi_mb_width
			boxSize += 8; // grid_roi_mb_height
		}

		else if (iroi_type == 1)
		{
			boxSize += 24; // num_roi

			for (int i=0; i< num_roi; i++)
			{
				boxSize += 32; // top_left_mb
				boxSize += 8; // roi_mb_width
				boxSize += 8; // roi_mb_height
			}
		}
		return boxSize;
	}
}

}
