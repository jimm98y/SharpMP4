using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class InitialParameterSetBox extends Box ('svip') {
	unsigned int(8) sps_id_count;
	for (i=0; i< sps_id_count; i++)
		unsigned int(8) SPS_index;
	unsigned int(8) pps_id_count;
	for (i=0; i< pps_id_count; i++)
		unsigned int(8) PPS_index;
}
*/
public partial class InitialParameterSetBox : Box
{
	public const string TYPE = "svip";
	public override string DisplayName { get { return "InitialParameterSetBox"; } }

	protected byte sps_id_count; 
	public byte SpsIdCount { get { return this.sps_id_count; } set { this.sps_id_count = value; } }

	protected byte[] SPS_index; 
	public byte[] SPSIndex { get { return this.SPS_index; } set { this.SPS_index = value; } }

	protected byte pps_id_count; 
	public byte PpsIdCount { get { return this.pps_id_count; } set { this.pps_id_count = value; } }

	protected byte[] PPS_index; 
	public byte[] PPSIndex { get { return this.PPS_index; } set { this.PPS_index = value; } }

	public InitialParameterSetBox(): base(IsoStream.FromFourCC("svip"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.sps_id_count, "sps_id_count"); 

		this.SPS_index = new byte[IsoStream.GetInt( sps_id_count)];
		for (int i=0; i< sps_id_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.SPS_index[i], "SPS_index"); 
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.pps_id_count, "pps_id_count"); 

		this.PPS_index = new byte[IsoStream.GetInt( pps_id_count)];
		for (int i=0; i< pps_id_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.PPS_index[i], "PPS_index"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.sps_id_count, "sps_id_count"); 

		for (int i=0; i< sps_id_count; i++)
		{
			boxSize += stream.WriteUInt8( this.SPS_index[i], "SPS_index"); 
		}
		boxSize += stream.WriteUInt8( this.pps_id_count, "pps_id_count"); 

		for (int i=0; i< pps_id_count; i++)
		{
			boxSize += stream.WriteUInt8( this.PPS_index[i], "PPS_index"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // sps_id_count

		for (int i=0; i< sps_id_count; i++)
		{
			boxSize += 8; // SPS_index
		}
		boxSize += 8; // pps_id_count

		for (int i=0; i< pps_id_count; i++)
		{
			boxSize += 8; // PPS_index
		}
		return boxSize;
	}
}

}
