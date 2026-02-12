using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class SVCPriorityLayerInfoBox extends Box('qlif'){
	unsigned int(8) pr_layer_num;
	for(j=0; j< pr_layer_num; j++){
		unsigned int(8) pr_layer;
		unsigned int(24) profile_level_idc;
		unsigned int(32) max_bitrate;
		unsigned int(32) avg_bitrate;
	}
}
*/
public partial class SVCPriorityLayerInfoBox : Box
{
	public const string TYPE = "qlif";
	public override string DisplayName { get { return "SVCPriorityLayerInfoBox"; } }

	protected byte pr_layer_num; 
	public byte PrLayerNum { get { return this.pr_layer_num; } set { this.pr_layer_num = value; } }

	protected byte[] pr_layer; 
	public byte[] PrLayer { get { return this.pr_layer; } set { this.pr_layer = value; } }

	protected uint[] profile_level_idc; 
	public uint[] ProfileLevelIdc { get { return this.profile_level_idc; } set { this.profile_level_idc = value; } }

	protected uint[] max_bitrate; 
	public uint[] MaxBitrate { get { return this.max_bitrate; } set { this.max_bitrate = value; } }

	protected uint[] avg_bitrate; 
	public uint[] AvgBitrate { get { return this.avg_bitrate; } set { this.avg_bitrate = value; } }

	public SVCPriorityLayerInfoBox(): base(IsoStream.FromFourCC("qlif"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.pr_layer_num, "pr_layer_num"); 

		this.pr_layer = new byte[IsoStream.GetInt( pr_layer_num)];
		this.profile_level_idc = new uint[IsoStream.GetInt( pr_layer_num)];
		this.max_bitrate = new uint[IsoStream.GetInt( pr_layer_num)];
		this.avg_bitrate = new uint[IsoStream.GetInt( pr_layer_num)];
		for (int j=0; j< pr_layer_num; j++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.pr_layer[j], "pr_layer"); 
			boxSize += stream.ReadUInt24(boxSize, readSize,  out this.profile_level_idc[j], "profile_level_idc"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.max_bitrate[j], "max_bitrate"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avg_bitrate[j], "avg_bitrate"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.pr_layer_num, "pr_layer_num"); 

		for (int j=0; j< pr_layer_num; j++)
		{
			boxSize += stream.WriteUInt8( this.pr_layer[j], "pr_layer"); 
			boxSize += stream.WriteUInt24( this.profile_level_idc[j], "profile_level_idc"); 
			boxSize += stream.WriteUInt32( this.max_bitrate[j], "max_bitrate"); 
			boxSize += stream.WriteUInt32( this.avg_bitrate[j], "avg_bitrate"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // pr_layer_num

		for (int j=0; j< pr_layer_num; j++)
		{
			boxSize += 8; // pr_layer
			boxSize += 24; // profile_level_idc
			boxSize += 32; // max_bitrate
			boxSize += 32; // avg_bitrate
		}
		return boxSize;
	}
}

}
