// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ThreeGPPLocationInformationBox() extends FullBox('loci', version = 0, 0) {
	bit(1) reserved = 0;
	unsigned int(5)[3] language;
	string placeName;
	unsigned int(8) role;
	fixedpoint1616 longitude;
	fixedpoint1616 latitude;
	fixedpoint1616 altitude;
	string astronomicalBody;
	string additionalNotes;
}
*/
public partial class ThreeGPPLocationInformationBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ThreeGPPLocationInformationBox() extends FullBox('loci', version = 0, 0) {
	bit(1) reserved = 0;
	unsigned int(5)[3] language;
	string placeName;
	unsigned int(8) role;
	fixedpoint1616 longitude;
	fixedpoint1616 latitude;
	fixedpoint1616 altitude;
	string astronomicalBody;
	string additionalNotes;
}"; } }
	public const string TYPE = "loci";
	public override string DisplayName { get { return "ThreeGPPLocationInformationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected bool reserved = false; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected string language = "und"; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected BinaryUTF8String placeName; 
	public BinaryUTF8String PlaceName { get { return this.placeName; } set { this.placeName = value; } }

	protected byte role; 
	public byte Role { get { return this.role; } set { this.role = value; } }

	protected double longitude; 
	public double Longitude { get { return this.longitude; } set { this.longitude = value; } }

	protected double latitude; 
	public double Latitude { get { return this.latitude; } set { this.latitude = value; } }

	protected double altitude; 
	public double Altitude { get { return this.altitude; } set { this.altitude = value; } }

	protected BinaryUTF8String astronomicalBody; 
	public BinaryUTF8String AstronomicalBody { get { return this.astronomicalBody; } set { this.astronomicalBody = value; } }

	protected BinaryUTF8String additionalNotes; 
	public BinaryUTF8String AdditionalNotes { get { return this.additionalNotes; } set { this.additionalNotes = value; } }

	public ThreeGPPLocationInformationBox(): base(IsoStream.FromFourCC("loci"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.placeName, "placeName"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.role, "role"); 
		boxSize += stream.ReadFixedPoint1616(boxSize, readSize,  out this.longitude, "longitude"); 
		boxSize += stream.ReadFixedPoint1616(boxSize, readSize,  out this.latitude, "latitude"); 
		boxSize += stream.ReadFixedPoint1616(boxSize, readSize,  out this.altitude, "altitude"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.astronomicalBody, "astronomicalBody"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.additionalNotes, "additionalNotes"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteStringZeroTerminated( this.placeName, "placeName"); 
		boxSize += stream.WriteUInt8( this.role, "role"); 
		boxSize += stream.WriteFixedPoint1616( this.longitude, "longitude"); 
		boxSize += stream.WriteFixedPoint1616( this.latitude, "latitude"); 
		boxSize += stream.WriteFixedPoint1616( this.altitude, "altitude"); 
		boxSize += stream.WriteStringZeroTerminated( this.astronomicalBody, "astronomicalBody"); 
		boxSize += stream.WriteStringZeroTerminated( this.additionalNotes, "additionalNotes"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // reserved
		boxSize += 15; // language
		boxSize += IsoStream.CalculateStringSize(placeName); // placeName
		boxSize += 8; // role
		boxSize += 32; // longitude
		boxSize += 32; // latitude
		boxSize += 32; // altitude
		boxSize += IsoStream.CalculateStringSize(astronomicalBody); // astronomicalBody
		boxSize += IsoStream.CalculateStringSize(additionalNotes); // additionalNotes
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved", 1, this.reserved); 
		boxSize += tracer.Field("language", 15, this.language); 
		boxSize += tracer.Field("placeName", IsoStream.CalculateStringSize(placeName), this.placeName); 
		boxSize += tracer.Field("role", 8, this.role); 
		boxSize += tracer.Field("longitude", 32, this.longitude); 
		boxSize += tracer.Field("latitude", 32, this.latitude); 
		boxSize += tracer.Field("altitude", 32, this.altitude); 
		boxSize += tracer.Field("astronomicalBody", IsoStream.CalculateStringSize(astronomicalBody), this.astronomicalBody); 
		boxSize += tracer.Field("additionalNotes", IsoStream.CalculateStringSize(additionalNotes), this.additionalNotes); 
		return boxSize;
	}
}

}
