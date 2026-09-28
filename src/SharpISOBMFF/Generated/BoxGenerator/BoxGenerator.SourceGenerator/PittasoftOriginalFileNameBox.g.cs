using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ExifTool: substr($val, 4, -1) - 4 bytes, then the name and its terminator
aligned(8) class PittasoftOriginalFileNameBox() extends Box('ptnm') {
 unsigned int(32) unknown;
 string name;
}

*/
public partial class PittasoftOriginalFileNameBox : Box
{
	public const string TYPE = "ptnm";
	public override string DisplayName { get { return "PittasoftOriginalFileNameBox"; } }

	protected uint unknown; 
	public uint Unknown { get { return this.unknown; } set { this.unknown = value; } }

	protected BinaryUTF8String name; 
	public BinaryUTF8String Name { get { return this.name; } set { this.name = value; } }

	public PittasoftOriginalFileNameBox(): base(IsoStream.FromFourCC("ptnm"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown, "unknown"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.name, "name"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown, "unknown"); 
		boxSize += stream.WriteStringZeroTerminated( this.name, "name"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown
		boxSize += IsoStream.CalculateStringSize(name); // name
		return boxSize;
	}
}

}
