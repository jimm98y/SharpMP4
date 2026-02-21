using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AUDSampleEntry() extends VisualSampleGroupEntry ('aud ')
{
	bit(24) audNalUnit;
}
*/
public partial class AUDSampleEntry : VisualSampleGroupEntry
{
	public const string TYPE = "aud ";
	public override string DisplayName { get { return "AUDSampleEntry"; } }

	protected uint audNalUnit; 
	public uint AudNalUnit { get { return this.audNalUnit; } set { this.audNalUnit = value; } }

	public AUDSampleEntry(): base(IsoStream.FromFourCC("aud "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt24(boxSize, readSize,  out this.audNalUnit, "audNalUnit"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt24( this.audNalUnit, "audNalUnit"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 24; // audNalUnit
		return boxSize;
	}
}

}
