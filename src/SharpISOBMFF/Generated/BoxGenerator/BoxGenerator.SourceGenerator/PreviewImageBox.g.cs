using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class PreviewImageBox() extends Box ('mcvr'){
 bit(8) data[];
 }
*/
public partial class PreviewImageBox : Box
{
	public const string TYPE = "mcvr";
	public override string DisplayName { get { return "PreviewImageBox"; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public PreviewImageBox(): base(IsoStream.FromFourCC("mcvr"))
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
