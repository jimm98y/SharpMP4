using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MVCSubTrackMultiviewGroupBox
	extends FullBox('stmg', 0, 0) {
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++) {
		unsigned int(32)	MultiviewGroupId;
	}
}
*/
public partial class MVCSubTrackMultiviewGroupBox : FullBox
{
	public const string TYPE = "stmg";
	public override string DisplayName { get { return "MVCSubTrackMultiviewGroupBox"; } }

	protected ushort item_count; 
	public ushort ItemCount { get { return this.item_count; } set { this.item_count = value; } }

	protected uint[] MultiviewGroupId; 
	public uint[] _MultiviewGroupId { get { return this.MultiviewGroupId; } set { this.MultiviewGroupId = value; } }

	public MVCSubTrackMultiviewGroupBox(): base(IsoStream.FromFourCC("stmg"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_count, "item_count"); 

		this.MultiviewGroupId = new uint[IsoStream.GetInt( item_count)];
		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.MultiviewGroupId[i], "MultiviewGroupId"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.item_count, "item_count"); 

		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.WriteUInt32( this.MultiviewGroupId[i], "MultiviewGroupId"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // item_count

		for (int i = 0; i< item_count; i++)
		{
			boxSize += 32; // MultiviewGroupId
		}
		return boxSize;
	}
}

}
