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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// QuickTime's, in a 'wave': 1 when the samples are little-endian; FFmpeg's mov_read_enda reads it
aligned(8) class AppleEndiannessBox() extends Box('enda') {
	unsigned int(16) littleEndian;
}"; } }
	public const string TYPE = "enda";
	public override string DisplayName { get { return "AppleEndiannessBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("wave"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("littleEndian", 16, this.littleEndian); 
		return boxSize;
	}
}

}
