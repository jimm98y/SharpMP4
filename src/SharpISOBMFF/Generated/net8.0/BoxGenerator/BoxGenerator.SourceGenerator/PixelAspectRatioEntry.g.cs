﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class PixelAspectRatioEntry() extends VisualSampleGroupEntry ('pasr'){
	unsigned int(32) hSpacing;
	unsigned int(32) vSpacing;
}
*/
public partial class PixelAspectRatioEntry : VisualSampleGroupEntry
{
	public const string TYPE = "pasr";
	public override string DisplayName { get { return "PixelAspectRatioEntry"; } }

	protected uint hSpacing; 
	public uint HSpacing { get { return this.hSpacing; } set { this.hSpacing = value; } }

	protected uint vSpacing; 
	public uint VSpacing { get { return this.vSpacing; } set { this.vSpacing = value; } }

	public PixelAspectRatioEntry(): base(IsoStream.FromFourCC("pasr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.hSpacing, "hSpacing"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vSpacing, "vSpacing"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.hSpacing, "hSpacing"); 
		boxSize += stream.WriteUInt32( this.vSpacing, "vSpacing"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // hSpacing
		boxSize += 32; // vSpacing
		return boxSize;
	}
}

}
