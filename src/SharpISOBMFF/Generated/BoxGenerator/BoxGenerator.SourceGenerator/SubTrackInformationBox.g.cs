using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SubTrackInformationBox
	extends FullBox('stri', version = 0, 0){
	template int(16)	switch_group = 0;
	template int(16)	alternate_group = 0;
	template unsigned int(32)	sub_track_ID = 0;
	unsigned int(32)	attribute_list[];	// to the end of the box
}
*/
public partial class SubTrackInformationBox : FullBox
{
	public const string TYPE = "stri";
	public override string DisplayName { get { return "SubTrackInformationBox"; } }

	protected short switch_group = 0; 
	public short SwitchGroup { get { return this.switch_group; } set { this.switch_group = value; } }

	protected short alternate_group = 0; 
	public short AlternateGroup { get { return this.alternate_group; } set { this.alternate_group = value; } }

	protected uint sub_track_ID = 0; 
	public uint SubTrackID { get { return this.sub_track_ID; } set { this.sub_track_ID = value; } }

	protected uint[] attribute_list;  //  to the end of the box
	public uint[] AttributeList { get { return this.attribute_list; } set { this.attribute_list = value; } }

	public SubTrackInformationBox(): base(IsoStream.FromFourCC("stri"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.switch_group, "switch_group"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.alternate_group, "alternate_group"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sub_track_ID, "sub_track_ID"); 
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.attribute_list, "attribute_list"); // to the end of the box
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt16( this.switch_group, "switch_group"); 
		boxSize += stream.WriteInt16( this.alternate_group, "alternate_group"); 
		boxSize += stream.WriteUInt32( this.sub_track_ID, "sub_track_ID"); 
		boxSize += stream.WriteUInt32ArrayTillEnd( this.attribute_list, "attribute_list"); // to the end of the box
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // switch_group
		boxSize += 16; // alternate_group
		boxSize += 32; // sub_track_ID
		boxSize += ((ulong)attribute_list.Length * 32); // attribute_list
		return boxSize;
	}
}

}
