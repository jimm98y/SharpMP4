using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class XMLSubtitleSampleEntry() extends SubtitleSampleEntry ('stpp') { 
 string namespace; 
 string schema_location;  // optional 
 string auxiliary_mime_types; // optional, required if auxiliary resources are present 
 BitRateBox (); 
} 


*/
public partial class XMLSubtitleSampleEntry : SubtitleSampleEntry
{
	public const string TYPE = "stpp";
	public override string DisplayName { get { return "XMLSubtitleSampleEntry"; } }

	protected BinaryUTF8String ns; 
	public BinaryUTF8String Ns { get { return this.ns; } set { this.ns = value; } }

	protected BinaryUTF8String schema_location;  //  optional 
	public BinaryUTF8String SchemaLocation { get { return this.schema_location; } set { this.schema_location = value; } }

	protected BinaryUTF8String auxiliary_mime_types;  //  optional, required if auxiliary resources are present 
	public BinaryUTF8String AuxiliaryMimeTypes { get { return this.auxiliary_mime_types; } set { this.auxiliary_mime_types = value; } }
	public BitRateBox _BitRateBox { get { return this.children.OfType<BitRateBox>().FirstOrDefault(); } }

	public XMLSubtitleSampleEntry(): base(IsoStream.FromFourCC("stpp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.ns, "ns"); 
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.schema_location, "schema_location"); // optional 
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.auxiliary_mime_types, "auxiliary_mime_types"); // optional, required if auxiliary resources are present 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.BitRateBox, "BitRateBox"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.ns, "ns"); 
		boxSize += stream.WriteStringZeroTerminated( this.schema_location, "schema_location"); // optional 
		boxSize += stream.WriteStringZeroTerminated( this.auxiliary_mime_types, "auxiliary_mime_types"); // optional, required if auxiliary resources are present 
		// boxSize += stream.WriteBox( this.BitRateBox, "BitRateBox"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(ns); // ns
		boxSize += IsoStream.CalculateStringSize(schema_location); // schema_location
		boxSize += IsoStream.CalculateStringSize(auxiliary_mime_types); // auxiliary_mime_types
		// boxSize += IsoStream.CalculateBoxSize(BitRateBox); // BitRateBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
