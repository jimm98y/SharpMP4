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
class XMLMetaDataSampleEntry() extends MetaDataSampleEntry ('metx') {
	utf8string content_encoding; // optional
	utf8list namespace;
	utf8list schema_location; // optional
}
*/
public partial class XMLMetaDataSampleEntry : MetaDataSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class XMLMetaDataSampleEntry() extends MetaDataSampleEntry ('metx') {
	utf8string content_encoding; // optional
	utf8list namespace;
	utf8list schema_location; // optional
}"; } }
	public const string TYPE = "metx";
	public override string DisplayName { get { return "XMLMetaDataSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		if (this.content_encodingPresent) boxSize += tracer.Field("content_encoding", IsoStream.CalculateStringSize(content_encoding), this.content_encoding); // optional
		boxSize += tracer.Field("ns", IsoStream.CalculateStringSize(ns), this.ns); 
		if (this.schema_locationPresent) boxSize += tracer.Field("schema_location", IsoStream.CalculateStringSize(schema_location), this.schema_location); // optional
		return boxSize;
	}
}

}
