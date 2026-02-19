using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

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
	public BinaryUTF8String ContentEncoding { get { return this.content_encoding; } set { this.content_encoding = value; } }

	protected BinaryUTF8String ns; 
	public BinaryUTF8String Ns { get { return this.ns; } set { this.ns = value; } }

	protected BinaryUTF8String schema_location;  //  optional
	public BinaryUTF8String SchemaLocation { get { return this.schema_location; } set { this.schema_location = value; } }

	public XMLMetaDataSampleEntry(): base(IsoStream.FromFourCC("metx"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_encoding, "content_encoding"); // optional
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.ns, "ns"); 
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.schema_location, "schema_location"); // optional
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.content_encoding, "content_encoding"); // optional
		boxSize += stream.WriteStringZeroTerminated( this.ns, "ns"); 
		boxSize += stream.WriteStringZeroTerminated( this.schema_location, "schema_location"); // optional
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(content_encoding); // content_encoding
		boxSize += IsoStream.CalculateStringSize(ns); // ns
		boxSize += IsoStream.CalculateStringSize(schema_location); // schema_location
		return boxSize;
	}
}

}
