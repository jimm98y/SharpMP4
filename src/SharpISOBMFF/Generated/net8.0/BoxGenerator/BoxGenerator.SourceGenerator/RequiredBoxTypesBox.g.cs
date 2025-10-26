using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class RequiredBoxTypesBox() extends FullBox ('must', 0, 0) {
 unsigned int(32) required_box_types[];
 }
*/
public partial class RequiredBoxTypesBox : FullBox
{
	public const string TYPE = "must";
	public override string DisplayName { get { return "RequiredBoxTypesBox"; } }

	protected uint[] required_box_types; 
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
}

}
