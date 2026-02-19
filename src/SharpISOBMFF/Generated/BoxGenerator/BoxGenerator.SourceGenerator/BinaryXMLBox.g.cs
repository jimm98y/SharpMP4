using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class BinaryXMLBox
		extends FullBox('bxml', version = 0, 0) {
	unsigned int(8) data[];		// to end of box
}
*/
public partial class BinaryXMLBox : FullBox
{
	public const string TYPE = "bxml";
	public override string DisplayName { get { return "BinaryXMLBox"; } }

	protected byte[] data;  //  to end of box
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public BinaryXMLBox(): base(IsoStream.FromFourCC("bxml"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); // to end of box
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); // to end of box
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
