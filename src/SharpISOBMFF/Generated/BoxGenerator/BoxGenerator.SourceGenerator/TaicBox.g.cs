using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TaicBox() extends FullBox('taic') {
	 unsigned int(64) time_uncertainty;
 unsigned int(32) clock_resolution;
 signed int(32) clock_drift_rate;
 unsigned int(8) clock_type;
 } 
*/
public partial class TaicBox : FullBox
{
	public const string TYPE = "taic";
	public override string DisplayName { get { return "TaicBox"; } }

	protected ulong time_uncertainty; 
	public ulong TimeUncertainty { get { return this.time_uncertainty; } set { this.time_uncertainty = value; } }

	protected uint clock_resolution; 
	public uint ClockResolution { get { return this.clock_resolution; } set { this.clock_resolution = value; } }

	protected int clock_drift_rate; 
	public int ClockDriftRate { get { return this.clock_drift_rate; } set { this.clock_drift_rate = value; } }

	protected byte clock_type; 
	public byte ClockType { get { return this.clock_type; } set { this.clock_type = value; } }

	public TaicBox(): base(IsoStream.FromFourCC("taic"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.time_uncertainty, "time_uncertainty"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.clock_resolution, "clock_resolution"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.clock_drift_rate, "clock_drift_rate"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.clock_type, "clock_type"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.time_uncertainty, "time_uncertainty"); 
		boxSize += stream.WriteUInt32( this.clock_resolution, "clock_resolution"); 
		boxSize += stream.WriteInt32( this.clock_drift_rate, "clock_drift_rate"); 
		boxSize += stream.WriteUInt8( this.clock_type, "clock_type"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // time_uncertainty
		boxSize += 32; // clock_resolution
		boxSize += 32; // clock_drift_rate
		boxSize += 8; // clock_type
		return boxSize;
	}
}

}
