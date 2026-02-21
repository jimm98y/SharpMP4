using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class JPEG_DecoderConfig extends DecoderSpecificInfo : bit(8) tag=DecSpecificInfoTag {
 int(16) headerLength;
 int(16) Xdensity;
 int(16) Ydensity;
 int(8) numComponents;
 }
*/
public partial class JPEG_DecoderConfig : DecoderSpecificInfo
{
	public const byte TYPE = DescriptorTags.DecSpecificInfoTag;
	public override string DisplayName { get { return "JPEG_DecoderConfig"; } }

	protected short headerLength; 
	public short HeaderLength { get { return this.headerLength; } set { this.headerLength = value; } }

	protected short Xdensity; 
	public short _Xdensity { get { return this.Xdensity; } set { this.Xdensity = value; } }

	protected short Ydensity; 
	public short _Ydensity { get { return this.Ydensity; } set { this.Ydensity = value; } }

	protected sbyte numComponents; 
	public sbyte NumComponents { get { return this.numComponents; } set { this.numComponents = value; } }

	public JPEG_DecoderConfig(): base()
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.headerLength, "headerLength"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.Xdensity, "Xdensity"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.Ydensity, "Ydensity"); 
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.numComponents, "numComponents"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt16( this.headerLength, "headerLength"); 
		boxSize += stream.WriteInt16( this.Xdensity, "Xdensity"); 
		boxSize += stream.WriteInt16( this.Ydensity, "Ydensity"); 
		boxSize += stream.WriteInt8( this.numComponents, "numComponents"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // headerLength
		boxSize += 16; // Xdensity
		boxSize += 16; // Ydensity
		boxSize += 8; // numComponents
		return boxSize;
	}
}

}
