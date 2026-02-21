using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackSelectionBox
	extends FullBox('tsel', version = 0, 0) {
	template int(32) switch_group = 0;
	unsigned int(32) attribute_list[];		// to end of the box
}
*/
public partial class TrackSelectionBox : FullBox
{
	public const string TYPE = "tsel";
	public override string DisplayName { get { return "TrackSelectionBox"; } }

	protected int switch_group = 0; 
	public int SwitchGroup { get { return this.switch_group; } set { this.switch_group = value; } }

	protected uint[] attribute_list;  //  to end of the box
	public uint[] AttributeList { get { return this.attribute_list; } set { this.attribute_list = value; } }

	public TrackSelectionBox(): base(IsoStream.FromFourCC("tsel"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.switch_group, "switch_group"); 
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.attribute_list, "attribute_list"); // to end of the box
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.switch_group, "switch_group"); 
		boxSize += stream.WriteUInt32ArrayTillEnd( this.attribute_list, "attribute_list"); // to end of the box
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // switch_group
		boxSize += ((ulong)attribute_list.Length * 32); // attribute_list
		return boxSize;
	}
}

}
