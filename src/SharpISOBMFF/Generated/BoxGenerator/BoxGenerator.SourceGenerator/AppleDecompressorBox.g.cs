using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class AppleDecompressorBox() extends Box ('dcom'){
 unsigned int(32) decompressor;
 }
*/
public partial class AppleDecompressorBox : Box
{
	public const string TYPE = "dcom";
	public override string DisplayName { get { return "AppleDecompressorBox"; } }

	protected uint decompressor; 
	public uint Decompressor { get { return this.decompressor; } set { this.decompressor = value; } }

	public AppleDecompressorBox(): base(IsoStream.FromFourCC("dcom"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.decompressor, "decompressor"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.decompressor, "decompressor"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // decompressor
		return boxSize;
	}
}

}
