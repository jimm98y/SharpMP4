using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ItemDataBox extends Box('idat') {
	bit(8) data[];
}
*/
public partial class ItemDataBox : Box
{
	public const string TYPE = "idat";
	public override string DisplayName { get { return "ItemDataBox"; } }

	protected StreamMarker data; 
	public StreamMarker Data { get { return this.data; } set { this.data = value; } }

	public ItemDataBox(): base(IsoStream.FromFourCC("idat"))
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
