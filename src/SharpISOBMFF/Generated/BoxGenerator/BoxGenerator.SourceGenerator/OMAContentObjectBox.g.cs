using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OMAContentObjectBox() extends FullBox('odda') {
	 unsigned int(32) count;
 unsigned int(8) data[count];
 } 
*/
public partial class OMAContentObjectBox : FullBox
{
	public const string TYPE = "odda";
	public override string DisplayName { get { return "OMAContentObjectBox"; } }

	protected uint count; 
	public uint Count { get { return this.count; } set { this.count = value; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public OMAContentObjectBox(): base(IsoStream.FromFourCC("odda"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(count),  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.count, "count"); 
		boxSize += stream.WriteUInt8Array((uint)(count),  this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // count
		boxSize += ((ulong)(count) * 8); // data
		return boxSize;
	}
}

}
