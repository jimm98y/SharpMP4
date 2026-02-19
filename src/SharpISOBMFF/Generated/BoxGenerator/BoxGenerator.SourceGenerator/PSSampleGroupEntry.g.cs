using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class PSSampleGroupEntry() extends VisualSampleGroupEntry ('pss1')
{
	unsigned int(1) sps_present;
	unsigned int(1) pps_present;
	unsigned int(1) aps_present;
	bit(5) reserved = 0;
}
*/
public partial class PSSampleGroupEntry : VisualSampleGroupEntry
{
	public const string TYPE = "pss1";
	public override string DisplayName { get { return "PSSampleGroupEntry"; } }

	protected bool sps_present; 
	public bool SpsPresent { get { return this.sps_present; } set { this.sps_present = value; } }

	protected bool pps_present; 
	public bool PpsPresent { get { return this.pps_present; } set { this.pps_present = value; } }

	protected bool aps_present; 
	public bool ApsPresent { get { return this.aps_present; } set { this.aps_present = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public PSSampleGroupEntry(): base(IsoStream.FromFourCC("pss1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.sps_present, "sps_present"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.pps_present, "pps_present"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.aps_present, "aps_present"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.sps_present, "sps_present"); 
		boxSize += stream.WriteBit( this.pps_present, "pps_present"); 
		boxSize += stream.WriteBit( this.aps_present, "aps_present"); 
		boxSize += stream.WriteBits(5,  this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // sps_present
		boxSize += 1; // pps_present
		boxSize += 1; // aps_present
		boxSize += 5; // reserved
		return boxSize;
	}
}

}
