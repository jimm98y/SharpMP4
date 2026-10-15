using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Canon.pm, CMT1 to CMT4: TIFF structures - IFD0, the Exif IFD, the maker notes and the GPS IFD
aligned(8) class CanonCMT1Box() extends Box('CMT1') {
 bit(8) tiff[];
}

*/
public partial class CanonCMT1Box : Box
{
	public const string TYPE = "CMT1";
	public override string DisplayName { get { return "CanonCMT1Box"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("uuid"); }


	protected byte[] tiff; 
	public byte[] Tiff { get { return this.tiff; } set { this.tiff = value; } }

	public CanonCMT1Box(): base(IsoStream.FromFourCC("CMT1"))
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

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("tiff", ((ulong)tiff.Length * 8), this.tiff); 
		return boxSize;
	}
}

}
