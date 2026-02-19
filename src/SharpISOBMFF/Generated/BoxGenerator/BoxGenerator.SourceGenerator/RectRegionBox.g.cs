using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class RectRegionBox extends Box('rrgn'){
	unsigned int(16) base_region_tierID;
	unsigned int(1) dynamic_rect;
	bit(7) reserved = 0;
	if(dynamic_rect == 0) { 
		unsigned int(16) horizontal_offset;
		unsigned int(16) vertical_offset;
		unsigned int(16) region_width;
		unsigned int(16) region_height;
	}
}
*/
public partial class RectRegionBox : Box
{
	public const string TYPE = "rrgn";
	public override string DisplayName { get { return "RectRegionBox"; } }

	protected ushort base_region_tierID; 
	public ushort BaseRegionTierID { get { return this.base_region_tierID; } set { this.base_region_tierID = value; } }

	protected bool dynamic_rect; 
	public bool DynamicRect { get { return this.dynamic_rect; } set { this.dynamic_rect = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort horizontal_offset; 
	public ushort HorizontalOffset { get { return this.horizontal_offset; } set { this.horizontal_offset = value; } }

	protected ushort vertical_offset; 
	public ushort VerticalOffset { get { return this.vertical_offset; } set { this.vertical_offset = value; } }

	protected ushort region_width; 
	public ushort RegionWidth { get { return this.region_width; } set { this.region_width = value; } }

	protected ushort region_height; 
	public ushort RegionHeight { get { return this.region_height; } set { this.region_height = value; } }

	public RectRegionBox(): base(IsoStream.FromFourCC("rrgn"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.base_region_tierID, "base_region_tierID"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.dynamic_rect, "dynamic_rect"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 

		if (dynamic_rect == false)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.horizontal_offset, "horizontal_offset"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.vertical_offset, "vertical_offset"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.region_width, "region_width"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.region_height, "region_height"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.base_region_tierID, "base_region_tierID"); 
		boxSize += stream.WriteBit( this.dynamic_rect, "dynamic_rect"); 
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 

		if (dynamic_rect == false)
		{
			boxSize += stream.WriteUInt16( this.horizontal_offset, "horizontal_offset"); 
			boxSize += stream.WriteUInt16( this.vertical_offset, "vertical_offset"); 
			boxSize += stream.WriteUInt16( this.region_width, "region_width"); 
			boxSize += stream.WriteUInt16( this.region_height, "region_height"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // base_region_tierID
		boxSize += 1; // dynamic_rect
		boxSize += 7; // reserved

		if (dynamic_rect == false)
		{
			boxSize += 16; // horizontal_offset
			boxSize += 16; // vertical_offset
			boxSize += 16; // region_width
			boxSize += 16; // region_height
		}
		return boxSize;
	}
}

}
