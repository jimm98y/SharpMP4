using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackEncryptionBox() extends FullBox('tenc') {
 unsigned int(24) defaultAlgorithmID;
 unsigned int(8) defaultIvSize;
 unsigned int(8) defaultKID[16];
 } 
*/
public partial class TrackEncryptionBox : FullBox
{
	public const string TYPE = "tenc";
	public override string DisplayName { get { return "TrackEncryptionBox"; } }

	protected uint defaultAlgorithmID; 
	public uint DefaultAlgorithmID { get { return this.defaultAlgorithmID; } set { this.defaultAlgorithmID = value; } }

	protected byte defaultIvSize; 
	public byte DefaultIvSize { get { return this.defaultIvSize; } set { this.defaultIvSize = value; } }

	protected byte[] defaultKID; 
	public byte[] DefaultKID { get { return this.defaultKID; } set { this.defaultKID = value; } }

	public TrackEncryptionBox(): base(IsoStream.FromFourCC("tenc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt24(boxSize, readSize,  out this.defaultAlgorithmID, "defaultAlgorithmID"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.defaultIvSize, "defaultIvSize"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.defaultKID, "defaultKID"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt24( this.defaultAlgorithmID, "defaultAlgorithmID"); 
		boxSize += stream.WriteUInt8( this.defaultIvSize, "defaultIvSize"); 
		boxSize += stream.WriteUInt8Array(16,  this.defaultKID, "defaultKID"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 24; // defaultAlgorithmID
		boxSize += 8; // defaultIvSize
		boxSize += 16 * 8; // defaultKID
		return boxSize;
	}
}

}
