using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class MultiviewSceneInfoBox extends Box ('vwdi') 
{
	unsigned int(8) 	max_disparity;
}
*/
public partial class MultiviewSceneInfoBox : Box
{
	public const string TYPE = "vwdi";
	public override string DisplayName { get { return "MultiviewSceneInfoBox"; } }

	protected byte max_disparity; 
	public byte MaxDisparity { get { return this.max_disparity; } set { this.max_disparity = value; } }

	public MultiviewSceneInfoBox(): base(IsoStream.FromFourCC("vwdi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.max_disparity, "max_disparity"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.max_disparity, "max_disparity"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // max_disparity
		return boxSize;
	}
}

}
