using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SubpicLevelInfoEntry() extends VisualSampleGroupEntry('spli')
{
	unsigned int(8) level_idc;
}
*/
public partial class SubpicLevelInfoEntry : VisualSampleGroupEntry
{
	public const string TYPE = "spli";
	public override string DisplayName { get { return "SubpicLevelInfoEntry"; } }

	protected byte level_idc; 
	public byte LevelIdc { get { return this.level_idc; } set { this.level_idc = value; } }

	public SubpicLevelInfoEntry(): base(IsoStream.FromFourCC("spli"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level_idc, "level_idc"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.level_idc, "level_idc"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // level_idc
		return boxSize;
	}
}

}
