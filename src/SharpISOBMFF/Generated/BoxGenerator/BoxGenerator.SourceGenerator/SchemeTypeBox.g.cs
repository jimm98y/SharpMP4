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
aligned(8) class SchemeTypeBox extends FullBox('schm', 0, flags) {
	unsigned int(32) scheme_type; // 4CC identifying the scheme
	unsigned int(32) scheme_version; // scheme version
	if (flags & 0x000001) {
		utf8string scheme_uri; // browser uri
	}
}
*/
public partial class SchemeTypeBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SchemeTypeBox extends FullBox('schm', 0, flags) {
	unsigned int(32) scheme_type; // 4CC identifying the scheme
	unsigned int(32) scheme_version; // scheme version
	if (flags & 0x000001) {
		utf8string scheme_uri; // browser uri
	}
}"; } }
	public const string TYPE = "schm";
	public override string DisplayName { get { return "SchemeTypeBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("sinf", "rinf", "srpp"); }


	protected uint scheme_type;  //  4CC identifying the scheme
	public uint SchemeType { get { return this.scheme_type; } set { this.scheme_type = value; } }

	protected uint scheme_version;  //  scheme version
	public uint SchemeVersion { get { return this.scheme_version; } set { this.scheme_version = value; } }

	protected BinaryUTF8String scheme_uri;  //  browser uri
	public BinaryUTF8String SchemeUri { get { return this.scheme_uri; } set { this.scheme_uri = value; } }

	public SchemeTypeBox(uint flags = 0): base(IsoStream.FromFourCC("schm"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.scheme_type, "scheme_type"); // 4CC identifying the scheme
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.scheme_version, "scheme_version"); // scheme version

		if ((flags  &  0x000001) ==  0x000001)
		{
			boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.scheme_uri, "scheme_uri"); // browser uri
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.scheme_type, "scheme_type"); // 4CC identifying the scheme
		boxSize += stream.WriteUInt32( this.scheme_version, "scheme_version"); // scheme version

		if ((flags  &  0x000001) ==  0x000001)
		{
			boxSize += stream.WriteStringZeroTerminated( this.scheme_uri, "scheme_uri"); // browser uri
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // scheme_type
		boxSize += 32; // scheme_version

		if ((flags  &  0x000001) ==  0x000001)
		{
			boxSize += IsoStream.CalculateStringSize(scheme_uri); // scheme_uri
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("scheme_type", 32, this.scheme_type); // 4CC identifying the scheme
		boxSize += tracer.Field("scheme_version", 32, this.scheme_version); // scheme version

		if ((flags  &  0x000001) ==  0x000001)
		{
			boxSize += tracer.Field("scheme_uri", IsoStream.CalculateStringSize(scheme_uri), this.scheme_uri); // browser uri
		}
		return boxSize;
	}
}

}
