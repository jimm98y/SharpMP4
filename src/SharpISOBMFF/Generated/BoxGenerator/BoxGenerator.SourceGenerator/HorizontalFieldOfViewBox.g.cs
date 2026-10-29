using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HorizontalFieldOfViewBox extends Box('hfov') { 
unsigned int(32) field_of_view; 
} 
*/
public partial class HorizontalFieldOfViewBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class HorizontalFieldOfViewBox extends Box('hfov') { 
unsigned int(32) field_of_view; 
} "; } }
	public const string TYPE = "hfov";
	public override string DisplayName { get { return "HorizontalFieldOfViewBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry"); }


	protected uint field_of_view; 
	public uint FieldOfView { get { return this.field_of_view; } set { this.field_of_view = value; } }

	public HorizontalFieldOfViewBox(): base(IsoStream.FromFourCC("hfov"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.field_of_view, "field_of_view"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.field_of_view, "field_of_view"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // field_of_view
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("field_of_view", 32, this.field_of_view); 
		return boxSize;
	}
}

}
