using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ReceivedRtpHintSampleEntry() extends HintSampleEntry ('rrtp') {
	uint(16)		hinttrackversion = 1;
	uint(16)		highestcompatibleversion = 1;
	uint(32)		maxpacketsize;
}
*/
public partial class ReceivedRtpHintSampleEntry : HintSampleEntry
{
	public const string TYPE = "rrtp";
	public override string DisplayName { get { return "ReceivedRtpHintSampleEntry"; } }

	protected ushort hinttrackversion = 1; 
	public ushort Hinttrackversion { get { return this.hinttrackversion; } set { this.hinttrackversion = value; } }

	protected ushort highestcompatibleversion = 1; 
	public ushort Highestcompatibleversion { get { return this.highestcompatibleversion; } set { this.highestcompatibleversion = value; } }

	protected uint maxpacketsize; 
	public uint Maxpacketsize { get { return this.maxpacketsize; } set { this.maxpacketsize = value; } }

	public ReceivedRtpHintSampleEntry(): base(IsoStream.FromFourCC("rrtp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.hinttrackversion, "hinttrackversion"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.highestcompatibleversion, "highestcompatibleversion"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxpacketsize, "maxpacketsize"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.hinttrackversion, "hinttrackversion"); 
		boxSize += stream.WriteUInt16( this.highestcompatibleversion, "highestcompatibleversion"); 
		boxSize += stream.WriteUInt32( this.maxpacketsize, "maxpacketsize"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // hinttrackversion
		boxSize += 16; // highestcompatibleversion
		boxSize += 32; // maxpacketsize
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
