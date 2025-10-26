using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class PiffTrackEncryptionBox() extends FullBox('uuid 8974dbce7be74c5184f97148f9882554', version, flags) 
{
 unsigned int(24) algorithmID;
 unsigned int(8) Per_Sample_IV_Size;
 unsigned int(8) kid[16];
 }

*/
public partial class PiffTrackEncryptionBox : FullBox
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "PiffTrackEncryptionBox"; } }

	protected uint algorithmID; 
	public uint AlgorithmID { get { return this.algorithmID; } set { this.algorithmID = value; } }

	protected byte Per_Sample_IV_Size; 
	public byte PerSampleIVSize { get { return this.Per_Sample_IV_Size; } set { this.Per_Sample_IV_Size = value; } }

	protected byte[] kid; 
	public byte[] Kid { get { return this.kid; } set { this.kid = value; } }

	public PiffTrackEncryptionBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("uuid"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt24(boxSize, readSize,  out this.algorithmID, "algorithmID"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.Per_Sample_IV_Size, "Per_Sample_IV_Size"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.kid, "kid"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt24( this.algorithmID, "algorithmID"); 
		boxSize += stream.WriteUInt8( this.Per_Sample_IV_Size, "Per_Sample_IV_Size"); 
		boxSize += stream.WriteUInt8Array(16,  this.kid, "kid"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 24; // algorithmID
		boxSize += 8; // Per_Sample_IV_Size
		boxSize += 16 * 8; // kid
		return boxSize;
	}
}

}
