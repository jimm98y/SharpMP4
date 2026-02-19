using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ProjectionInformationBox extends FullBox('prji', 0, 0) { 
unsigned int(32) projection_kind; 
// a FourCC for the kind of projection 
}
*/
public partial class ProjectionInformationBox : FullBox
{
	public const string TYPE = "prji";
	public override string DisplayName { get { return "ProjectionInformationBox"; } }

	protected uint projection_kind;  //  a FourCC for the kind of projection 
	public uint ProjectionKind { get { return this.projection_kind; } set { this.projection_kind = value; } }

	public ProjectionInformationBox(): base(IsoStream.FromFourCC("prji"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_kind, "projection_kind"); // a FourCC for the kind of projection 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.projection_kind, "projection_kind"); // a FourCC for the kind of projection 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // projection_kind
		return boxSize;
	}
}

}
