using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ITunesMetadataNameBox() extends FullBox('name') {
 string name; 
 }
*/
public partial class ITunesMetadataNameBox : FullBox
{
	public const string TYPE = "name";
	public override string DisplayName { get { return "ITunesMetadataNameBox"; } }

	protected BinaryUTF8String name; 
	public BinaryUTF8String Name { get { return this.name; } set { this.name = value; } }

	public ITunesMetadataNameBox(): base(IsoStream.FromFourCC("name"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.name, "name"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.name, "name"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(name); // name
		return boxSize;
	}
}

}
