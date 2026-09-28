using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// A part of the codestream: its index, the last part's with the top bit set
aligned(8) class JpegXLPartialCodestreamBox extends Box('jxlp') {
 unsigned int(1) is_last;
 unsigned int(31) index;
 bit(8) data[];
 }
*/
public partial class JpegXLPartialCodestreamBox : Box
{
	public const string TYPE = "jxlp";
	public override string DisplayName { get { return "JpegXLPartialCodestreamBox"; } }

	protected bool is_last; 
	public bool IsLast { get { return this.is_last; } set { this.is_last = value; } }

	protected uint index; 
	public uint Index { get { return this.index; } set { this.index = value; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public JpegXLPartialCodestreamBox(): base(IsoStream.FromFourCC("jxlp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.is_last, "is_last"); 
		boxSize += stream.ReadBits(boxSize, readSize, 31,  out this.index, "index"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.is_last, "is_last"); 
		boxSize += stream.WriteBits(31,  this.index, "index"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // is_last
		boxSize += 31; // index
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}
}

}
