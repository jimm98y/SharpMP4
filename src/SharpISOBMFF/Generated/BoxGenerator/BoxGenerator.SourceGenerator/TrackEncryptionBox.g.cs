using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackEncryptionBox() extends FullBox('tenc', version, flags = 0) {
 unsigned int(8) reserved = 0;
 if (version == 0) {
  unsigned int(8) reserved = 0;
 }
 else {
  unsigned int(4) default_crypt_byte_block;
  unsigned int(4) default_skip_byte_block;
 }
 unsigned int(8) default_isProtected;
 unsigned int(8) default_Per_Sample_IV_Size;
 unsigned int(8) default_KID[16];
 if (default_isProtected == 1 && default_Per_Sample_IV_Size == 0) {
  unsigned int(8) defaultConstantIVSize;
  unsigned int(8) defaultConstantIV[defaultConstantIVSize];
 }
 } 
*/
public partial class TrackEncryptionBox : FullBox
{
	public const string TYPE = "tenc";
	public override string DisplayName { get { return "TrackEncryptionBox"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte reserved0 = 0; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte default_crypt_byte_block; 
	public byte DefaultCryptByteBlock { get { return this.default_crypt_byte_block; } set { this.default_crypt_byte_block = value; } }

	protected byte default_skip_byte_block; 
	public byte DefaultSkipByteBlock { get { return this.default_skip_byte_block; } set { this.default_skip_byte_block = value; } }

	protected byte default_isProtected; 
	public byte DefaultIsProtected { get { return this.default_isProtected; } set { this.default_isProtected = value; } }

	protected byte default_Per_Sample_IV_Size; 
	public byte DefaultPerSampleIVSize { get { return this.default_Per_Sample_IV_Size; } set { this.default_Per_Sample_IV_Size = value; } }

	protected byte[] default_KID; 
	public byte[] DefaultKID { get { return this.default_KID; } set { this.default_KID = value; } }

	protected byte defaultConstantIVSize; 
	public byte DefaultConstantIVSize { get { return this.defaultConstantIVSize; } set { this.defaultConstantIVSize = value; } }

	protected byte[] defaultConstantIV; 
	public byte[] DefaultConstantIV { get { return this.defaultConstantIV; } set { this.defaultConstantIV = value; } }

	public TrackEncryptionBox(byte version = 0): base(IsoStream.FromFourCC("tenc"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved, "reserved"); 

		if (version == 0)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved0, "reserved0"); 
		}

		else 
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.default_crypt_byte_block, "default_crypt_byte_block"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.default_skip_byte_block, "default_skip_byte_block"); 
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.default_isProtected, "default_isProtected"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.default_Per_Sample_IV_Size, "default_Per_Sample_IV_Size"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.default_KID, "default_KID"); 

		if (default_isProtected == 1 && default_Per_Sample_IV_Size == 0)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.defaultConstantIVSize, "defaultConstantIVSize"); 
			boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(defaultConstantIVSize),  out this.defaultConstantIV, "defaultConstantIV"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.reserved, "reserved"); 

		if (version == 0)
		{
			boxSize += stream.WriteUInt8( this.reserved0, "reserved0"); 
		}

		else 
		{
			boxSize += stream.WriteBits(4,  this.default_crypt_byte_block, "default_crypt_byte_block"); 
			boxSize += stream.WriteBits(4,  this.default_skip_byte_block, "default_skip_byte_block"); 
		}
		boxSize += stream.WriteUInt8( this.default_isProtected, "default_isProtected"); 
		boxSize += stream.WriteUInt8( this.default_Per_Sample_IV_Size, "default_Per_Sample_IV_Size"); 
		boxSize += stream.WriteUInt8Array(16,  this.default_KID, "default_KID"); 

		if (default_isProtected == 1 && default_Per_Sample_IV_Size == 0)
		{
			boxSize += stream.WriteUInt8( this.defaultConstantIVSize, "defaultConstantIVSize"); 
			boxSize += stream.WriteUInt8Array((uint)(defaultConstantIVSize),  this.defaultConstantIV, "defaultConstantIV"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // reserved

		if (version == 0)
		{
			boxSize += 8; // reserved0
		}

		else 
		{
			boxSize += 4; // default_crypt_byte_block
			boxSize += 4; // default_skip_byte_block
		}
		boxSize += 8; // default_isProtected
		boxSize += 8; // default_Per_Sample_IV_Size
		boxSize += 16 * 8; // default_KID

		if (default_isProtected == 1 && default_Per_Sample_IV_Size == 0)
		{
			boxSize += 8; // defaultConstantIVSize
			boxSize += ((ulong)(defaultConstantIVSize) * 8); // defaultConstantIV
		}
		return boxSize;
	}
}

}
