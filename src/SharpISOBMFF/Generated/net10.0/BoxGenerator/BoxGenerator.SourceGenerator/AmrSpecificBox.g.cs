using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AmrSpecificBox() extends Box('damr') {
 unsigned int(8) decoderVersion;
 unsigned int(16) modeSet;
 unsigned int(8) modeChangedPeriod;
 unsigned int(8) framesPerSecond;
 } 
*/
public partial class AmrSpecificBox : Box
{
	public const string TYPE = "damr";
	public override string DisplayName { get { return "AmrSpecificBox"; } }

	protected byte decoderVersion; 
	public byte DecoderVersion { get { return this.decoderVersion; } set { this.decoderVersion = value; } }

	protected ushort modeSet; 
	public ushort ModeSet { get { return this.modeSet; } set { this.modeSet = value; } }

	protected byte modeChangedPeriod; 
	public byte ModeChangedPeriod { get { return this.modeChangedPeriod; } set { this.modeChangedPeriod = value; } }

	protected byte framesPerSecond; 
	public byte FramesPerSecond { get { return this.framesPerSecond; } set { this.framesPerSecond = value; } }

	public AmrSpecificBox(): base(IsoStream.FromFourCC("damr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.decoderVersion, "decoderVersion"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.modeSet, "modeSet"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.modeChangedPeriod, "modeChangedPeriod"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.framesPerSecond, "framesPerSecond"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.decoderVersion, "decoderVersion"); 
		boxSize += stream.WriteUInt16( this.modeSet, "modeSet"); 
		boxSize += stream.WriteUInt8( this.modeChangedPeriod, "modeChangedPeriod"); 
		boxSize += stream.WriteUInt8( this.framesPerSecond, "framesPerSecond"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // decoderVersion
		boxSize += 16; // modeSet
		boxSize += 8; // modeChangedPeriod
		boxSize += 8; // framesPerSecond
		return boxSize;
	}
}

}
