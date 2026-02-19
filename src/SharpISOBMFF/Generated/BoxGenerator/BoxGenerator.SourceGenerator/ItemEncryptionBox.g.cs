using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ItemEncryptionBox extends ItemFullProperty('ienc', version, flags=0)
{
      unsigned int(8) reserved = 0;
      if (version==0) { 
            unsigned int(8) reserved = 0;
      } else { // version is 1 or greater
            unsigned int(4) crypt_byte_block;
            unsigned int(4) skip_byte_block;
      }
      unsigned int(8) num_keys;
      for (i=1; i<= num_keys; i++) {
         unsigned int(8) Per_Sample_IV_Size;
         unsigned int(8)[16] KID;
         if (Per_Sample_IV_Size == 0) { 
            unsigned int(8) constant_IV_size; 
            unsigned int(8)[constant_IV_size] constant_IV;
         }
      }
}

*/
public partial class ItemEncryptionBox : ItemFullProperty
{
	public const string TYPE = "ienc";
	public override string DisplayName { get { return "ItemEncryptionBox"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte reserved0 = 0; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte crypt_byte_block; 
	public byte CryptByteBlock { get { return this.crypt_byte_block; } set { this.crypt_byte_block = value; } }

	protected byte skip_byte_block; 
	public byte SkipByteBlock { get { return this.skip_byte_block; } set { this.skip_byte_block = value; } }

	protected byte num_keys; 
	public byte NumKeys { get { return this.num_keys; } set { this.num_keys = value; } }

	protected byte[] Per_Sample_IV_Size; 
	public byte[] PerSampleIVSize { get { return this.Per_Sample_IV_Size; } set { this.Per_Sample_IV_Size = value; } }

	protected byte[][] KID; 
	public byte[][] _KID { get { return this.KID; } set { this.KID = value; } }

	protected byte[] constant_IV_size; 
	public byte[] ConstantIVSize { get { return this.constant_IV_size; } set { this.constant_IV_size = value; } }

	protected byte[][] constant_IV; 
	public byte[][] ConstantIV { get { return this.constant_IV; } set { this.constant_IV = value; } }

	public ItemEncryptionBox(byte version = 0): base(IsoStream.FromFourCC("ienc"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved, "reserved"); 

		if (version==0)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved0, "reserved0"); 
		}

		else 
		{
			/*  version is 1 or greater */
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.crypt_byte_block, "crypt_byte_block"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.skip_byte_block, "skip_byte_block"); 
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_keys, "num_keys"); 

		this.Per_Sample_IV_Size = new byte[IsoStream.GetInt( num_keys)];
		this.KID = new byte[IsoStream.GetInt( num_keys)][];
		this.constant_IV_size = new byte[IsoStream.GetInt( num_keys)];
		this.constant_IV = new byte[IsoStream.GetInt( num_keys)][];
		for (int i=0; i< num_keys; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.Per_Sample_IV_Size[i], "Per_Sample_IV_Size"); 
			boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.KID[i], "KID"); 

			if (Per_Sample_IV_Size[i] == 0)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.constant_IV_size[i], "constant_IV_size"); 
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(IsoStream.GetInt(constant_IV_size)),  out this.constant_IV[i], "constant_IV"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.reserved, "reserved"); 

		if (version==0)
		{
			boxSize += stream.WriteUInt8( this.reserved0, "reserved0"); 
		}

		else 
		{
			/*  version is 1 or greater */
			boxSize += stream.WriteBits(4,  this.crypt_byte_block, "crypt_byte_block"); 
			boxSize += stream.WriteBits(4,  this.skip_byte_block, "skip_byte_block"); 
		}
		boxSize += stream.WriteUInt8( this.num_keys, "num_keys"); 

		for (int i=0; i< num_keys; i++)
		{
			boxSize += stream.WriteUInt8( this.Per_Sample_IV_Size[i], "Per_Sample_IV_Size"); 
			boxSize += stream.WriteUInt8Array(16,  this.KID[i], "KID"); 

			if (Per_Sample_IV_Size[i] == 0)
			{
				boxSize += stream.WriteUInt8( this.constant_IV_size[i], "constant_IV_size"); 
				boxSize += stream.WriteUInt8Array((uint)(IsoStream.GetInt(constant_IV_size)),  this.constant_IV[i], "constant_IV"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // reserved

		if (version==0)
		{
			boxSize += 8; // reserved0
		}

		else 
		{
			/*  version is 1 or greater */
			boxSize += 4; // crypt_byte_block
			boxSize += 4; // skip_byte_block
		}
		boxSize += 8; // num_keys

		for (int i=0; i< num_keys; i++)
		{
			boxSize += 8; // Per_Sample_IV_Size
			boxSize += 16 * 8; // KID

			if (Per_Sample_IV_Size[i] == 0)
			{
				boxSize += 8; // constant_IV_size
				boxSize += ((ulong)(IsoStream.GetInt(constant_IV_size)) * 8); // constant_IV
			}
		}
		return boxSize;
	}
}

}
