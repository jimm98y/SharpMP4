using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackReferenceTypeBox() extends Box('vdep') {
 unsigned int(32) trackIds[];
 } 
*/
public partial class TrackReferenceTypeBoxvdepDup : Box
{
	public const string TYPE = "vdep";
	public override string DisplayName { get { return "TrackReferenceTypeBoxvdepDup"; } }

	protected uint[] trackIds; 
	public uint[] TrackIds { get { return this.trackIds; } set { this.trackIds = value; } }

	public TrackReferenceTypeBoxvdepDup(): base(IsoStream.FromFourCC("vdep"))
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
