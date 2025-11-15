using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class HintBox() extends Box('hint') {
 unsigned int(32) trackId;
 } 
*/
public partial class HintBox : Box
{
	public const string TYPE = "hint";
	public override string DisplayName { get { return "HintBox"; } }

	protected uint trackId; 
	public uint TrackId { get { return this.trackId; } set { this.trackId = value; } }

	public HintBox(): base(IsoStream.FromFourCC("hint"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.trackId, "trackId"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.trackId, "trackId"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // trackId
		return boxSize;
	}
}

}
