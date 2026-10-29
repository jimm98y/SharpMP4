using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class PixelAspectRatioBox extends Box('pasp'){
	unsigned int(32) hSpacing;
	unsigned int(32) vSpacing;
}
*/
public partial class PixelAspectRatioBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class PixelAspectRatioBox extends Box('pasp'){
	unsigned int(32) hSpacing;
	unsigned int(32) vSpacing;
}"; } }
	public const string TYPE = "pasp";
	public override string DisplayName { get { return "PixelAspectRatioBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry", "ipco"); }


	protected uint hSpacing; 
	public uint HSpacing { get { return this.hSpacing; } set { this.hSpacing = value; } }

	protected uint vSpacing; 
	public uint VSpacing { get { return this.vSpacing; } set { this.vSpacing = value; } }

	public PixelAspectRatioBox(): base(IsoStream.FromFourCC("pasp"))
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("hSpacing", 32, this.hSpacing); 
		boxSize += tracer.Field("vSpacing", 32, this.vSpacing); 
		return boxSize;
	}
}

}
