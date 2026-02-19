using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CubemapProjection extends ProjectionDataBox('cbmp', 0, 0) {
    unsigned int(32) layout;
    unsigned int(32) projection_padding;
 }

*/
public partial class CubemapProjection : ProjectionDataBox
{
	public const string TYPE = "cbmp";
	public override string DisplayName { get { return "CubemapProjection"; } }

	protected uint layout; 
	public uint Layout { get { return this.layout; } set { this.layout = value; } }

	protected uint projection_padding; 
	public uint ProjectionPadding { get { return this.projection_padding; } set { this.projection_padding = value; } }

	public CubemapProjection(): base(IsoStream.FromFourCC("cbmp"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.layout, "layout"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_padding, "projection_padding"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.layout, "layout"); 
		boxSize += stream.WriteUInt32( this.projection_padding, "projection_padding"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // layout
		boxSize += 32; // projection_padding
		return boxSize;
	}
}

}
