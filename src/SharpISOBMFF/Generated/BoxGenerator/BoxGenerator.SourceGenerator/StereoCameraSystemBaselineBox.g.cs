using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class StereoCameraSystemBaselineBox extends FullBox('blin') {
  unsigned int(32) baseline_value;
 }

*/
public partial class StereoCameraSystemBaselineBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class StereoCameraSystemBaselineBox extends FullBox('blin') {
  unsigned int(32) baseline_value;
 }
"; } }
	public const string TYPE = "blin";
	public override string DisplayName { get { return "StereoCameraSystemBaselineBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("cams"); }


	protected uint baseline_value; 
	public uint BaselineValue { get { return this.baseline_value; } set { this.baseline_value = value; } }

	public StereoCameraSystemBaselineBox(): base(IsoStream.FromFourCC("blin"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.baseline_value, "baseline_value"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.baseline_value, "baseline_value"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // baseline_value
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("baseline_value", 32, this.baseline_value); 
		return boxSize;
	}
}

}
