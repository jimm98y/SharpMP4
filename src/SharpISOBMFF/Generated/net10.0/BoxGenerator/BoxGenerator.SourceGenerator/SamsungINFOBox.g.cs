using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class SamsungINFOBox() extends Box ('INFO') {
 bit(8) data[];
 }
*/
public partial class SamsungINFOBox : Box
{
	public const string TYPE = "INFO";
	public override string DisplayName { get { return "SamsungINFOBox"; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public SamsungINFOBox(): base(IsoStream.FromFourCC("INFO"))
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
