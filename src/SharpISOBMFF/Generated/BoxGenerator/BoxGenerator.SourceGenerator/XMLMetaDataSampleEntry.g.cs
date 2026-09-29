using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class XMLMetaDataSampleEntry() extends MetaDataSampleEntry ('metx') {
	utf8string content_encoding; // optional
	utf8list namespace;
	utf8list schema_location; // optional
}
*/
public partial class XMLMetaDataSampleEntry : MetaDataSampleEntry
{
	public const string TYPE = "metx";
	public override string DisplayName { get { return "XMLMetaDataSampleEntry"; } }

	protected BinaryUTF8String content_encoding;  //  optional
	protected bool content_encodingPresent;
	public BinaryUTF8String ContentEncoding { get { return this.content_encoding; } set { this.content_encoding = value; this.content_encodingPresent = true; } }
	public bool ContentEncodingPresent { get { return this.content_encodingPresent; } set { this.content_encodingPresent = value; } }

	protected BinaryUTF8String ns; 
	public BinaryUTF8String Ns { get { return this.ns; } set { this.ns = value; } }

	protected BinaryUTF8String schema_location;  //  optional
	protected bool schema_locationPresent;
	public BinaryUTF8String SchemaLocation { get { return this.schema_location; } set { this.schema_location = value; this.schema_locationPresent = true; } }
	public bool SchemaLocationPresent { get { return this.schema_locationPresent; } set { this.schema_locationPresent = value; } }

	public XMLMetaDataSampleEntry(): base(IsoStream.FromFourCC("metx"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		if (stream.HasStringBeforeBoxes(boxSize, readSize)) { boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_encoding, "content_encoding"); this.content_encodingPresent = true; } // optional
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.ns, "ns"); 
		if (stream.HasStringBeforeBoxes(boxSize, readSize)) { boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.schema_location, "schema_location"); this.schema_locationPresent = true; } // optional
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		if (this.content_encodingPresent) boxSize += stream.WriteStringZeroTerminated( this.content_encoding, "content_encoding"); // optional
		boxSize += stream.WriteStringZeroTerminated( this.ns, "ns"); 
		if (this.schema_locationPresent) boxSize += stream.WriteStringZeroTerminated( this.schema_location, "schema_location"); // optional
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		if (this.content_encodingPresent) boxSize += IsoStream.CalculateStringSize(content_encoding); // content_encoding
		boxSize += IsoStream.CalculateStringSize(ns); // ns
		if (this.schema_locationPresent) boxSize += IsoStream.CalculateStringSize(schema_location); // schema_location
		return boxSize;
	}
}

}
