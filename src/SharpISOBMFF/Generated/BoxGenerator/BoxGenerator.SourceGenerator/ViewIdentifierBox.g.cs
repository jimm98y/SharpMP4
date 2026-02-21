using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ViewIdentifierBox extends FullBox ('vwid', version=0, flags) 
{
	unsigned int(2) 	reserved6 = 0;
	unsigned int(3) 	min_temporal_id;
	unsigned int(3) 	max_temporal_id;
	unsigned int(16)	num_views;
	for (i=0; i<num_views; i++) {
		unsigned int(6) 	reserved1 = 0;
		unsigned int(10) 	view_id[i];
		unsigned int(6) 	reserved2 = 0;
		unsigned int(10) 	view_order_index;
		unsigned int(1)	texture_in_stream[i];
		unsigned int(1)	texture_in_track[i];
		unsigned int(1)	depth_in_stream[i];
		unsigned int(1)	depth_in_track[i];
		unsigned int(2) 	base_view_type;
		unsigned int(10) 	num_ref_views;
		for (j = 0; j < num_ref_views; j++) {
			unsigned int(4) 	reserved5 = 0;
			unsigned int(2) 	dependent_component_idc[i][j];
			unsigned int(10) 	ref_view_id[i][j];
		}
	}
}
*/
public partial class ViewIdentifierBox : FullBox
{
	public const string TYPE = "vwid";
	public override string DisplayName { get { return "ViewIdentifierBox"; } }

	protected byte reserved6 = 0; 
	public byte Reserved6 { get { return this.reserved6; } set { this.reserved6 = value; } }

	protected byte min_temporal_id; 
	public byte MinTemporalId { get { return this.min_temporal_id; } set { this.min_temporal_id = value; } }

	protected byte max_temporal_id; 
	public byte MaxTemporalId { get { return this.max_temporal_id; } set { this.max_temporal_id = value; } }

	protected ushort num_views; 
	public ushort NumViews { get { return this.num_views; } set { this.num_views = value; } }

	protected byte[] reserved1; 
	public byte[] Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort[] view_id; 
	public ushort[] ViewId { get { return this.view_id; } set { this.view_id = value; } }

	protected byte[] reserved2; 
	public byte[] Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected ushort[] view_order_index; 
	public ushort[] ViewOrderIndex { get { return this.view_order_index; } set { this.view_order_index = value; } }

	protected bool[] texture_in_stream; 
	public bool[] TextureInStream { get { return this.texture_in_stream; } set { this.texture_in_stream = value; } }

	protected bool[] texture_in_track; 
	public bool[] TextureInTrack { get { return this.texture_in_track; } set { this.texture_in_track = value; } }

	protected bool[] depth_in_stream; 
	public bool[] DepthInStream { get { return this.depth_in_stream; } set { this.depth_in_stream = value; } }

	protected bool[] depth_in_track; 
	public bool[] DepthInTrack { get { return this.depth_in_track; } set { this.depth_in_track = value; } }

	protected byte[] base_view_type; 
	public byte[] BaseViewType { get { return this.base_view_type; } set { this.base_view_type = value; } }

	protected ushort[] num_ref_views; 
	public ushort[] NumRefViews { get { return this.num_ref_views; } set { this.num_ref_views = value; } }

	protected byte[][] reserved5; 
	public byte[][] Reserved5 { get { return this.reserved5; } set { this.reserved5 = value; } }

	protected byte[][] dependent_component_idc; 
	public byte[][] DependentComponentIdc { get { return this.dependent_component_idc; } set { this.dependent_component_idc = value; } }

	protected ushort[][] ref_view_id; 
	public ushort[][] RefViewId { get { return this.ref_view_id; } set { this.ref_view_id = value; } }

	public ViewIdentifierBox(uint flags = 0): base(IsoStream.FromFourCC("vwid"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved6, "reserved6"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.min_temporal_id, "min_temporal_id"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.max_temporal_id, "max_temporal_id"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_views, "num_views"); 

		this.reserved1 = new byte[IsoStream.GetInt(num_views)];
		this.view_id = new ushort[IsoStream.GetInt(num_views)];
		this.reserved2 = new byte[IsoStream.GetInt(num_views)];
		this.view_order_index = new ushort[IsoStream.GetInt(num_views)];
		this.texture_in_stream = new bool[IsoStream.GetInt(num_views)];
		this.texture_in_track = new bool[IsoStream.GetInt(num_views)];
		this.depth_in_stream = new bool[IsoStream.GetInt(num_views)];
		this.depth_in_track = new bool[IsoStream.GetInt(num_views)];
		this.base_view_type = new byte[IsoStream.GetInt(num_views)];
		this.num_ref_views = new ushort[IsoStream.GetInt(num_views)];
		this.reserved5 = new byte[IsoStream.GetInt(num_views)][];
		this.dependent_component_idc = new byte[IsoStream.GetInt(num_views)][];
		this.ref_view_id = new ushort[IsoStream.GetInt(num_views)][];
		for (int i=0; i<num_views; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved1[i], "reserved1"); 
			boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.view_id[i], "view_id"); 
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved2[i], "reserved2"); 
			boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.view_order_index[i], "view_order_index"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.texture_in_stream[i], "texture_in_stream"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.texture_in_track[i], "texture_in_track"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.depth_in_stream[i], "depth_in_stream"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.depth_in_track[i], "depth_in_track"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.base_view_type[i], "base_view_type"); 
			boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.num_ref_views[i], "num_ref_views"); 

			this.reserved5[i] = new byte[IsoStream.GetInt( num_ref_views[i])];
			this.dependent_component_idc[i] = new byte[IsoStream.GetInt( num_ref_views[i])];
			this.ref_view_id[i] = new ushort[IsoStream.GetInt( num_ref_views[i])];
			for (int j = 0; j < num_ref_views[i]; j++)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved5[i][j], "reserved5"); 
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dependent_component_idc[i][j], "dependent_component_idc"); 
				boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.ref_view_id[i][j], "ref_view_id"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(2,  this.reserved6, "reserved6"); 
		boxSize += stream.WriteBits(3,  this.min_temporal_id, "min_temporal_id"); 
		boxSize += stream.WriteBits(3,  this.max_temporal_id, "max_temporal_id"); 
		boxSize += stream.WriteUInt16( this.num_views, "num_views"); 

		for (int i=0; i<num_views; i++)
		{
			boxSize += stream.WriteBits(6,  this.reserved1[i], "reserved1"); 
			boxSize += stream.WriteBits(10,  this.view_id[i], "view_id"); 
			boxSize += stream.WriteBits(6,  this.reserved2[i], "reserved2"); 
			boxSize += stream.WriteBits(10,  this.view_order_index[i], "view_order_index"); 
			boxSize += stream.WriteBit( this.texture_in_stream[i], "texture_in_stream"); 
			boxSize += stream.WriteBit( this.texture_in_track[i], "texture_in_track"); 
			boxSize += stream.WriteBit( this.depth_in_stream[i], "depth_in_stream"); 
			boxSize += stream.WriteBit( this.depth_in_track[i], "depth_in_track"); 
			boxSize += stream.WriteBits(2,  this.base_view_type[i], "base_view_type"); 
			boxSize += stream.WriteBits(10,  this.num_ref_views[i], "num_ref_views"); 

			for (int j = 0; j < num_ref_views[i]; j++)
			{
				boxSize += stream.WriteBits(4,  this.reserved5[i][j], "reserved5"); 
				boxSize += stream.WriteBits(2,  this.dependent_component_idc[i][j], "dependent_component_idc"); 
				boxSize += stream.WriteBits(10,  this.ref_view_id[i][j], "ref_view_id"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 2; // reserved6
		boxSize += 3; // min_temporal_id
		boxSize += 3; // max_temporal_id
		boxSize += 16; // num_views

		for (int i=0; i<num_views; i++)
		{
			boxSize += 6; // reserved1
			boxSize += 10; // view_id
			boxSize += 6; // reserved2
			boxSize += 10; // view_order_index
			boxSize += 1; // texture_in_stream
			boxSize += 1; // texture_in_track
			boxSize += 1; // depth_in_stream
			boxSize += 1; // depth_in_track
			boxSize += 2; // base_view_type
			boxSize += 10; // num_ref_views

			for (int j = 0; j < num_ref_views[i]; j++)
			{
				boxSize += 4; // reserved5
				boxSize += 2; // dependent_component_idc
				boxSize += 10; // ref_view_id
			}
		}
		return boxSize;
	}
}

}
