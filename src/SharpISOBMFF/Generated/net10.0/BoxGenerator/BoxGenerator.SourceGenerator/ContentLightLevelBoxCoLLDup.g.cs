using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class ContentLightLevelBox() extends FullBox('CoLL') {
 unsigned int(16) maxCLL;
 unsigned int(8) maxFALL;
 } 
*/
public partial class ContentLightLevelBoxCoLLDup : FullBox
{
	public const string TYPE = "CoLL";
	public override string DisplayName { get { return "ContentLightLevelBoxCoLLDup"; } }

	protected ushort maxCLL; 
	public ushort MaxCLL { get { return this.maxCLL; } set { this.maxCLL = value; } }

	protected byte maxFALL; 
	public byte MaxFALL { get { return this.maxFALL; } set { this.maxFALL = value; } }

	public ContentLightLevelBoxCoLLDup(): base(IsoStream.FromFourCC("CoLL"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.maxCLL, "maxCLL"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.maxFALL, "maxFALL"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.maxCLL, "maxCLL"); 
		boxSize += stream.WriteUInt8( this.maxFALL, "maxFALL"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // maxCLL
		boxSize += 8; // maxFALL
		return boxSize;
	}
}

}
