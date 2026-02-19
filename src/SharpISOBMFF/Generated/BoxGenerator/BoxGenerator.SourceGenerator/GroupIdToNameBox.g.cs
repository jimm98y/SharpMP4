using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class GroupIdToNameBox
		extends FullBox('gitn', version = 0, 0) {
	unsigned int(16)	entry_count;
	for (i=1; i <= entry_count; i++) {
		unsigned int(32)	group_ID;
		utf8string			group_name;
	}
}
*/
public partial class GroupIdToNameBox : FullBox
{
	public const string TYPE = "gitn";
	public override string DisplayName { get { return "GroupIdToNameBox"; } }

	protected ushort entry_count; 
	public ushort EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] group_ID; 
	public uint[] GroupID { get { return this.group_ID; } set { this.group_ID = value; } }

	protected BinaryUTF8String[] group_name; 
	public BinaryUTF8String[] GroupName { get { return this.group_name; } set { this.group_name = value; } }

	public GroupIdToNameBox(): base(IsoStream.FromFourCC("gitn"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.group_ID = new uint[IsoStream.GetInt( entry_count)];
		this.group_name = new BinaryUTF8String[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.group_ID[i], "group_ID"); 
			boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.group_name[i], "group_name"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.entry_count, "entry_count"); 

		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.WriteUInt32( this.group_ID[i], "group_ID"); 
			boxSize += stream.WriteStringZeroTerminated( this.group_name[i], "group_name"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // entry_count

		for (int i=0; i < entry_count; i++)
		{
			boxSize += 32; // group_ID
			boxSize += IsoStream.CalculateStringSize(group_name); // group_name
		}
		return boxSize;
	}
}

}
