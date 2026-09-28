using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// How the samples are encrypted: selectively or not, and how long their key indicators and IVs are
aligned(8) class ISMASampleFormatBox extends FullBox('iSFM', 0, 0) {
 bit(1) selective_encryption;
 bit(7) reserved;
 unsigned int(8) key_indicator_length;
 unsigned int(8) IV_length;
 }
*/
public partial class ISMASampleFormatBox : FullBox
{
	public const string TYPE = "iSFM";
	public override string DisplayName { get { return "ISMASampleFormatBox"; } }

	protected bool selective_encryption; 
	public bool SelectiveEncryption { get { return this.selective_encryption; } set { this.selective_encryption = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte key_indicator_length; 
	public byte KeyIndicatorLength { get { return this.key_indicator_length; } set { this.key_indicator_length = value; } }

	protected byte IV_length; 
	public byte IVLength { get { return this.IV_length; } set { this.IV_length = value; } }

	public ISMASampleFormatBox(): base(IsoStream.FromFourCC("iSFM"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.selective_encryption, "selective_encryption"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.key_indicator_length, "key_indicator_length"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.IV_length, "IV_length"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.selective_encryption, "selective_encryption"); 
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteUInt8( this.key_indicator_length, "key_indicator_length"); 
		boxSize += stream.WriteUInt8( this.IV_length, "IV_length"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // selective_encryption
		boxSize += 7; // reserved
		boxSize += 8; // key_indicator_length
		boxSize += 8; // IV_length
		return boxSize;
	}
}

}
