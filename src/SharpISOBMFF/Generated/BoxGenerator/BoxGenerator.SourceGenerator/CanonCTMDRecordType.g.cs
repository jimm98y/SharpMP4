using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CanonCTMDRecordType() {
 unsigned int(32) recordType;
 unsigned int(32) recordSize;
}

*/
public partial class CanonCTMDRecordType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "CanonCTMDRecordType"; } }

	protected uint recordType; 
	public uint RecordType { get { return this.recordType; } set { this.recordType = value; } }

	protected uint recordSize; 
	public uint RecordSize { get { return this.recordSize; } set { this.recordSize = value; } }

	public CanonCTMDRecordType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.recordType, "recordType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.recordSize, "recordSize"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.recordType, "recordType"); 
		boxSize += stream.WriteUInt32( this.recordSize, "recordSize"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // recordType
		boxSize += 32; // recordSize
		return boxSize;
	}
}

}
