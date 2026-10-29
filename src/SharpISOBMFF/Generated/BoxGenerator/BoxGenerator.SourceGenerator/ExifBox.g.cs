using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Exif: where in it the TIFF header is, then the Exif
aligned(8) class ExifBox extends Box('Exif') {
 unsigned int(32) tiff_header_offset;
 bit(8) data[];
 }
*/
public partial class ExifBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// Exif: where in it the TIFF header is, then the Exif
aligned(8) class ExifBox extends Box('Exif') {
 unsigned int(32) tiff_header_offset;
 bit(8) data[];
 }"; } }
	public const string TYPE = "Exif";
	public override string DisplayName { get { return "ExifBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("file"); }


	protected uint tiff_header_offset; 
	public uint TiffHeaderOffset { get { return this.tiff_header_offset; } set { this.tiff_header_offset = value; } }

	protected byte[] data = []; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public ExifBox(): base(IsoStream.FromFourCC("Exif"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tiff_header_offset, "tiff_header_offset"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.tiff_header_offset, "tiff_header_offset"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // tiff_header_offset
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("tiff_header_offset", 32, this.tiff_header_offset); 
		boxSize += tracer.Field("data", ((ulong)data.Length * 8), this.data); 
		return boxSize;
	}
}

}
