using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FDSessionGroupBox extends Box('segr') {
	unsigned int(16)	num_session_groups;
	for(i=0; i < num_session_groups; i++) {
		unsigned int(8)	entry_count;
		for (j=0; j < entry_count; j++) {
			unsigned int(32)	group_ID;
		}
		unsigned int(16) num_channels_in_session_group;
		for(k=0; k < num_channels_in_session_group; k++) {
			unsigned int(32) hint_track_ID;
		}
	}
}
*/
public partial class FDSessionGroupBox : Box
{
	public const string TYPE = "segr";
	public override string DisplayName { get { return "FDSessionGroupBox"; } }

	protected ushort num_session_groups; 
	public ushort NumSessionGroups { get { return this.num_session_groups; } set { this.num_session_groups = value; } }

	protected byte[] entry_count; 
	public byte[] EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[][] group_ID; 
	public uint[][] GroupID { get { return this.group_ID; } set { this.group_ID = value; } }

	protected ushort[] num_channels_in_session_group; 
	public ushort[] NumChannelsInSessionGroup { get { return this.num_channels_in_session_group; } set { this.num_channels_in_session_group = value; } }

	protected uint[][] hint_track_ID; 
	public uint[][] HintTrackID { get { return this.hint_track_ID; } set { this.hint_track_ID = value; } }

	public FDSessionGroupBox(): base(IsoStream.FromFourCC("segr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_session_groups, "num_session_groups"); 

		this.entry_count = new byte[IsoStream.GetInt( num_session_groups)];
		this.group_ID = new uint[IsoStream.GetInt( num_session_groups)][];
		this.num_channels_in_session_group = new ushort[IsoStream.GetInt( num_session_groups)];
		this.hint_track_ID = new uint[IsoStream.GetInt( num_session_groups)][];
		for (int i=0; i < num_session_groups; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.entry_count[i], "entry_count"); 

			this.group_ID[i] = new uint[IsoStream.GetInt( entry_count[i])];
			for (int j=0; j < entry_count[i]; j++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.group_ID[i][j], "group_ID"); 
			}
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_channels_in_session_group[i], "num_channels_in_session_group"); 

			this.hint_track_ID[i] = new uint[IsoStream.GetInt( num_channels_in_session_group[i])];
			for (int k=0; k < num_channels_in_session_group[i]; k++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.hint_track_ID[i][k], "hint_track_ID"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.num_session_groups, "num_session_groups"); 

		for (int i=0; i < num_session_groups; i++)
		{
			boxSize += stream.WriteUInt8( this.entry_count[i], "entry_count"); 

			for (int j=0; j < entry_count[i]; j++)
			{
				boxSize += stream.WriteUInt32( this.group_ID[i][j], "group_ID"); 
			}
			boxSize += stream.WriteUInt16( this.num_channels_in_session_group[i], "num_channels_in_session_group"); 

			for (int k=0; k < num_channels_in_session_group[i]; k++)
			{
				boxSize += stream.WriteUInt32( this.hint_track_ID[i][k], "hint_track_ID"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // num_session_groups

		for (int i=0; i < num_session_groups; i++)
		{
			boxSize += 8; // entry_count

			for (int j=0; j < entry_count[i]; j++)
			{
				boxSize += 32; // group_ID
			}
			boxSize += 16; // num_channels_in_session_group

			for (int k=0; k < num_channels_in_session_group[i]; k++)
			{
				boxSize += 32; // hint_track_ID
			}
		}
		return boxSize;
	}
}

}
