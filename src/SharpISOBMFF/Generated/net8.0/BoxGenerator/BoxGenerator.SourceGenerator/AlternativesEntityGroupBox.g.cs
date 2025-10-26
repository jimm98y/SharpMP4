using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AlternativesEntityGroupBox() extends TrackGroupTypeBox('altr') {
	 unsigned int(32) entity_count;
 unsigned int(32) entity_ids[]; 
 } 
*/
public partial class AlternativesEntityGroupBox : TrackGroupTypeBox
{
	public const string TYPE = "altr";
	public override string DisplayName { get { return "AlternativesEntityGroupBox"; } }

	protected uint entity_count; 
	public uint EntityCount { get { return this.entity_count; } set { this.entity_count = value; } }

	protected uint[] entity_ids; 
	public uint[] EntityIds { get { return this.entity_ids; } set { this.entity_ids = value; } }

	public AlternativesEntityGroupBox(): base(IsoStream.FromFourCC("altr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entity_count, "entity_count"); 
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.entity_ids, "entity_ids"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entity_count, "entity_count"); 
		boxSize += stream.WriteUInt32ArrayTillEnd( this.entity_ids, "entity_ids"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entity_count
		boxSize += ((ulong)entity_ids.Length * 32); // entity_ids
		return boxSize;
	}
}

}
