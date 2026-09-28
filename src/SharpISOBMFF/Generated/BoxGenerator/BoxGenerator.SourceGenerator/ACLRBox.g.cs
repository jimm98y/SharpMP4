using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Avid's color range, as FFmpeg's mov_write_avid_tag writes it: 1 video range, 2 full range
aligned(8) class ACLRBox() extends Box('ACLR') {
	unsigned int(32) tag;
	unsigned int(32) version;
	unsigned int(32) colorRange;
	unsigned int(32) reserved;
}
*/
public partial class ACLRBox : Box
{
	public const string TYPE = "ACLR";
	public override string DisplayName { get { return "ACLRBox"; } }

	protected uint tag; 
	public uint Tag { get { return this.tag; } set { this.tag = value; } }

	protected uint version; 
	public uint Version { get { return this.version; } set { this.version = value; } }

	protected uint colorRange; 
	public uint ColorRange { get { return this.colorRange; } set { this.colorRange = value; } }

	protected uint reserved; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public ACLRBox(): base(IsoStream.FromFourCC("ACLR"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tag, "tag"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.version, "version"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.colorRange, "colorRange"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.tag, "tag"); 
		boxSize += stream.WriteUInt32( this.version, "version"); 
		boxSize += stream.WriteUInt32( this.colorRange, "colorRange"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // tag
		boxSize += 32; // version
		boxSize += 32; // colorRange
		boxSize += 32; // reserved
		return boxSize;
	}
}

}
