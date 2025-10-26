using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MediaDataBox extends Box('mdat') {
	bit(8) data[];
}
*/
public partial class MediaDataBox : Box
{
	public const string TYPE = "mdat";
	public override string DisplayName { get { return "MediaDataBox"; } }

	protected StreamMarker data; 
	public StreamMarker Data { get { return this.data; } set { this.data = value; } }

	public MediaDataBox(): base(IsoStream.FromFourCC("mdat"))
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
