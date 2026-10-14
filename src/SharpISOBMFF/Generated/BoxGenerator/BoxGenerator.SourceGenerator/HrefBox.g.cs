using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// 3GPP TS 26.245 5.17.3.13, HyperTextBox: a modifier of a timed text sample - after its text, not in the box tree - making a run of its characters a link
aligned(8) class HrefBox() extends Box('href') {
 unsigned int(16) startcharoffset;
 unsigned int(16) endcharoffset;
 unsigned int(8) urlLength;
 unsigned int(8) url[urlLength];
 unsigned int(8) altLength;
 unsigned int(8) altstring[altLength];
}
*/
public partial class HrefBox : Box
{
	public const string TYPE = "href";
	public override string DisplayName { get { return "HrefBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("*"); }


	protected ushort startcharoffset; 
	public ushort Startcharoffset { get { return this.startcharoffset; } set { this.startcharoffset = value; } }

	protected ushort endcharoffset; 
	public ushort Endcharoffset { get { return this.endcharoffset; } set { this.endcharoffset = value; } }

	protected byte urlLength; 
	public byte UrlLength { get { return this.urlLength; } set { this.urlLength = value; } }

	protected byte[] url; 
	public byte[] Url { get { return this.url; } set { this.url = value; } }

	protected byte altLength; 
	public byte AltLength { get { return this.altLength; } set { this.altLength = value; } }

	protected byte[] altstring; 
	public byte[] Altstring { get { return this.altstring; } set { this.altstring = value; } }

	public HrefBox(): base(IsoStream.FromFourCC("href"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.startcharoffset, "startcharoffset"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.endcharoffset, "endcharoffset"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.urlLength, "urlLength"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(urlLength),  out this.url, "url"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.altLength, "altLength"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(altLength),  out this.altstring, "altstring"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.startcharoffset, "startcharoffset"); 
		boxSize += stream.WriteUInt16( this.endcharoffset, "endcharoffset"); 
		boxSize += stream.WriteUInt8( this.urlLength, "urlLength"); 
		boxSize += stream.WriteUInt8Array((uint)(urlLength),  this.url, "url"); 
		boxSize += stream.WriteUInt8( this.altLength, "altLength"); 
		boxSize += stream.WriteUInt8Array((uint)(altLength),  this.altstring, "altstring"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // startcharoffset
		boxSize += 16; // endcharoffset
		boxSize += 8; // urlLength
		boxSize += ((ulong)(urlLength) * 8); // url
		boxSize += 8; // altLength
		boxSize += ((ulong)(altLength) * 8); // altstring
		return boxSize;
	}
}

}
