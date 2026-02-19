using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class VvcSubpicLayoutMapEntry() extends VisualSampleGroupEntry ('sulm') {
	unsigned int(32) groupID_info_4cc;
	unsigned int(16) entry_count_minus1;
	for(i=0; i <= entry_count_minus1; i++)
		unsigned int(16) groupID;
}
*/
public partial class VvcSubpicLayoutMapEntry : VisualSampleGroupEntry
{
	public const string TYPE = "sulm";
	public override string DisplayName { get { return "VvcSubpicLayoutMapEntry"; } }

	protected uint groupID_info_4cc; 
	public uint GroupIDInfo4cc { get { return this.groupID_info_4cc; } set { this.groupID_info_4cc = value; } }

	protected ushort entry_count_minus1; 
	public ushort EntryCountMinus1 { get { return this.entry_count_minus1; } set { this.entry_count_minus1 = value; } }

	protected ushort[] groupID; 
	public ushort[] GroupID { get { return this.groupID; } set { this.groupID = value; } }

	public VvcSubpicLayoutMapEntry(): base(IsoStream.FromFourCC("sulm"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.groupID_info_4cc, "groupID_info_4cc"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count_minus1, "entry_count_minus1"); 

		this.groupID = new ushort[IsoStream.GetInt( entry_count_minus1 + 1)];
		for (int i=0; i <= entry_count_minus1; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.groupID[i], "groupID"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.groupID_info_4cc, "groupID_info_4cc"); 
		boxSize += stream.WriteUInt16( this.entry_count_minus1, "entry_count_minus1"); 

		for (int i=0; i <= entry_count_minus1; i++)
		{
			boxSize += stream.WriteUInt16( this.groupID[i], "groupID"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // groupID_info_4cc
		boxSize += 16; // entry_count_minus1

		for (int i=0; i <= entry_count_minus1; i++)
		{
			boxSize += 16; // groupID
		}
		return boxSize;
	}
}

}
