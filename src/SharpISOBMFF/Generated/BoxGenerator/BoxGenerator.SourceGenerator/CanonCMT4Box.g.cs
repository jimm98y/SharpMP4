using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CanonCMT4Box() extends Box('CMT4') {
 bit(8) tiff[];
}

*/
public partial class CanonCMT4Box : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CanonCMT4Box() extends Box('CMT4') {
 bit(8) tiff[];
}
"; } }
	public const string TYPE = "CMT4";
	public override string DisplayName { get { return "CanonCMT4Box"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("uuid"); }


	protected byte[] tiff = []; 
	public byte[] Tiff { get { return this.tiff; } set { this.tiff = value; } }

	public CanonCMT4Box(): base(IsoStream.FromFourCC("CMT4"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.tiff, "tiff"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.tiff, "tiff"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)tiff.Length * 8); // tiff
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("tiff", ((ulong)tiff.Length * 8), this.tiff); 
		return boxSize;
	}
}

}
