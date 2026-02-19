using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class VideoContourMapBox extends FullBox('ctrm', 0, 0) { 
 bit(8) data[]; // TODO requires switch/case 
} 
*/
public partial class VideoContourMapBox : FullBox
{
	public const string TYPE = "ctrm";
	public override string DisplayName { get { return "VideoContourMapBox"; } }

	protected byte[] data;  //  TODO requires switch/case 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public VideoContourMapBox(): base(IsoStream.FromFourCC("ctrm"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); // TODO requires switch/case 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); // TODO requires switch/case 
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
