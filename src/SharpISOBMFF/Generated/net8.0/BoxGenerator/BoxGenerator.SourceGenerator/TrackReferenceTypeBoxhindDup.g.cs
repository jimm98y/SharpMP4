﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackReferenceTypeBox() extends Box('hind') {
 unsigned int(32) trackIds[];
 } 
*/
public partial class TrackReferenceTypeBoxhindDup : Box
{
	public const string TYPE = "hind";
	public override string DisplayName { get { return "TrackReferenceTypeBoxhindDup"; } }

	protected uint[] trackIds; 
	public uint[] TrackIds { get { return this.trackIds; } set { this.trackIds = value; } }

	public TrackReferenceTypeBoxhindDup(): base(IsoStream.FromFourCC("hind"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.trackIds, "trackIds"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32ArrayTillEnd( this.trackIds, "trackIds"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)trackIds.Length * 32); // trackIds
		return boxSize;
	}
}

}
