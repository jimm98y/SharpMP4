using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AppleClippingRegionBox() extends Box ('crgn'){
 unsigned int(16) regionSize;
 unsigned int(64) regionBoundaryBox;
 unsigned int(8) clippingRegionData[];
 }
*/
public partial class AppleClippingRegionBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AppleClippingRegionBox() extends Box ('crgn'){
 unsigned int(16) regionSize;
 unsigned int(64) regionBoundaryBox;
 unsigned int(8) clippingRegionData[];
 }"; } }
	public const string TYPE = "crgn";
	public override string DisplayName { get { return "AppleClippingRegionBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("clip"); }


	protected ushort regionSize; 
	public ushort RegionSize { get { return this.regionSize; } set { this.regionSize = value; } }

	protected ulong regionBoundaryBox; 
	public ulong RegionBoundaryBox { get { return this.regionBoundaryBox; } set { this.regionBoundaryBox = value; } }

	protected byte[] clippingRegionData = []; 
	public byte[] ClippingRegionData { get { return this.clippingRegionData; } set { this.clippingRegionData = value; } }

	public AppleClippingRegionBox(): base(IsoStream.FromFourCC("crgn"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.regionSize, "regionSize"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.regionBoundaryBox, "regionBoundaryBox"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.clippingRegionData, "clippingRegionData"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.regionSize, "regionSize"); 
		boxSize += stream.WriteUInt64( this.regionBoundaryBox, "regionBoundaryBox"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.clippingRegionData, "clippingRegionData"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // regionSize
		boxSize += 64; // regionBoundaryBox
		boxSize += ((ulong)clippingRegionData.Length * 8); // clippingRegionData
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("regionSize", 16, this.regionSize); 
		boxSize += tracer.Field("regionBoundaryBox", 64, this.regionBoundaryBox); 
		boxSize += tracer.Field("clippingRegionData", ((ulong)clippingRegionData.Length * 8), this.clippingRegionData); 
		return boxSize;
	}
}

}
