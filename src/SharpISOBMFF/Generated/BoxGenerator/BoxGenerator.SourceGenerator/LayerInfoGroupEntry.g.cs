using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class LayerInfoGroupEntry extends VisualSampleGroupEntry ('linf') {
	bit(2) reserved = 0;
	unsigned int(6) num_layers_in_track;
	for (i=0; i<num_layers_in_track; i++) {
		bit(2) reserved = 0;
		unsigned int(1) irap_gdr_pics_in_layer_only_flag;
		unsigned int(1) completeness_flag;
		unsigned int(6) layer_id;
		unsigned int(3) min_TemporalId;
		unsigned int(3) max_TemporalId;
		bit(1) reserved = 0;
		unsigned int(7) sub_layer_presence_flags;
	}
}
*/
public partial class LayerInfoGroupEntry : VisualSampleGroupEntry
{
	public const string TYPE = "linf";
	public override string DisplayName { get { return "LayerInfoGroupEntry"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte num_layers_in_track; 
	public byte NumLayersInTrack { get { return this.num_layers_in_track; } set { this.num_layers_in_track = value; } }

	protected byte[] reserved0; 
	public byte[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected bool[] irap_gdr_pics_in_layer_only_flag; 
	public bool[] IrapGdrPicsInLayerOnlyFlag { get { return this.irap_gdr_pics_in_layer_only_flag; } set { this.irap_gdr_pics_in_layer_only_flag = value; } }

	protected bool[] completeness_flag; 
	public bool[] CompletenessFlag { get { return this.completeness_flag; } set { this.completeness_flag = value; } }

	protected byte[] layer_id; 
	public byte[] LayerId { get { return this.layer_id; } set { this.layer_id = value; } }

	protected byte[] min_TemporalId; 
	public byte[] MinTemporalId { get { return this.min_TemporalId; } set { this.min_TemporalId = value; } }

	protected byte[] max_TemporalId; 
	public byte[] MaxTemporalId { get { return this.max_TemporalId; } set { this.max_TemporalId = value; } }

	protected bool[] reserved00; 
	public bool[] Reserved00 { get { return this.reserved00; } set { this.reserved00 = value; } }

	protected byte[] sub_layer_presence_flags; 
	public byte[] SubLayerPresenceFlags { get { return this.sub_layer_presence_flags; } set { this.sub_layer_presence_flags = value; } }

	public LayerInfoGroupEntry(): base(IsoStream.FromFourCC("linf"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.num_layers_in_track, "num_layers_in_track"); 

		this.reserved0 = new byte[IsoStream.GetInt(num_layers_in_track)];
		this.irap_gdr_pics_in_layer_only_flag = new bool[IsoStream.GetInt(num_layers_in_track)];
		this.completeness_flag = new bool[IsoStream.GetInt(num_layers_in_track)];
		this.layer_id = new byte[IsoStream.GetInt(num_layers_in_track)];
		this.min_TemporalId = new byte[IsoStream.GetInt(num_layers_in_track)];
		this.max_TemporalId = new byte[IsoStream.GetInt(num_layers_in_track)];
		this.reserved00 = new bool[IsoStream.GetInt(num_layers_in_track)];
		this.sub_layer_presence_flags = new byte[IsoStream.GetInt(num_layers_in_track)];
		for (int i=0; i<num_layers_in_track; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved0[i], "reserved0"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.irap_gdr_pics_in_layer_only_flag[i], "irap_gdr_pics_in_layer_only_flag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.completeness_flag[i], "completeness_flag"); 
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.layer_id[i], "layer_id"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.min_TemporalId[i], "min_TemporalId"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.max_TemporalId[i], "max_TemporalId"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved00[i], "reserved00"); 
			boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.sub_layer_presence_flags[i], "sub_layer_presence_flags"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(2,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(6,  this.num_layers_in_track, "num_layers_in_track"); 

		for (int i=0; i<num_layers_in_track; i++)
		{
			boxSize += stream.WriteBits(2,  this.reserved0[i], "reserved0"); 
			boxSize += stream.WriteBit( this.irap_gdr_pics_in_layer_only_flag[i], "irap_gdr_pics_in_layer_only_flag"); 
			boxSize += stream.WriteBit( this.completeness_flag[i], "completeness_flag"); 
			boxSize += stream.WriteBits(6,  this.layer_id[i], "layer_id"); 
			boxSize += stream.WriteBits(3,  this.min_TemporalId[i], "min_TemporalId"); 
			boxSize += stream.WriteBits(3,  this.max_TemporalId[i], "max_TemporalId"); 
			boxSize += stream.WriteBit( this.reserved00[i], "reserved00"); 
			boxSize += stream.WriteBits(7,  this.sub_layer_presence_flags[i], "sub_layer_presence_flags"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 2; // reserved
		boxSize += 6; // num_layers_in_track

		for (int i=0; i<num_layers_in_track; i++)
		{
			boxSize += 2; // reserved0
			boxSize += 1; // irap_gdr_pics_in_layer_only_flag
			boxSize += 1; // completeness_flag
			boxSize += 6; // layer_id
			boxSize += 3; // min_TemporalId
			boxSize += 3; // max_TemporalId
			boxSize += 1; // reserved00
			boxSize += 7; // sub_layer_presence_flags
		}
		return boxSize;
	}
}

}
