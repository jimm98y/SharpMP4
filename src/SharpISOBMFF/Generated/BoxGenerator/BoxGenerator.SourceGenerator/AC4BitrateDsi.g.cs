using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AC4BitrateDsi() {
 bit(2) bit_rate_mode;
 bit(32) bit_rate;
 bit(32) bit_rate_precision;
}
// a presentation of ac4_dsi_v1(): its version, and its pres_bytes (pres_bytes += add_pres_bytes). Version 1's content

*/
public partial class AC4BitrateDsi : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AC4BitrateDsi"; } }

	protected byte bit_rate_mode; 
	public byte BitRateMode { get { return this.bit_rate_mode; } set { this.bit_rate_mode = value; } }

	protected uint bit_rate; 
	public uint BitRate { get { return this.bit_rate; } set { this.bit_rate = value; } }

	protected uint bit_rate_precision; 
	public uint BitRatePrecision { get { return this.bit_rate_precision; } set { this.bit_rate_precision = value; } }

	public AC4BitrateDsi(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.bit_rate_mode, "bit_rate_mode"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bit_rate, "bit_rate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bit_rate_precision, "bit_rate_precision"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(2,  this.bit_rate_mode, "bit_rate_mode"); 
		boxSize += stream.WriteUInt32( this.bit_rate, "bit_rate"); 
		boxSize += stream.WriteUInt32( this.bit_rate_precision, "bit_rate_precision"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 2; // bit_rate_mode
		boxSize += 32; // bit_rate
		boxSize += 32; // bit_rate_precision
		return boxSize;
	}
}

}
