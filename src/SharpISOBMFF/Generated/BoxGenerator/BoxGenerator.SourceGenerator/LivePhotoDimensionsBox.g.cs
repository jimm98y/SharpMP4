using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class LivePhotoDimensionsBox() extends Box('dims') {
 unsigned int(32) width;
 unsigned int(32) height;
}

*/
public partial class LivePhotoDimensionsBox : Box
{
	public const string TYPE = "dims";
	public override string DisplayName { get { return "LivePhotoDimensionsBox"; } }

	protected uint width; 
	public uint Width { get { return this.width; } set { this.width = value; } }

	protected uint height; 
	public uint Height { get { return this.height; } set { this.height = value; } }

	public LivePhotoDimensionsBox(): base(IsoStream.FromFourCC("dims"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.width, "width"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.height, "height"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.width, "width"); 
		boxSize += stream.WriteUInt32( this.height, "height"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // width
		boxSize += 32; // height
		return boxSize;
	}
}

}
