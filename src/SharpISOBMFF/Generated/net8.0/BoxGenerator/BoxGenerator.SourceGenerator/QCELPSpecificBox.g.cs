﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class QCELPSpecificBox() extends Box('dqcp') {
	 unsigned int(32) vendor;
	 unsigned int(8) decoderVersion;
 unsigned int(8) framesPerSample;
 } 
*/
public partial class QCELPSpecificBox : Box
{
	public const string TYPE = "dqcp";
	public override string DisplayName { get { return "QCELPSpecificBox"; } }

	protected uint vendor; 
	public uint Vendor { get { return this.vendor; } set { this.vendor = value; } }

	protected byte decoderVersion; 
	public byte DecoderVersion { get { return this.decoderVersion; } set { this.decoderVersion = value; } }

	protected byte framesPerSample; 
	public byte FramesPerSample { get { return this.framesPerSample; } set { this.framesPerSample = value; } }

	public QCELPSpecificBox(): base(IsoStream.FromFourCC("dqcp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vendor, "vendor"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.decoderVersion, "decoderVersion"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.framesPerSample, "framesPerSample"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.vendor, "vendor"); 
		boxSize += stream.WriteUInt8( this.decoderVersion, "decoderVersion"); 
		boxSize += stream.WriteUInt8( this.framesPerSample, "framesPerSample"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // vendor
		boxSize += 8; // decoderVersion
		boxSize += 8; // framesPerSample
		return boxSize;
	}
}

}
