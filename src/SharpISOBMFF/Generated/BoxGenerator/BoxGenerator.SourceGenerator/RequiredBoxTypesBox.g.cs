using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class RequiredBoxTypesBox() extends FullBox ('must', 0, 0) {
	unsigned int(32) required_box_types[];
}
*/
public partial class RequiredBoxTypesBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class RequiredBoxTypesBox() extends FullBox ('must', 0, 0) {
	unsigned int(32) required_box_types[];
}"; } }
	public const string TYPE = "must";
	public override string DisplayName { get { return "RequiredBoxTypesBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("vexu", "eyes", "proj", "pack", "lnsc", "lens", "lnex", "cams", "cmfy", "cxfm"); }


	protected uint[] required_box_types = []; 
	public uint[] RequiredBoxTypes { get { return this.required_box_types; } set { this.required_box_types = value; } }

	public RequiredBoxTypesBox(): base(IsoStream.FromFourCC("must"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.required_box_types, "required_box_types"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32ArrayTillEnd( this.required_box_types, "required_box_types"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)required_box_types.Length * 32); // required_box_types
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("required_box_types", ((ulong)required_box_types.Length * 32), this.required_box_types); 
		return boxSize;
	}
}

}
