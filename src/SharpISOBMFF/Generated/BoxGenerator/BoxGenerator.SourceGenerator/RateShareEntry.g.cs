using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class RateShareEntry() extends SampleGroupDescriptionEntry('rash') {
	unsigned int(16)	operation_point_count;
	if (operation_point_count == 1) {
		unsigned int(16)		target_rate_share;
	}
	else {
		for (i=0; i < operation_point_count; i++) {
			unsigned int(32)	available_bitrate;
			unsigned int(16)	target_rate_share;
		}
	}
	unsigned int(32)	maximum_bitrate;
	unsigned int(32)	minimum_bitrate;
	unsigned int(8)	discard_priority;
}
*/
public partial class RateShareEntry : SampleGroupDescriptionEntry
{
	public const string TYPE = "rash";
	public override string DisplayName { get { return "RateShareEntry"; } }

	protected ushort operation_point_count; 
	public ushort OperationPointCount { get { return this.operation_point_count; } set { this.operation_point_count = value; } }

	protected ushort target_rate_share; 
	public ushort TargetRateShare { get { return this.target_rate_share; } set { this.target_rate_share = value; } }

	protected uint[] available_bitrate; 
	public uint[] AvailableBitrate { get { return this.available_bitrate; } set { this.available_bitrate = value; } }

	protected ushort[] target_rate_share0; 
	public ushort[] TargetRateShare0 { get { return this.target_rate_share0; } set { this.target_rate_share0 = value; } }

	protected uint maximum_bitrate; 
	public uint MaximumBitrate { get { return this.maximum_bitrate; } set { this.maximum_bitrate = value; } }

	protected uint minimum_bitrate; 
	public uint MinimumBitrate { get { return this.minimum_bitrate; } set { this.minimum_bitrate = value; } }

	protected byte discard_priority; 
	public byte DiscardPriority { get { return this.discard_priority; } set { this.discard_priority = value; } }

	public RateShareEntry(): base(IsoStream.FromFourCC("rash"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.operation_point_count, "operation_point_count"); 

		if (operation_point_count == 1)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.target_rate_share, "target_rate_share"); 
		}

		else 
		{

			this.available_bitrate = new uint[IsoStream.GetInt( operation_point_count)];
			this.target_rate_share0 = new ushort[IsoStream.GetInt( operation_point_count)];
			for (int i=0; i < operation_point_count; i++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.available_bitrate[i], "available_bitrate"); 
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.target_rate_share0[i], "target_rate_share0"); 
			}
		}
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maximum_bitrate, "maximum_bitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.minimum_bitrate, "minimum_bitrate"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.discard_priority, "discard_priority"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.operation_point_count, "operation_point_count"); 

		if (operation_point_count == 1)
		{
			boxSize += stream.WriteUInt16( this.target_rate_share, "target_rate_share"); 
		}

		else 
		{

			for (int i=0; i < operation_point_count; i++)
			{
				boxSize += stream.WriteUInt32( this.available_bitrate[i], "available_bitrate"); 
				boxSize += stream.WriteUInt16( this.target_rate_share0[i], "target_rate_share0"); 
			}
		}
		boxSize += stream.WriteUInt32( this.maximum_bitrate, "maximum_bitrate"); 
		boxSize += stream.WriteUInt32( this.minimum_bitrate, "minimum_bitrate"); 
		boxSize += stream.WriteUInt8( this.discard_priority, "discard_priority"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // operation_point_count

		if (operation_point_count == 1)
		{
			boxSize += 16; // target_rate_share
		}

		else 
		{

			for (int i=0; i < operation_point_count; i++)
			{
				boxSize += 32; // available_bitrate
				boxSize += 16; // target_rate_share0
			}
		}
		boxSize += 32; // maximum_bitrate
		boxSize += 32; // minimum_bitrate
		boxSize += 8; // discard_priority
		return boxSize;
	}
}

}
