using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class EquirectangularProjection extends ProjectionDataBox('equi', 0, 0) {
    unsigned int(32) projection_bounds_top;
    unsigned int(32) projection_bounds_bottom;
    unsigned int(32) projection_bounds_left;
    unsigned int(32) projection_bounds_right;
 }

*/
public partial class EquirectangularProjection : ProjectionDataBox
{
	public const string TYPE = "equi";
	public override string DisplayName { get { return "EquirectangularProjection"; } }

	protected uint projection_bounds_top; 
	public uint ProjectionBoundsTop { get { return this.projection_bounds_top; } set { this.projection_bounds_top = value; } }

	protected uint projection_bounds_bottom; 
	public uint ProjectionBoundsBottom { get { return this.projection_bounds_bottom; } set { this.projection_bounds_bottom = value; } }

	protected uint projection_bounds_left; 
	public uint ProjectionBoundsLeft { get { return this.projection_bounds_left; } set { this.projection_bounds_left = value; } }

	protected uint projection_bounds_right; 
	public uint ProjectionBoundsRight { get { return this.projection_bounds_right; } set { this.projection_bounds_right = value; } }

	public EquirectangularProjection(): base(IsoStream.FromFourCC("equi"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_bounds_top, "projection_bounds_top"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_bounds_bottom, "projection_bounds_bottom"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_bounds_left, "projection_bounds_left"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_bounds_right, "projection_bounds_right"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.projection_bounds_top, "projection_bounds_top"); 
		boxSize += stream.WriteUInt32( this.projection_bounds_bottom, "projection_bounds_bottom"); 
		boxSize += stream.WriteUInt32( this.projection_bounds_left, "projection_bounds_left"); 
		boxSize += stream.WriteUInt32( this.projection_bounds_right, "projection_bounds_right"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // projection_bounds_top
		boxSize += 32; // projection_bounds_bottom
		boxSize += 32; // projection_bounds_left
		boxSize += 32; // projection_bounds_right
		return boxSize;
	}
}

}
