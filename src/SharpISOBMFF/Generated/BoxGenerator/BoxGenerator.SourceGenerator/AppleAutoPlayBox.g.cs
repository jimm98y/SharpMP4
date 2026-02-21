using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleAutoPlayBox() extends Box('play') {
	 unsigned int(8) autoplay;
 } 
*/
public partial class AppleAutoPlayBox : Box
{
	public const string TYPE = "play";
	public override string DisplayName { get { return "AppleAutoPlayBox"; } }

	protected byte autoplay; 
	public byte Autoplay { get { return this.autoplay; } set { this.autoplay = value; } }

	public AppleAutoPlayBox(): base(IsoStream.FromFourCC("play"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.autoplay, "autoplay"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.autoplay, "autoplay"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // autoplay
		return boxSize;
	}
}

}
