using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class VisualDRAPEntry() 
extends VisualSampleGroupEntry('drap') {
	unsigned int(3) DRAP_type;
	unsigned int(29) reserved = 0;
}
*/
public partial class VisualDRAPEntry : VisualSampleGroupEntry
{
	public const string TYPE = "drap";
	public override string DisplayName { get { return "VisualDRAPEntry"; } }

	protected byte DRAP_type; 
	public byte DRAPType { get { return this.DRAP_type; } set { this.DRAP_type = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public VisualDRAPEntry(): base(IsoStream.FromFourCC("drap"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.DRAP_type, "DRAP_type"); 
		boxSize += stream.ReadBits(boxSize, readSize, 29,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(3,  this.DRAP_type, "DRAP_type"); 
		boxSize += stream.WriteBits(29,  this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 3; // DRAP_type
		boxSize += 29; // reserved
		return boxSize;
	}
}

}
