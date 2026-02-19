using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ThreeGPPAlbumBox() extends Box('albm') {
	bit(1) reserved;
 unsigned int(5)[3] language;
	string value;
 bit(8) trackNumber; // optional
} 
*/
public partial class ThreeGPPAlbumBox : Box
{
	public const string TYPE = "albm";
	public override string DisplayName { get { return "ThreeGPPAlbumBox"; } }

	protected bool reserved; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected string language; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected BinaryUTF8String value; 
	public BinaryUTF8String Value { get { return this.value; } set { this.value = value; } }

	protected byte trackNumber;  //  optional
	public byte TrackNumber { get { return this.trackNumber; } set { this.trackNumber = value; } }

	public ThreeGPPAlbumBox(): base(IsoStream.FromFourCC("albm"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.value, "value"); 
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadUInt8(boxSize, readSize,  out this.trackNumber, "trackNumber"); // optional
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteStringZeroTerminated( this.value, "value"); 
		boxSize += stream.WriteUInt8( this.trackNumber, "trackNumber"); // optional
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // reserved
		boxSize += 15; // language
		boxSize += IsoStream.CalculateStringSize(value); // value
		boxSize += 8; // trackNumber
		return boxSize;
	}
}

}
