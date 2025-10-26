using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
abstract aligned(8) class SampleEntry (unsigned int(32) format) extends Box(format){ 
const unsigned int(8)[6] reservedSampleEntry = 0; 
unsigned int(16) data_reference_index; 
}

*/
public abstract partial class SampleEntry : Box
{
	public override string DisplayName { get { return "SampleEntry"; } }

	protected byte[] reservedSampleEntry = []; 
	public byte[] ReservedSampleEntry { get { return this.reservedSampleEntry; } set { this.reservedSampleEntry = value; } }

	protected ushort data_reference_index; 
	public ushort DataReferenceIndex { get { return this.data_reference_index; } set { this.data_reference_index = value; } }

	public SampleEntry(uint format): base(format)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 6,  out this.reservedSampleEntry, "reservedSampleEntry"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.data_reference_index, "data_reference_index"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(6,  this.reservedSampleEntry, "reservedSampleEntry"); 
		boxSize += stream.WriteUInt16( this.data_reference_index, "data_reference_index"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6 * 8; // reservedSampleEntry
		boxSize += 16; // data_reference_index
		return boxSize;
	}
}

}
