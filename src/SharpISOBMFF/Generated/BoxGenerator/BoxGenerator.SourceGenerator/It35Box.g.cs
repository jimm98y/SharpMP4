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
// ITU-T T.35 metadata's sample entry, as Chromium's MetadataIT35SampleEntry reads it
class It35Box() extends MetaDataSampleEntry('it35') {
	unsigned int(8) it35_identifier_length;
	bit(8*it35_identifier_length) it35_prefix;
}
*/
public partial class It35Box : MetaDataSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// ITU-T T.35 metadata's sample entry, as Chromium's MetadataIT35SampleEntry reads it
class It35Box() extends MetaDataSampleEntry('it35') {
	unsigned int(8) it35_identifier_length;
	bit(8*it35_identifier_length) it35_prefix;
}"; } }
	public const string TYPE = "it35";
	public override string DisplayName { get { return "It35Box"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }


	protected byte it35_identifier_length; 
	public byte It35IdentifierLength { get { return this.it35_identifier_length; } set { this.it35_identifier_length = value; } }

	protected byte[] it35_prefix = []; 
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("it35_identifier_length", 8, this.it35_identifier_length); 
		boxSize += tracer.Field("it35_prefix", (ulong)(8*it35_identifier_length ), this.it35_prefix); 
		return boxSize;
	}
}

}
