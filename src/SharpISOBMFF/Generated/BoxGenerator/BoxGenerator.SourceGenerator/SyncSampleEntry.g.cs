using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SyncSampleEntry() extends VisualSampleGroupEntry ('sync')
{
		bit(2) reserved = 0;
		unsigned int(6) NAL_unit_type;
}
*/
public partial class SyncSampleEntry : VisualSampleGroupEntry
{
	public const string TYPE = "sync";
	public override string DisplayName { get { return "SyncSampleEntry"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte NAL_unit_type; 
	public byte NALUnitType { get { return this.NAL_unit_type; } set { this.NAL_unit_type = value; } }

	public SyncSampleEntry(): base(IsoStream.FromFourCC("sync"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.NAL_unit_type, "NAL_unit_type"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(2,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(6,  this.NAL_unit_type, "NAL_unit_type"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 2; // reserved
		boxSize += 6; // NAL_unit_type
		return boxSize;
	}
}

}
