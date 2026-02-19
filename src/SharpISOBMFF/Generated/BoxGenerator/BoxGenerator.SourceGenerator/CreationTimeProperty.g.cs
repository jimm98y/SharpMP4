using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CreationTimeProperty
extends ItemFullProperty('crtt', version = 0, flags = 0) {
	unsigned int(64)  creation_time;
}

*/
public partial class CreationTimeProperty : ItemFullProperty
{
	public const string TYPE = "crtt";
	public override string DisplayName { get { return "CreationTimeProperty"; } }

	protected ulong creation_time; 
	public ulong CreationTime { get { return this.creation_time; } set { this.creation_time = value; } }

	public CreationTimeProperty(): base(IsoStream.FromFourCC("crtt"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.creation_time, "creation_time"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.creation_time, "creation_time"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // creation_time
		return boxSize;
	}
}

}
