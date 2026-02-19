using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AppleCompressedMoovDataBox() extends Box ('cmvd'){
 unsigned int(32) uncompressedSize;
bit(8) compressedData[];
 }
*/
public partial class AppleCompressedMoovDataBox : Box
{
	public const string TYPE = "cmvd";
	public override string DisplayName { get { return "AppleCompressedMoovDataBox"; } }

	protected uint uncompressedSize; 
	public uint UncompressedSize { get { return this.uncompressedSize; } set { this.uncompressedSize = value; } }

	protected byte[] compressedData; 
	public byte[] CompressedData { get { return this.compressedData; } set { this.compressedData = value; } }

	public AppleCompressedMoovDataBox(): base(IsoStream.FromFourCC("cmvd"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.uncompressedSize, "uncompressedSize"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.compressedData, "compressedData"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.uncompressedSize, "uncompressedSize"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.compressedData, "compressedData"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // uncompressedSize
		boxSize += ((ulong)compressedData.Length * 8); // compressedData
		return boxSize;
	}
}

}
