using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TileSubTrackGroupBox extends FullBox('tstb', 0, 0) {
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++) {
		unsigned int(16) tileGroupID;
	}
}
*/
public partial class TileSubTrackGroupBox : FullBox
{
	public const string TYPE = "tstb";
	public override string DisplayName { get { return "TileSubTrackGroupBox"; } }

	protected ushort item_count; 
	public ushort ItemCount { get { return this.item_count; } set { this.item_count = value; } }

	protected ushort[] tileGroupID; 
	public ushort[] TileGroupID { get { return this.tileGroupID; } set { this.tileGroupID = value; } }

	public TileSubTrackGroupBox(): base(IsoStream.FromFourCC("tstb"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_count, "item_count"); 

		this.tileGroupID = new ushort[IsoStream.GetInt( item_count)];
		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tileGroupID[i], "tileGroupID"); 
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
			boxSize += stream.WriteUInt16( this.tileGroupID[i], "tileGroupID"); 
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
			boxSize += 16; // tileGroupID
		}
		return boxSize;
	}
}

}
