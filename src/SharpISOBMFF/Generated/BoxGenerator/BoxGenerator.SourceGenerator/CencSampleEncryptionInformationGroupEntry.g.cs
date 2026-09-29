using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23001-7:2016/Amd 1:2019, clause 6: a single key, or with multi_key_flag several, each with its IV size and KID (its constant IV's size multi_constant_IV_length here, for the generator's rewriting of constant_IV_size); the single key's fields as they were, its constant IV only where it is protected, as GPAC reads it
aligned(8) class CencSampleEncryptionInformationGroupEntry() extends SampleGroupDescriptionEntry('seig') {
 unsigned int(1) multi_key_flag;
 unsigned int(7) reserved = 0;
 unsigned int(4) crypt_byte_block;
 unsigned int(4) skip_byte_block;
 unsigned int(8) isProtected;
 if (multi_key_flag) {
  unsigned int(16) key_count;
  for (i=1; i <= key_count; i++) {
   unsigned int(8) multi_Per_Sample_IV_Size;
   unsigned int(8)[16] multi_KID;
   if (multi_Per_Sample_IV_Size == 0) {
    unsigned int(8) multi_constant_IV_length;
    unsigned int(8)[multi_constant_IV_length] multi_constant_IV;
   }
  }
 }
 else {
  unsigned int(8) Per_Sample_IV_Size;
  unsigned int(8) KID[16];
  if (isProtected == 1 && Per_Sample_IV_Size == 0) {
   unsigned int(8) constant_IV_size;
   unsigned int(8) constant_IV[constant_IV_size];
  }
 }
}
*/
public partial class CencSampleEncryptionInformationGroupEntry : SampleGroupDescriptionEntry
{
	public const string TYPE = "seig";
	public override string DisplayName { get { return "CencSampleEncryptionInformationGroupEntry"; } }

	protected bool multi_key_flag; 
	public bool MultiKeyFlag { get { return this.multi_key_flag; } set { this.multi_key_flag = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte crypt_byte_block; 
	public byte CryptByteBlock { get { return this.crypt_byte_block; } set { this.crypt_byte_block = value; } }

	protected byte skip_byte_block; 
	public byte SkipByteBlock { get { return this.skip_byte_block; } set { this.skip_byte_block = value; } }

	protected byte isProtected; 
	public byte IsProtected { get { return this.isProtected; } set { this.isProtected = value; } }

	protected ushort key_count; 
	public ushort KeyCount { get { return this.key_count; } set { this.key_count = value; } }

	protected byte[] multi_Per_Sample_IV_Size; 
	public byte[] MultiPerSampleIVSize { get { return this.multi_Per_Sample_IV_Size; } set { this.multi_Per_Sample_IV_Size = value; } }

	protected byte[][] multi_KID; 
	public byte[][] MultiKID { get { return this.multi_KID; } set { this.multi_KID = value; } }

	protected byte[] multi_constant_IV_length; 
	public byte[] MultiConstantIVLength { get { return this.multi_constant_IV_length; } set { this.multi_constant_IV_length = value; } }

	protected byte[][] multi_constant_IV; 
	public byte[][] MultiConstantIV { get { return this.multi_constant_IV; } set { this.multi_constant_IV = value; } }

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
		boxSize += stream.ReadBit(boxSize, readSize,  out this.multi_key_flag, "multi_key_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.crypt_byte_block, "crypt_byte_block"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.skip_byte_block, "skip_byte_block"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.isProtected, "isProtected"); 

		if (multi_key_flag)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.key_count, "key_count"); 

			this.multi_Per_Sample_IV_Size = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( key_count), "multi_Per_Sample_IV_Size");
			this.multi_KID = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt( key_count), "multi_KID");
			this.multi_constant_IV_length = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( key_count), "multi_constant_IV_length");
			this.multi_constant_IV = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt( key_count), "multi_constant_IV");
			for (int i=0; i < key_count; i++)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.multi_Per_Sample_IV_Size[i], "multi_Per_Sample_IV_Size"); 
				boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.multi_KID[i], "multi_KID"); 

				if (multi_Per_Sample_IV_Size[i] == 0)
				{
					boxSize += stream.ReadUInt8(boxSize, readSize,  out this.multi_constant_IV_length[i], "multi_constant_IV_length"); 
					boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(multi_constant_IV_length[i]),  out this.multi_constant_IV[i], "multi_constant_IV"); 
				}
			}
		}

		else 
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.Per_Sample_IV_Size, "Per_Sample_IV_Size"); 
			boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.KID, "KID"); 

			if (isProtected == 1 && Per_Sample_IV_Size == 0)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.constant_IV_size, "constant_IV_size"); 
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(IsoStream.GetInt(constant_IV_size)),  out this.constant_IV, "constant_IV"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.multi_key_flag, "multi_key_flag"); 
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(4,  this.crypt_byte_block, "crypt_byte_block"); 
		boxSize += stream.WriteBits(4,  this.skip_byte_block, "skip_byte_block"); 
		boxSize += stream.WriteUInt8( this.isProtected, "isProtected"); 

		if (multi_key_flag)
		{
			boxSize += stream.WriteUInt16( this.key_count, "key_count"); 

			for (int i=0; i < key_count; i++)
			{
				boxSize += stream.WriteUInt8( this.multi_Per_Sample_IV_Size[i], "multi_Per_Sample_IV_Size"); 
				boxSize += stream.WriteUInt8Array(16,  this.multi_KID[i], "multi_KID"); 

				if (multi_Per_Sample_IV_Size[i] == 0)
				{
					boxSize += stream.WriteUInt8( this.multi_constant_IV_length[i], "multi_constant_IV_length"); 
					boxSize += stream.WriteUInt8Array((uint)(multi_constant_IV_length[i]),  this.multi_constant_IV[i], "multi_constant_IV"); 
				}
			}
		}

		else 
		{
			boxSize += stream.WriteUInt8( this.Per_Sample_IV_Size, "Per_Sample_IV_Size"); 
			boxSize += stream.WriteUInt8Array(16,  this.KID, "KID"); 

			if (isProtected == 1 && Per_Sample_IV_Size == 0)
			{
				boxSize += stream.WriteUInt8( this.constant_IV_size, "constant_IV_size"); 
				boxSize += stream.WriteUInt8Array((uint)(IsoStream.GetInt(constant_IV_size)),  this.constant_IV, "constant_IV"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // multi_key_flag
		boxSize += 7; // reserved
		boxSize += 4; // crypt_byte_block
		boxSize += 4; // skip_byte_block
		boxSize += 8; // isProtected

		if (multi_key_flag)
		{
			boxSize += 16; // key_count

			for (int i=0; i < key_count; i++)
			{
				boxSize += 8; // multi_Per_Sample_IV_Size
				boxSize += 16 * 8; // multi_KID

				if (multi_Per_Sample_IV_Size[i] == 0)
				{
					boxSize += 8; // multi_constant_IV_length
					boxSize += ((ulong)(multi_constant_IV_length[i]) * 8); // multi_constant_IV
				}
			}
		}

		else 
		{
			boxSize += 8; // Per_Sample_IV_Size
			boxSize += 16 * 8; // KID

			if (isProtected == 1 && Per_Sample_IV_Size == 0)
			{
				boxSize += 8; // constant_IV_size
				boxSize += ((ulong)(IsoStream.GetInt(constant_IV_size)) * 8); // constant_IV
			}
		}
		return boxSize;
	}
}

}
