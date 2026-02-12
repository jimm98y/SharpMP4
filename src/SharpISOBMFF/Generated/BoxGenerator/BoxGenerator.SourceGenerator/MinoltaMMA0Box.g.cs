using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class MinoltaMMA0Box() extends Box ('MMA0') {
 bit(8) data[];
 }
*/
public partial class MinoltaMMA0Box : Box
{
	public const string TYPE = "MMA0";
	public override string DisplayName { get { return "MinoltaMMA0Box"; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public MinoltaMMA0Box(): base(IsoStream.FromFourCC("MMA0"))
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
