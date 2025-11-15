using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class PlayAllFramesBox() extends Box('AllF') {
	 unsigned int(8) data;
 } 
*/
public partial class PlayAllFramesBox : Box
{
	public const string TYPE = "AllF";
	public override string DisplayName { get { return "PlayAllFramesBox"; } }

	protected byte data; 
	public byte Data { get { return this.data; } set { this.data = value; } }

	public PlayAllFramesBox(): base(IsoStream.FromFourCC("AllF"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // data
		return boxSize;
	}
}

}
