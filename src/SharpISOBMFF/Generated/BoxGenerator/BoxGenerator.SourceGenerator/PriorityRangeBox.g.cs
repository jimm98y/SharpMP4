using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class PriorityRangeBox extends Box('svpr') {
	bit(2) reserved1 = 0;
	unsigned int(6) min_priorityId;
	bit(2) reserved2 = 0;
	unsigned int(6) max_priorityId;
}
*/
public partial class PriorityRangeBox : Box
{
	public const string TYPE = "svpr";
	public override string DisplayName { get { return "PriorityRangeBox"; } }

	protected byte reserved1 = 0; 
	public byte Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected byte min_priorityId; 
	public byte MinPriorityId { get { return this.min_priorityId; } set { this.min_priorityId = value; } }

	protected byte reserved2 = 0; 
	public byte Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected byte max_priorityId; 
	public byte MaxPriorityId { get { return this.max_priorityId; } set { this.max_priorityId = value; } }

	public PriorityRangeBox(): base(IsoStream.FromFourCC("svpr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.min_priorityId, "min_priorityId"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.max_priorityId, "max_priorityId"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(2,  this.reserved1, "reserved1"); 
		boxSize += stream.WriteBits(6,  this.min_priorityId, "min_priorityId"); 
		boxSize += stream.WriteBits(2,  this.reserved2, "reserved2"); 
		boxSize += stream.WriteBits(6,  this.max_priorityId, "max_priorityId"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 2; // reserved1
		boxSize += 6; // min_priorityId
		boxSize += 2; // reserved2
		boxSize += 6; // max_priorityId
		return boxSize;
	}
}

}
