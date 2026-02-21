using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ReceivedSsrcBox extends Box('rssr') {
	unsigned int(32)	SSRC
}
*/
public partial class ReceivedSsrcBox : Box
{
	public const string TYPE = "rssr";
	public override string DisplayName { get { return "ReceivedSsrcBox"; } }

	protected uint SSRC; 
	public uint _SSRC { get { return this.SSRC; } set { this.SSRC = value; } }

	public ReceivedSsrcBox(): base(IsoStream.FromFourCC("rssr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.SSRC, "SSRC"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.SSRC, "SSRC"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // SSRC
		return boxSize;
	}
}

}
