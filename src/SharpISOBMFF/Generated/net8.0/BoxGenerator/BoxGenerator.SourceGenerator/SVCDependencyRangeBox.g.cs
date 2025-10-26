using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class SVCDependencyRangeBox extends Box('svdr') {
	unsigned int(3) min_dependency_id;
	unsigned int(3) min_temporal_id;
	bit(6) reserved = 0;
	unsigned int(4) min_quality_id;
	unsigned int(3) max_dependency_id;
	unsigned int(3) max_temporal_id;
	bit(6) reserved = 0;
	unsigned int(4) max_quality_id;
}
*/
public partial class SVCDependencyRangeBox : Box
{
	public const string TYPE = "svdr";
	public override string DisplayName { get { return "SVCDependencyRangeBox"; } }

	protected byte min_dependency_id; 
	public byte MinDependencyId { get { return this.min_dependency_id; } set { this.min_dependency_id = value; } }

	protected byte min_temporal_id; 
	public byte MinTemporalId { get { return this.min_temporal_id; } set { this.min_temporal_id = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte min_quality_id; 
	public byte MinQualityId { get { return this.min_quality_id; } set { this.min_quality_id = value; } }

	protected byte max_dependency_id; 
	public byte MaxDependencyId { get { return this.max_dependency_id; } set { this.max_dependency_id = value; } }

	protected byte max_temporal_id; 
	public byte MaxTemporalId { get { return this.max_temporal_id; } set { this.max_temporal_id = value; } }

	protected byte reserved0 = 0; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte max_quality_id; 
	public byte MaxQualityId { get { return this.max_quality_id; } set { this.max_quality_id = value; } }

	public SVCDependencyRangeBox(): base(IsoStream.FromFourCC("svdr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.min_dependency_id, "min_dependency_id"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.min_temporal_id, "min_temporal_id"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.min_quality_id, "min_quality_id"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.max_dependency_id, "max_dependency_id"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.max_temporal_id, "max_temporal_id"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.max_quality_id, "max_quality_id"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(3,  this.min_dependency_id, "min_dependency_id"); 
		boxSize += stream.WriteBits(3,  this.min_temporal_id, "min_temporal_id"); 
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(4,  this.min_quality_id, "min_quality_id"); 
		boxSize += stream.WriteBits(3,  this.max_dependency_id, "max_dependency_id"); 
		boxSize += stream.WriteBits(3,  this.max_temporal_id, "max_temporal_id"); 
		boxSize += stream.WriteBits(6,  this.reserved0, "reserved0"); 
		boxSize += stream.WriteBits(4,  this.max_quality_id, "max_quality_id"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 3; // min_dependency_id
		boxSize += 3; // min_temporal_id
		boxSize += 6; // reserved
		boxSize += 4; // min_quality_id
		boxSize += 3; // max_dependency_id
		boxSize += 3; // max_temporal_id
		boxSize += 6; // reserved0
		boxSize += 4; // max_quality_id
		return boxSize;
	}
}

}
