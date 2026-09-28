using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// The level of the codestream, 5 or 10
aligned(8) class JpegXLLevelBox extends Box('jxll') {
 unsigned int(8) level;
 }
*/
public partial class JpegXLLevelBox : Box
{
	public const string TYPE = "jxll";
	public override string DisplayName { get { return "JpegXLLevelBox"; } }

	protected byte level; 
	public byte Level { get { return this.level; } set { this.level = value; } }

	public JpegXLLevelBox(): base(IsoStream.FromFourCC("jxll"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level, "level"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.level, "level"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // level
		return boxSize;
	}
}

}
