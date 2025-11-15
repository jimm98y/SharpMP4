using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class hintrepeatedBytesSent extends Box('drep') {
	uint(64)	bytessent; }	// total bytes in repeated packets
*/
public partial class hintrepeatedBytesSent : Box
{
	public const string TYPE = "drep";
	public override string DisplayName { get { return "hintrepeatedBytesSent"; } }

	protected ulong bytessent; 
	public ulong Bytessent { get { return this.bytessent; } set { this.bytessent = value; } }

	public hintrepeatedBytesSent(): base(IsoStream.FromFourCC("drep"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.bytessent, "bytessent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.bytessent, "bytessent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // bytessent
		return boxSize;
	}
}

}
