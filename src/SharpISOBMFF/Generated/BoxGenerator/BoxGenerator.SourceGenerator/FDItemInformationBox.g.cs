using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FDItemInformationBox
	extends FullBox('fiin', version = 0, 0) {
	unsigned int(16)	entry_count;
	PartitionEntry		partition_entries[ entry_count ];
	FDSessionGroupBox	session_info;			//optional
	GroupIdToNameBox	group_id_to_name;	//optional
}
*/
public partial class FDItemInformationBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class FDItemInformationBox
	extends FullBox('fiin', version = 0, 0) {
	unsigned int(16)	entry_count;
	PartitionEntry		partition_entries[ entry_count ];
	FDSessionGroupBox	session_info;			//optional
	GroupIdToNameBox	group_id_to_name;	//optional
}"; } }
	public const string TYPE = "fiin";
	public override string DisplayName { get { return "FDItemInformationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("FullBox(meta)"); }


	protected ushort entry_count; 
	public ushort EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }
	public IEnumerable<PartitionEntry> PartitionEntries { get { return this.children.OfType<PartitionEntry>(); } }
	public FDSessionGroupBox SessionInfo { get { return this.children.OfType<FDSessionGroupBox>().FirstOrDefault(); } }
	public GroupIdToNameBox GroupIdToName { get { return this.children.OfType<GroupIdToNameBox>().FirstOrDefault(); } }

	public FDItemInformationBox(): base(IsoStream.FromFourCC("fiin"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count, "entry_count"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.partition_entries, "partition_entries"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.session_info, "session_info"); //optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.group_id_to_name, "group_id_to_name"); //optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.entry_count, "entry_count"); 
		// boxSize += stream.WriteBox( this.partition_entries, "partition_entries"); 
		// boxSize += stream.WriteBox( this.session_info, "session_info"); //optional
		// boxSize += stream.WriteBox( this.group_id_to_name, "group_id_to_name"); //optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // entry_count
		// boxSize += IsoStream.CalculateBoxSize(partition_entries); // partition_entries
		// boxSize += IsoStream.CalculateBoxSize(session_info); // session_info
		// boxSize += IsoStream.CalculateBoxSize(group_id_to_name); // group_id_to_name
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("entry_count", 16, this.entry_count); 
		// boxSize += tracer.Field("partition_entries", IsoStream.CalculateBoxSize(partition_entries), this.partition_entries); 
		// boxSize += tracer.Field("session_info", IsoStream.CalculateBoxSize(session_info), this.session_info); //optional
		// boxSize += tracer.Field("group_id_to_name", IsoStream.CalculateBoxSize(group_id_to_name), this.group_id_to_name); //optional
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
