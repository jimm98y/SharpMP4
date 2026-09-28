using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HTCSlmtBox() extends Box('slmt') {
 unsigned int(32) value;
}

*/
public partial class HTCSlmtBox : Box
{
	public const string TYPE = "slmt";
	public override string DisplayName { get { return "HTCSlmtBox"; } }

	protected uint value; 
	public uint Value { get { return this.value; } set { this.value = value; } }

	public HTCSlmtBox(): base(IsoStream.FromFourCC("slmt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.value, "value"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.value, "value"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // value
		return boxSize;
	}
}

}
