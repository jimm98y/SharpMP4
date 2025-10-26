using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class AppleQualityBox() extends Box ('rmqu'){
 unsigned int(32) flags;
 unsigned int(32) quality;
 }
*/
public partial class AppleQualityBox : Box
{
	public const string TYPE = "rmqu";
	public override string DisplayName { get { return "AppleQualityBox"; } }

	protected uint flags; 
	public uint Flags { get { return this.flags; } set { this.flags = value; } }

	protected uint quality; 
	public uint Quality { get { return this.quality; } set { this.quality = value; } }

	public AppleQualityBox(): base(IsoStream.FromFourCC("rmqu"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.flags, "flags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.quality, "quality"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.flags, "flags"); 
		boxSize += stream.WriteUInt32( this.quality, "quality"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // flags
		boxSize += 32; // quality
		return boxSize;
	}
}

}
