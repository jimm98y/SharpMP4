using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class DataEntryUrnBox (bit(24) flags)
	extends DataEntryBaseBox('urn ', flags) {
	utf8string name;
	utf8string location;
}
*/
public partial class DataEntryUrnBox : DataEntryBaseBox
{
	public const string TYPE = "urn ";
	public override string DisplayName { get { return "DataEntryUrnBox"; } }

	protected BinaryUTF8String name; 
	public BinaryUTF8String Name { get { return this.name; } set { this.name = value; } }

	protected BinaryUTF8String location; 
	public BinaryUTF8String Location { get { return this.location; } set { this.location = value; } }

	public DataEntryUrnBox(uint flags = 0): base(IsoStream.FromFourCC("urn "), flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.name, "name"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.location, "location"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.name, "name"); 
		boxSize += stream.WriteStringZeroTerminated( this.location, "location"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(name); // name
		boxSize += IsoStream.CalculateStringSize(location); // location
		return boxSize;
	}
}

}
