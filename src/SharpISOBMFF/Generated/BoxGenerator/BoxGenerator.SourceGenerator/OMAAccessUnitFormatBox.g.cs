using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OMAAccessUnitFormatBox() extends FullBox('odaf') {
	 bit(7) reserved;
 bit(1) selectiveEncrypted;
 unsigned int(8) keyIndicatorLength;
 unsigned int(8) initialVectorLength;
 } 
*/
public partial class OMAAccessUnitFormatBox : FullBox
{
	public const string TYPE = "odaf";
	public override string DisplayName { get { return "OMAAccessUnitFormatBox"; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool selectiveEncrypted; 
	public bool SelectiveEncrypted { get { return this.selectiveEncrypted; } set { this.selectiveEncrypted = value; } }

	protected byte keyIndicatorLength; 
	public byte KeyIndicatorLength { get { return this.keyIndicatorLength; } set { this.keyIndicatorLength = value; } }

	protected byte initialVectorLength; 
	public byte InitialVectorLength { get { return this.initialVectorLength; } set { this.initialVectorLength = value; } }

	public OMAAccessUnitFormatBox(): base(IsoStream.FromFourCC("odaf"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.selectiveEncrypted, "selectiveEncrypted"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.keyIndicatorLength, "keyIndicatorLength"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.initialVectorLength, "initialVectorLength"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.selectiveEncrypted, "selectiveEncrypted"); 
		boxSize += stream.WriteUInt8( this.keyIndicatorLength, "keyIndicatorLength"); 
		boxSize += stream.WriteUInt8( this.initialVectorLength, "initialVectorLength"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 7; // reserved
		boxSize += 1; // selectiveEncrypted
		boxSize += 8; // keyIndicatorLength
		boxSize += 8; // initialVectorLength
		return boxSize;
	}
}

}
