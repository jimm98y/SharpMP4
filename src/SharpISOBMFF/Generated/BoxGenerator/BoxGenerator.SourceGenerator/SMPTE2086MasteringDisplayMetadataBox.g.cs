using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SMPTE2086MasteringDisplayMetadataBox() extends FullBox('SmDm') {
 unsigned int(16) primaryRChromaticityX;
unsigned int(16) primaryRChromaticityY;
unsigned int(16) primaryGChromaticityX;
unsigned int(16) primaryGChromaticityY;
unsigned int(16) primaryBChromaticityX;
unsigned int(16) primaryBChromaticityY;
 unsigned int(32) luminanceMax;
 unsigned int(32) luminanceMin;
 } 
*/
public partial class SMPTE2086MasteringDisplayMetadataBox : FullBox
{
	public const string TYPE = "SmDm";
	public override string DisplayName { get { return "SMPTE2086MasteringDisplayMetadataBox"; } }

	protected ushort primaryRChromaticityX; 
	public ushort PrimaryRChromaticityX { get { return this.primaryRChromaticityX; } set { this.primaryRChromaticityX = value; } }

	protected ushort primaryRChromaticityY; 
	public ushort PrimaryRChromaticityY { get { return this.primaryRChromaticityY; } set { this.primaryRChromaticityY = value; } }

	protected ushort primaryGChromaticityX; 
	public ushort PrimaryGChromaticityX { get { return this.primaryGChromaticityX; } set { this.primaryGChromaticityX = value; } }

	protected ushort primaryGChromaticityY; 
	public ushort PrimaryGChromaticityY { get { return this.primaryGChromaticityY; } set { this.primaryGChromaticityY = value; } }

	protected ushort primaryBChromaticityX; 
	public ushort PrimaryBChromaticityX { get { return this.primaryBChromaticityX; } set { this.primaryBChromaticityX = value; } }

	protected ushort primaryBChromaticityY; 
	public ushort PrimaryBChromaticityY { get { return this.primaryBChromaticityY; } set { this.primaryBChromaticityY = value; } }

	protected uint luminanceMax; 
	public uint LuminanceMax { get { return this.luminanceMax; } set { this.luminanceMax = value; } }

	protected uint luminanceMin; 
	public uint LuminanceMin { get { return this.luminanceMin; } set { this.luminanceMin = value; } }

	public SMPTE2086MasteringDisplayMetadataBox(): base(IsoStream.FromFourCC("SmDm"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.primaryRChromaticityX, "primaryRChromaticityX"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.primaryRChromaticityY, "primaryRChromaticityY"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.primaryGChromaticityX, "primaryGChromaticityX"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.primaryGChromaticityY, "primaryGChromaticityY"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.primaryBChromaticityX, "primaryBChromaticityX"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.primaryBChromaticityY, "primaryBChromaticityY"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.luminanceMax, "luminanceMax"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.luminanceMin, "luminanceMin"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.primaryRChromaticityX, "primaryRChromaticityX"); 
		boxSize += stream.WriteUInt16( this.primaryRChromaticityY, "primaryRChromaticityY"); 
		boxSize += stream.WriteUInt16( this.primaryGChromaticityX, "primaryGChromaticityX"); 
		boxSize += stream.WriteUInt16( this.primaryGChromaticityY, "primaryGChromaticityY"); 
		boxSize += stream.WriteUInt16( this.primaryBChromaticityX, "primaryBChromaticityX"); 
		boxSize += stream.WriteUInt16( this.primaryBChromaticityY, "primaryBChromaticityY"); 
		boxSize += stream.WriteUInt32( this.luminanceMax, "luminanceMax"); 
		boxSize += stream.WriteUInt32( this.luminanceMin, "luminanceMin"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // primaryRChromaticityX
		boxSize += 16; // primaryRChromaticityY
		boxSize += 16; // primaryGChromaticityX
		boxSize += 16; // primaryGChromaticityY
		boxSize += 16; // primaryBChromaticityX
		boxSize += 16; // primaryBChromaticityY
		boxSize += 32; // luminanceMax
		boxSize += 32; // luminanceMin
		return boxSize;
	}
}

}
