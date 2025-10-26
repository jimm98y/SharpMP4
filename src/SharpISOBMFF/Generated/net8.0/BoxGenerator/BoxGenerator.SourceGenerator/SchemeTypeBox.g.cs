﻿using System;
using System.Linq;
using System.Collections.Generic;

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
	public const string TYPE = "schm";
	public override string DisplayName { get { return "SchemeTypeBox"; } }

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
}

}
