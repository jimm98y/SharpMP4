using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class OperatingPointSelectorProperty extends ItemProperty('a1op') {
    unsigned int(8) op_index;
}

*/
public partial class OperatingPointSelectorProperty : ItemProperty
{
	public const string TYPE = "a1op";
	public override string DisplayName { get { return "OperatingPointSelectorProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected byte op_index; 
	public byte OpIndex { get { return this.op_index; } set { this.op_index = value; } }

	public OperatingPointSelectorProperty(): base(IsoStream.FromFourCC("a1op"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.op_index, "op_index"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.op_index, "op_index"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // op_index
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("op_index", 8, this.op_index); 
		return boxSize;
	}
}

}
