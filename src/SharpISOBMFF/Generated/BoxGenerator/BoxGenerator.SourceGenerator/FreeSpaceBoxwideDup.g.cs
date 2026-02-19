using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FreeSpaceBox() extends Box('wide') {
 bit(8) data[];
 } 
*/
public partial class FreeSpaceBoxwideDup : Box
{
	public const string TYPE = "wide";
	public override string DisplayName { get { return "FreeSpaceBoxwideDup"; } }

	protected StreamMarker data; 
	public StreamMarker Data { get { return this.data; } set { this.data = value; } }

	public FreeSpaceBoxwideDup(): base(IsoStream.FromFourCC("wide"))
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
