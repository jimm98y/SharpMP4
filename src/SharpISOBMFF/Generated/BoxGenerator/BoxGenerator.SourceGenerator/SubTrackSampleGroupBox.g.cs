using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SubTrackSampleGroupBox
	extends FullBox('stsg', 0, 0){
	unsigned int(32) grouping_type;
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++)
		unsigned int(32)	group_description_index;
}
*/
public partial class SubTrackSampleGroupBox : FullBox
{
	public const string TYPE = "stsg";
	public override string DisplayName { get { return "SubTrackSampleGroupBox"; } }

	protected uint grouping_type; 
	public uint GroupingType { get { return this.grouping_type; } set { this.grouping_type = value; } }

	protected ushort item_count; 
	public ushort ItemCount { get { return this.item_count; } set { this.item_count = value; } }

	protected uint[] group_description_index; 
	public uint[] GroupDescriptionIndex { get { return this.group_description_index; } set { this.group_description_index = value; } }

	public SubTrackSampleGroupBox(): base(IsoStream.FromFourCC("stsg"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type, "grouping_type"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_count, "item_count"); 

		this.group_description_index = new uint[IsoStream.GetInt( item_count)];
		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.group_description_index[i], "group_description_index"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.grouping_type, "grouping_type"); 
		boxSize += stream.WriteUInt16( this.item_count, "item_count"); 

		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.WriteUInt32( this.group_description_index[i], "group_description_index"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // grouping_type
		boxSize += 16; // item_count

		for (int i = 0; i< item_count; i++)
		{
			boxSize += 32; // group_description_index
		}
		return boxSize;
	}
}

}
