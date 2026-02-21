using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleDataReferenceBox() extends FullBox('rdrf') {
 unsigned int(32) dataReferenceType;
 unsigned int(32) count;
 unsigned int(8) dataReference[count];
 } 
*/
public partial class AppleDataReferenceBox : FullBox
{
	public const string TYPE = "rdrf";
	public override string DisplayName { get { return "AppleDataReferenceBox"; } }

	protected uint dataReferenceType; 
	public uint DataReferenceType { get { return this.dataReferenceType; } set { this.dataReferenceType = value; } }

	protected uint count; 
	public uint Count { get { return this.count; } set { this.count = value; } }

	protected byte[] dataReference; 
	public byte[] DataReference { get { return this.dataReference; } set { this.dataReference = value; } }

	public AppleDataReferenceBox(): base(IsoStream.FromFourCC("rdrf"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.dataReferenceType, "dataReferenceType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(count),  out this.dataReference, "dataReference"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.dataReferenceType, "dataReferenceType"); 
		boxSize += stream.WriteUInt32( this.count, "count"); 
		boxSize += stream.WriteUInt8Array((uint)(count),  this.dataReference, "dataReference"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // dataReferenceType
		boxSize += 32; // count
		boxSize += ((ulong)(count) * 8); // dataReference
		return boxSize;
	}
}

}
