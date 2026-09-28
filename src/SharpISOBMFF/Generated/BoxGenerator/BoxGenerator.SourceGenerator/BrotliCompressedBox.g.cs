using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// A box compressed with Brotli: the type of the box it is, then that box's content, compressed
aligned(8) class BrotliCompressedBox extends Box('brob') {
 unsigned int(32) box_type;
 bit(8) data[];
 }
*/
public partial class BrotliCompressedBox : Box
{
	public const string TYPE = "brob";
	public override string DisplayName { get { return "BrotliCompressedBox"; } }

	protected uint box_type; 
	public uint BoxType { get { return this.box_type; } set { this.box_type = value; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public BrotliCompressedBox(): base(IsoStream.FromFourCC("brob"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.box_type, "box_type"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.box_type, "box_type"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // box_type
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}
}

}
