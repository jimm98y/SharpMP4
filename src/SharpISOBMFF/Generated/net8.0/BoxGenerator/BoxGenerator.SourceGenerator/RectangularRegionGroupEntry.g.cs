using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class RectangularRegionGroupEntry() extends VisualSampleGroupEntry ('trif')
{
	unsigned int(16) groupID;
	unsigned int(1) rect_region_flag;
	if (!rect_region_flag)
		bit(7)  reserved = 0;
	else {
		unsigned int(2) independent_idc;
		unsigned int(1) full_picture;
		unsigned int(1) filtering_disabled;
		unsigned int(1) has_dependency_list;
		bit(2)  reserved = 0;
		if (!full_picture) {
			unsigned int(16) horizontal_offset;
			unsigned int(16) vertical_offset;
		}
		unsigned int(16) region_width;
		unsigned int(16) region_height;
		if (has_dependency_list) {
			unsigned int(16) dependency_rect_region_count;
			for (i=1; i<= dependency_rect_region_count; i++)
				unsigned int(16) dependencyRectRegionGroupID;
	}
}
}
*/
public partial class RectangularRegionGroupEntry : VisualSampleGroupEntry
{
	public const string TYPE = "trif";
	public override string DisplayName { get { return "RectangularRegionGroupEntry"; } }

	protected ushort groupID; 
	public ushort GroupID { get { return this.groupID; } set { this.groupID = value; } }

	protected bool rect_region_flag; 
	public bool RectRegionFlag { get { return this.rect_region_flag; } set { this.rect_region_flag = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte independent_idc; 
	public byte IndependentIdc { get { return this.independent_idc; } set { this.independent_idc = value; } }

	protected bool full_picture; 
	public bool FullPicture { get { return this.full_picture; } set { this.full_picture = value; } }

	protected bool filtering_disabled; 
	public bool FilteringDisabled { get { return this.filtering_disabled; } set { this.filtering_disabled = value; } }

	protected bool has_dependency_list; 
	public bool HasDependencyList { get { return this.has_dependency_list; } set { this.has_dependency_list = value; } }

	protected byte reserved0 = 0; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected ushort horizontal_offset; 
	public ushort HorizontalOffset { get { return this.horizontal_offset; } set { this.horizontal_offset = value; } }

	protected ushort vertical_offset; 
	public ushort VerticalOffset { get { return this.vertical_offset; } set { this.vertical_offset = value; } }

	protected ushort region_width; 
	public ushort RegionWidth { get { return this.region_width; } set { this.region_width = value; } }

	protected ushort region_height; 
	public ushort RegionHeight { get { return this.region_height; } set { this.region_height = value; } }

	protected ushort dependency_rect_region_count; 
	public ushort DependencyRectRegionCount { get { return this.dependency_rect_region_count; } set { this.dependency_rect_region_count = value; } }

	protected ushort[] dependencyRectRegionGroupID; 
	public ushort[] DependencyRectRegionGroupID { get { return this.dependencyRectRegionGroupID; } set { this.dependencyRectRegionGroupID = value; } }

	public RectangularRegionGroupEntry(): base(IsoStream.FromFourCC("trif"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.groupID, "groupID"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.rect_region_flag, "rect_region_flag"); 

		if (!rect_region_flag)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		}

		else 
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.independent_idc, "independent_idc"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.full_picture, "full_picture"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.filtering_disabled, "filtering_disabled"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.has_dependency_list, "has_dependency_list"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved0, "reserved0"); 

			if (!full_picture)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.horizontal_offset, "horizontal_offset"); 
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.vertical_offset, "vertical_offset"); 
			}
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.region_width, "region_width"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.region_height, "region_height"); 

			if (has_dependency_list)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dependency_rect_region_count, "dependency_rect_region_count"); 

				this.dependencyRectRegionGroupID = new ushort[IsoStream.GetInt( dependency_rect_region_count)];
				for (int i=0; i< dependency_rect_region_count; i++)
				{
					boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dependencyRectRegionGroupID[i], "dependencyRectRegionGroupID"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.groupID, "groupID"); 
		boxSize += stream.WriteBit( this.rect_region_flag, "rect_region_flag"); 

		if (!rect_region_flag)
		{
			boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		}

		else 
		{
			boxSize += stream.WriteBits(2,  this.independent_idc, "independent_idc"); 
			boxSize += stream.WriteBit( this.full_picture, "full_picture"); 
			boxSize += stream.WriteBit( this.filtering_disabled, "filtering_disabled"); 
			boxSize += stream.WriteBit( this.has_dependency_list, "has_dependency_list"); 
			boxSize += stream.WriteBits(2,  this.reserved0, "reserved0"); 

			if (!full_picture)
			{
				boxSize += stream.WriteUInt16( this.horizontal_offset, "horizontal_offset"); 
				boxSize += stream.WriteUInt16( this.vertical_offset, "vertical_offset"); 
			}
			boxSize += stream.WriteUInt16( this.region_width, "region_width"); 
			boxSize += stream.WriteUInt16( this.region_height, "region_height"); 

			if (has_dependency_list)
			{
				boxSize += stream.WriteUInt16( this.dependency_rect_region_count, "dependency_rect_region_count"); 

				for (int i=0; i< dependency_rect_region_count; i++)
				{
					boxSize += stream.WriteUInt16( this.dependencyRectRegionGroupID[i], "dependencyRectRegionGroupID"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // groupID
		boxSize += 1; // rect_region_flag

		if (!rect_region_flag)
		{
			boxSize += 7; // reserved
		}

		else 
		{
			boxSize += 2; // independent_idc
			boxSize += 1; // full_picture
			boxSize += 1; // filtering_disabled
			boxSize += 1; // has_dependency_list
			boxSize += 2; // reserved0

			if (!full_picture)
			{
				boxSize += 16; // horizontal_offset
				boxSize += 16; // vertical_offset
			}
			boxSize += 16; // region_width
			boxSize += 16; // region_height

			if (has_dependency_list)
			{
				boxSize += 16; // dependency_rect_region_count

				for (int i=0; i< dependency_rect_region_count; i++)
				{
					boxSize += 16; // dependencyRectRegionGroupID
				}
			}
		}
		return boxSize;
	}
}

}
