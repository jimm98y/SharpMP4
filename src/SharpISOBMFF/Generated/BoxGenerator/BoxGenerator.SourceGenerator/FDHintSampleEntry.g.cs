using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class FDHintSampleEntry() extends HintSampleEntry ('fdp ') {
	unsigned int(16)	hinttrackversion = 1;
	unsigned int(16)	highestcompatibleversion = 1;
	unsigned int(16)	partition_entry_ID;
	unsigned int(16)	FEC_overhead;
}
*/
public partial class FDHintSampleEntry : HintSampleEntry
{
	public const string TYPE = "fdp ";
	public override string DisplayName { get { return "FDHintSampleEntry"; } }

	protected ushort hinttrackversion = 1; 
	public ushort Hinttrackversion { get { return this.hinttrackversion; } set { this.hinttrackversion = value; } }

	protected ushort highestcompatibleversion = 1; 
	public ushort Highestcompatibleversion { get { return this.highestcompatibleversion; } set { this.highestcompatibleversion = value; } }

	protected ushort partition_entry_ID; 
	public ushort PartitionEntryID { get { return this.partition_entry_ID; } set { this.partition_entry_ID = value; } }

	protected ushort FEC_overhead; 
	public ushort FECOverhead { get { return this.FEC_overhead; } set { this.FEC_overhead = value; } }

	public FDHintSampleEntry(): base(IsoStream.FromFourCC("fdp "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.hinttrackversion, "hinttrackversion"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.highestcompatibleversion, "highestcompatibleversion"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.partition_entry_ID, "partition_entry_ID"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.FEC_overhead, "FEC_overhead"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.hinttrackversion, "hinttrackversion"); 
		boxSize += stream.WriteUInt16( this.highestcompatibleversion, "highestcompatibleversion"); 
		boxSize += stream.WriteUInt16( this.partition_entry_ID, "partition_entry_ID"); 
		boxSize += stream.WriteUInt16( this.FEC_overhead, "FEC_overhead"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // hinttrackversion
		boxSize += 16; // highestcompatibleversion
		boxSize += 16; // partition_entry_ID
		boxSize += 16; // FEC_overhead
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
