using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// QuickTime's, in a 'wave': 1 when the samples are little-endian; FFmpeg's mov_read_enda reads it
aligned(8) class AppleEndiannessBox() extends Box('enda') {
	unsigned int(16) littleEndian;
}
*/
public partial class AppleEndiannessBox : Box
{
	public const string TYPE = "enda";
	public override string DisplayName { get { return "AppleEndiannessBox"; } }

	protected ushort littleEndian; 
	public ushort LittleEndian { get { return this.littleEndian; } set { this.littleEndian = value; } }

	public AppleEndiannessBox(): base(IsoStream.FromFourCC("enda"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.littleEndian, "littleEndian"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.littleEndian, "littleEndian"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // littleEndian
		return boxSize;
	}
}

}
