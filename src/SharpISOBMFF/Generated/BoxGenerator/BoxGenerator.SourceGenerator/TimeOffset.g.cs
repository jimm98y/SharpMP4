using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TimeOffset() extends Box('tsro') {
	int(32)		offset;
}


*/
public partial class TimeOffset : Box
{
	public const string TYPE = "tsro";
	public override string DisplayName { get { return "TimeOffset"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("HintSampleEntry(rtp )", "SrtpHintSampleEntry(srtp)", "ReceivedRtpHintSampleEntry(rrtp)"); }


	protected int offset; 
	public int Offset { get { return this.offset; } set { this.offset = value; } }

	public TimeOffset(): base(IsoStream.FromFourCC("tsro"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.offset, "offset"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.offset, "offset"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // offset
		return boxSize;
	}
}

}
