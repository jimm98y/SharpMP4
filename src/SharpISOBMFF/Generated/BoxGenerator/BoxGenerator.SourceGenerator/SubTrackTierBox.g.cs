using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SubTrackTierBox
	extends FullBox('stti', 0, 0) {
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++) {
		unsigned int(16)	tierID;
	}
}
*/
public partial class SubTrackTierBox : FullBox
{
	public const string TYPE = "stti";
	public override string DisplayName { get { return "SubTrackTierBox"; } }

	protected ushort item_count; 
	public ushort ItemCount { get { return this.item_count; } set { this.item_count = value; } }

	protected ushort[] tierID; 
	public ushort[] TierID { get { return this.tierID; } set { this.tierID = value; } }

	public SubTrackTierBox(): base(IsoStream.FromFourCC("stti"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_count, "item_count"); 

		this.tierID = new ushort[IsoStream.GetInt( item_count)];
		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tierID[i], "tierID"); 
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
			boxSize += stream.WriteUInt16( this.tierID[i], "tierID"); 
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
			boxSize += 16; // tierID
		}
		return boxSize;
	}
}

}
