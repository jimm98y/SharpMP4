using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Canon.pm, CTMD: a CR3's timed metadata; its entry lists the type and size of each record
aligned(8) class CanonTimedMetadataSampleEntry() extends SampleEntry('CTMD') {
 unsigned int(32) recordCount;
 CanonCTMDRecordType recordTypes[recordCount];
}

*/
public partial class CanonTimedMetadataSampleEntry : SampleEntry
{
	public const string TYPE = "CTMD";
	public override string DisplayName { get { return "CanonTimedMetadataSampleEntry"; } }

	protected uint recordCount; 
	public uint RecordCount { get { return this.recordCount; } set { this.recordCount = value; } }

	protected CanonCTMDRecordType[] recordTypes; 
	public CanonCTMDRecordType[] RecordTypes { get { return this.recordTypes; } set { this.recordTypes = value; } }

	public CanonTimedMetadataSampleEntry(): base(IsoStream.FromFourCC("CTMD"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.recordCount, "recordCount"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(recordCount), () => new CanonCTMDRecordType(),  out this.recordTypes, "recordTypes"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.recordCount, "recordCount"); 
		boxSize += stream.WriteClass( this.recordTypes, "recordTypes"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // recordCount
		boxSize += IsoStream.CalculateClassSize(recordTypes); // recordTypes
		return boxSize;
	}
}

}
