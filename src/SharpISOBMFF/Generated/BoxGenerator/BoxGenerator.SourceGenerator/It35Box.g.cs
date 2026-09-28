using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ITU-T T.35 metadata's sample entry, as Chromium's MetadataIT35SampleEntry reads it
class It35Box() extends MetaDataSampleEntry('it35') {
	unsigned int(8) it35_identifier_length;
	bit(8*it35_identifier_length) it35_prefix;
}
*/
public partial class It35Box : MetaDataSampleEntry
{
	public const string TYPE = "it35";
	public override string DisplayName { get { return "It35Box"; } }

	protected byte it35_identifier_length; 
	public byte It35IdentifierLength { get { return this.it35_identifier_length; } set { this.it35_identifier_length = value; } }

	protected byte[] it35_prefix; 
	public byte[] It35Prefix { get { return this.it35_prefix; } set { this.it35_prefix = value; } }

	public It35Box(): base(IsoStream.FromFourCC("it35"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.it35_identifier_length, "it35_identifier_length"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*it35_identifier_length ),  out this.it35_prefix, "it35_prefix"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.it35_identifier_length, "it35_identifier_length"); 
		boxSize += stream.WriteBits((uint)(8*it35_identifier_length ),  this.it35_prefix, "it35_prefix"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // it35_identifier_length
		boxSize += (ulong)(8*it35_identifier_length ); // it35_prefix
		return boxSize;
	}
}

}
