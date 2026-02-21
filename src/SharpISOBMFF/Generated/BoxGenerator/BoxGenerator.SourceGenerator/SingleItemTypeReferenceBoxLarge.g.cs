using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SingleItemTypeReferenceBoxLarge(referenceType) extends Box(referenceType) {
	unsigned int(32) from_item_ID;
	unsigned int(16) reference_count;
	for (j=0; j<reference_count; j++) {
		unsigned int(32) to_item_ID;
	}
}
*/
public partial class SingleItemTypeReferenceBoxLarge : Box
{
	public override string DisplayName { get { return "SingleItemTypeReferenceBoxLarge"; } }

	protected uint from_item_ID; 
	public uint FromItemID { get { return this.from_item_ID; } set { this.from_item_ID = value; } }

	protected ushort reference_count; 
	public ushort ReferenceCount { get { return this.reference_count; } set { this.reference_count = value; } }

	protected uint[] to_item_ID; 
	public uint[] ToItemID { get { return this.to_item_ID; } set { this.to_item_ID = value; } }

	public SingleItemTypeReferenceBoxLarge(uint referenceType): base(referenceType)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.from_item_ID, "from_item_ID"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reference_count, "reference_count"); 

		this.to_item_ID = new uint[IsoStream.GetInt(reference_count)];
		for (int j=0; j<reference_count; j++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.to_item_ID[j], "to_item_ID"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.from_item_ID, "from_item_ID"); 
		boxSize += stream.WriteUInt16( this.reference_count, "reference_count"); 

		for (int j=0; j<reference_count; j++)
		{
			boxSize += stream.WriteUInt32( this.to_item_ID[j], "to_item_ID"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // from_item_ID
		boxSize += 16; // reference_count

		for (int j=0; j<reference_count; j++)
		{
			boxSize += 32; // to_item_ID
		}
		return boxSize;
	}
}

}
