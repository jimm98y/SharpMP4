using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23001-7 (3rd edition, not at hand; the 2023 at hand a preview) CENC sample encryption information, the fields of tenc's version 1 as GPAC reads them, not the multi-key form of later editions
aligned(8) class CencSampleEncryptionInformationGroupEntry() extends SampleGroupDescriptionEntry('seig') {
 unsigned int(8) reserved = 0;
 unsigned int(4) crypt_byte_block;
 unsigned int(4) skip_byte_block;
 unsigned int(8) isProtected;
 unsigned int(8) Per_Sample_IV_Size;
 unsigned int(8) KID[16];
 if (isProtected == 1 && Per_Sample_IV_Size == 0) {
  unsigned int(8) constant_IV_size;
  unsigned int(8) constant_IV[constant_IV_size];
 }
 } 
*/
public partial class CencSampleEncryptionInformationGroupEntry : SampleGroupDescriptionEntry
{
	public const string TYPE = "seig";
	public override string DisplayName { get { return "CencSampleEncryptionInformationGroupEntry"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte crypt_byte_block; 
	public byte CryptByteBlock { get { return this.crypt_byte_block; } set { this.crypt_byte_block = value; } }

	protected byte skip_byte_block; 
	public byte SkipByteBlock { get { return this.skip_byte_block; } set { this.skip_byte_block = value; } }

	protected byte isProtected; 
	public byte IsProtected { get { return this.isProtected; } set { this.isProtected = value; } }

	protected byte Per_Sample_IV_Size; 
	public byte PerSampleIVSize { get { return this.Per_Sample_IV_Size; } set { this.Per_Sample_IV_Size = value; } }

	protected byte[] KID; 
	public byte[] _KID { get { return this.KID; } set { this.KID = value; } }

	protected byte constant_IV_size; 
	public byte ConstantIVSize { get { return this.constant_IV_size; } set { this.constant_IV_size = value; } }

	protected byte[] constant_IV; 
	public byte[] ConstantIV { get { return this.constant_IV; } set { this.constant_IV = value; } }

	public CencSampleEncryptionInformationGroupEntry(): base(IsoStream.FromFourCC("seig"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.crypt_byte_block, "crypt_byte_block"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.skip_byte_block, "skip_byte_block"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.isProtected, "isProtected"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.Per_Sample_IV_Size, "Per_Sample_IV_Size"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.KID, "KID"); 

		if (isProtected == 1 && Per_Sample_IV_Size == 0)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.constant_IV_size, "constant_IV_size"); 
			boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(IsoStream.GetInt(constant_IV_size)),  out this.constant_IV, "constant_IV"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.reserved, "reserved"); 
		boxSize += stream.WriteBits(4,  this.crypt_byte_block, "crypt_byte_block"); 
		boxSize += stream.WriteBits(4,  this.skip_byte_block, "skip_byte_block"); 
		boxSize += stream.WriteUInt8( this.isProtected, "isProtected"); 
		boxSize += stream.WriteUInt8( this.Per_Sample_IV_Size, "Per_Sample_IV_Size"); 
		boxSize += stream.WriteUInt8Array(16,  this.KID, "KID"); 

		if (isProtected == 1 && Per_Sample_IV_Size == 0)
		{
			boxSize += stream.WriteUInt8( this.constant_IV_size, "constant_IV_size"); 
			boxSize += stream.WriteUInt8Array((uint)(IsoStream.GetInt(constant_IV_size)),  this.constant_IV, "constant_IV"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // reserved
		boxSize += 4; // crypt_byte_block
		boxSize += 4; // skip_byte_block
		boxSize += 8; // isProtected
		boxSize += 8; // Per_Sample_IV_Size
		boxSize += 16 * 8; // KID

		if (isProtected == 1 && Per_Sample_IV_Size == 0)
		{
			boxSize += 8; // constant_IV_size
			boxSize += ((ulong)(IsoStream.GetInt(constant_IV_size)) * 8); // constant_IV
		}
		return boxSize;
	}
}

}
