using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleWindowLocationBox() extends Box('WLOC') {
	 signed int(16) locationX;
 signed int(16) locationY;
 } 
*/
public partial class AppleWindowLocationBox : Box
{
	public const string TYPE = "WLOC";
	public override string DisplayName { get { return "AppleWindowLocationBox"; } }

	protected short locationX; 
	public short LocationX { get { return this.locationX; } set { this.locationX = value; } }

	protected short locationY; 
	public short LocationY { get { return this.locationY; } set { this.locationY = value; } }

	public AppleWindowLocationBox(): base(IsoStream.FromFourCC("WLOC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.locationX, "locationX"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.locationY, "locationY"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt16( this.locationX, "locationX"); 
		boxSize += stream.WriteInt16( this.locationY, "locationY"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // locationX
		boxSize += 16; // locationY
		return boxSize;
	}
}

}
