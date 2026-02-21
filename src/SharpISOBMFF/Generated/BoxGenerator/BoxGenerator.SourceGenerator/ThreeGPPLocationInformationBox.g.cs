using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ThreeGPPLocationInformationBox() extends Box('loci') {
	bit(1) reserved;
 unsigned int(5)[3] language;
	string value;
 string placeName; unsigned int(8) role; fixedpoint1616 longitude;
 fixedpoint1616 latitude;
 fixedpoint1616 altitude;
 string astronomicalBody;
 string additionalNotes;
 }
*/
public partial class ThreeGPPLocationInformationBox : Box
{
	public const string TYPE = "loci";
	public override string DisplayName { get { return "ThreeGPPLocationInformationBox"; } }

	protected bool reserved; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected string language; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected BinaryUTF8String value; 
	public BinaryUTF8String Value { get { return this.value; } set { this.value = value; } }

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

	public ThreeGPPLocationInformationBox(): base(IsoStream.FromFourCC("loci"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.value, "value"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.placeName, "placeName"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.role, "role"); 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.longitude, "longitude"); 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.latitude, "latitude"); 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.altitude, "altitude"); 
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
		boxSize += stream.WriteStringZeroTerminated( this.value, "value"); 
		boxSize += stream.WriteStringZeroTerminated( this.placeName, "placeName"); 
		boxSize += stream.WriteUInt8( this.role, "role"); 
		boxSize += stream.WriteDouble32( this.longitude, "longitude"); 
		boxSize += stream.WriteDouble32( this.latitude, "latitude"); 
		boxSize += stream.WriteDouble32( this.altitude, "altitude"); 
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
		boxSize += IsoStream.CalculateStringSize(value); // value
		boxSize += IsoStream.CalculateStringSize(placeName); // placeName
		boxSize += 8; // role
		boxSize += 32; // longitude
		boxSize += 32; // latitude
		boxSize += 32; // altitude
		boxSize += IsoStream.CalculateStringSize(astronomicalBody); // astronomicalBody
		boxSize += IsoStream.CalculateStringSize(additionalNotes); // additionalNotes
		return boxSize;
	}
}

}
