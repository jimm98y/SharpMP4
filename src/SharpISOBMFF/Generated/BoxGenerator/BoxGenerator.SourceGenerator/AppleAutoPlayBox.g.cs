using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleAutoPlayBox() extends Box('play') {
	 unsigned int(8) autoplay;
 } 
*/
public partial class AppleAutoPlayBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AppleAutoPlayBox() extends Box('play') {
	 unsigned int(8) autoplay;
 } "; } }
	public const string TYPE = "play";
	public override string DisplayName { get { return "AppleAutoPlayBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected byte autoplay; 
	public byte Autoplay { get { return this.autoplay; } set { this.autoplay = value; } }

	public AppleAutoPlayBox(): base(IsoStream.FromFourCC("play"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.autoplay, "autoplay"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.autoplay, "autoplay"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // autoplay
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("autoplay", 8, this.autoplay); 
		return boxSize;
	}
}

}
