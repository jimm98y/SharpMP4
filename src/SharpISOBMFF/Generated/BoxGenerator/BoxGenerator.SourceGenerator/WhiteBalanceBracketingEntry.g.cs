using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class WhiteBalanceBracketingEntry
extends VisualSampleGroupEntry('wbbr') {
	unsigned int(16) blue_amber;
	int(8) green_magenta;
}
*/
public partial class WhiteBalanceBracketingEntry : VisualSampleGroupEntry
{
	public const string TYPE = "wbbr";
	public override string DisplayName { get { return "WhiteBalanceBracketingEntry"; } }

	protected ushort blue_amber; 
	public ushort BlueAmber { get { return this.blue_amber; } set { this.blue_amber = value; } }

	protected sbyte green_magenta; 
	public sbyte GreenMagenta { get { return this.green_magenta; } set { this.green_magenta = value; } }

	public WhiteBalanceBracketingEntry(): base(IsoStream.FromFourCC("wbbr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.blue_amber, "blue_amber"); 
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.green_magenta, "green_magenta"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.blue_amber, "blue_amber"); 
		boxSize += stream.WriteInt8( this.green_magenta, "green_magenta"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // blue_amber
		boxSize += 8; // green_magenta
		return boxSize;
	}
}

}
