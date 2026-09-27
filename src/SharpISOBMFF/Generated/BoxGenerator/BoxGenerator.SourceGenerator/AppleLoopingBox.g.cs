using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleLoopingBox() extends Box('LOOP') {
	 unsigned int(32) data; // 0 normal, 1 palindromic
 } 
*/
public partial class AppleLoopingBox : Box
{
	public const string TYPE = "LOOP";
	public override string DisplayName { get { return "AppleLoopingBox"; } }

	protected uint data;  //  0 normal, 1 palindromic
	public uint Data { get { return this.data; } set { this.data = value; } }

	public AppleLoopingBox(): base(IsoStream.FromFourCC("LOOP"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.data, "data"); // 0 normal, 1 palindromic
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.data, "data"); // 0 normal, 1 palindromic
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // data
		return boxSize;
	}
}

}
