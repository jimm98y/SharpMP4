using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleProductionApertureDimensionsBox() extends FullBox('prof') {
 unsigned int(32) width;
 unsigned int(32) height;
 } 
*/
public partial class AppleProductionApertureDimensionsBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AppleProductionApertureDimensionsBox() extends FullBox('prof') {
 unsigned int(32) width;
 unsigned int(32) height;
 } "; } }
	public const string TYPE = "prof";
	public override string DisplayName { get { return "AppleProductionApertureDimensionsBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("tapt"); }


	protected uint width; 
	public uint Width { get { return this.width; } set { this.width = value; } }

	protected uint height; 
	public uint Height { get { return this.height; } set { this.height = value; } }

	public AppleProductionApertureDimensionsBox(): base(IsoStream.FromFourCC("prof"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.width, "width"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.height, "height"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.width, "width"); 
		boxSize += stream.WriteUInt32( this.height, "height"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // width
		boxSize += 32; // height
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("width", 32, this.width); 
		boxSize += tracer.Field("height", 32, this.height); 
		return boxSize;
	}
}

}
