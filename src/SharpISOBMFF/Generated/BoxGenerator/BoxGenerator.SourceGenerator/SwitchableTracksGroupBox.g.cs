using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SwitchableTracksGroupBox extends EntityToGroupBox('swtk',0,0)
{
	for (i = 0; i < num_entities_in_group; i++)
		unsigned int(16) track_switch_hierarchy_id[i];
}
*/
public partial class SwitchableTracksGroupBox : EntityToGroupBox
{
	public const string TYPE = "swtk";
	public override string DisplayName { get { return "SwitchableTracksGroupBox"; } }

	protected ushort[] track_switch_hierarchy_id; 
	public ushort[] TrackSwitchHierarchyId { get { return this.track_switch_hierarchy_id; } set { this.track_switch_hierarchy_id = value; } }

	public SwitchableTracksGroupBox(): base(IsoStream.FromFourCC("swtk"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		this.track_switch_hierarchy_id = new ushort[IsoStream.GetInt( num_entities_in_group)];
		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.track_switch_hierarchy_id[i], "track_switch_hierarchy_id"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += stream.WriteUInt16( this.track_switch_hierarchy_id[i], "track_switch_hierarchy_id"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += 16; // track_switch_hierarchy_id
		}
		return boxSize;
	}
}

}
