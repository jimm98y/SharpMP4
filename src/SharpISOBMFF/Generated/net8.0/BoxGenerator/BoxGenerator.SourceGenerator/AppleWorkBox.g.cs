﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleWorkBox() extends Box('©wrk') {
 Box boxes[];
 } 
*/
public partial class AppleWorkBox : Box
{
	public const string TYPE = "©wrk";
	public override string DisplayName { get { return "AppleWorkBox"; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public AppleWorkBox(): base(IsoStream.FromFourCC("©wrk"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
