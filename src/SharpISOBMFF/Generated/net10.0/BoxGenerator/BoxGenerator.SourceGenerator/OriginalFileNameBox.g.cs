using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class OriginalFileNameBox() extends Box ('finm'){
 bit(8) data[];
 }
*/
public partial class OriginalFileNameBox : Box
{
	public const string TYPE = "finm";
	public override string DisplayName { get { return "OriginalFileNameBox"; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public OriginalFileNameBox(): base(IsoStream.FromFourCC("finm"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}
}

}
