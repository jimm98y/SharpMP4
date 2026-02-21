using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SequenceOffset() extends Box('snro') {
	int(32)		offset;
}
*/
public partial class SequenceOffset : Box
{
	public const string TYPE = "snro";
	public override string DisplayName { get { return "SequenceOffset"; } }

	protected int offset; 
	public int Offset { get { return this.offset; } set { this.offset = value; } }

	public SequenceOffset(): base(IsoStream.FromFourCC("snro"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.offset, "offset"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.offset, "offset"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // offset
		return boxSize;
	}
}

}
