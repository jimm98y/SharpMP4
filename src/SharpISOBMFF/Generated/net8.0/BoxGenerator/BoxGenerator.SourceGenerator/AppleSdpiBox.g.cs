using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleSdpiBox extends Box('sdpi') {
 unsigned int(32) data; //TODO 
}

*/
public partial class AppleSdpiBox : Box
{
	public const string TYPE = "sdpi";
	public override string DisplayName { get { return "AppleSdpiBox"; } }

	protected uint data;  // TODO 
	public uint Data { get { return this.data; } set { this.data = value; } }

	public AppleSdpiBox(): base(IsoStream.FromFourCC("sdpi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.data, "data"); //TODO 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.data, "data"); //TODO 
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
